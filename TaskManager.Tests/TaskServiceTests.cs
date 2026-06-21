using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskManager.Tests;

[TestClass]
public class TaskServiceTests
{
    [TestMethod]
    public void Add_AssignsId()
    {
        var svc = new TaskService();
        var task = svc.Add(new TaskItem { Title = "Test" });
        Assert.AreNotEqual(0, task.Id);
    }

    [TestMethod]
    public void Add_SetsCreatedAt()
    {
        var svc = new TaskService();
        var task = svc.Add(new TaskItem { Title = "Test" });
        Assert.IsTrue(task.CreatedAt <= DateTime.Now);
    }

    [TestMethod]
    public void Add_IncreasesCount()
    {
        var svc = new TaskService();
        svc.Add(new TaskItem { Title = "First" });
        svc.Add(new TaskItem { Title = "Second" });
        Assert.AreEqual(2, svc.GetAll().Count);
    }

    [TestMethod]
    public void Update_ChangesTitle()
    {
        var svc = new TaskService();
        var task = svc.Add(new TaskItem { Title = "Old" });
        task.Title = "New";
        svc.Update(task);
        Assert.AreEqual("New", svc.GetAll()[0].Title);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Update_UnknownId_Throws()
        => new TaskService().Update(new TaskItem { Id = 99 });

    [TestMethod]
    public void Delete_RemovesTask()
    {
        var svc = new TaskService();
        var task = svc.Add(new TaskItem { Title = "ToDelete" });
        svc.Delete(task.Id);
        Assert.AreEqual(0, svc.GetAll().Count);
    }

    [TestMethod]
    public void Delete_NonExistentId_DoesNotThrow()
        => new TaskService().Delete(999);

    [TestMethod]
    public void GetByStatus_ReturnsOnlyMatching()
    {
        var svc = new TaskService();
        svc.Add(new TaskItem { Title = "T1", Status = WorkStatus.New });
        svc.Add(new TaskItem { Title = "T2", Status = WorkStatus.InProgress });
        svc.Add(new TaskItem { Title = "T3", Status = WorkStatus.Completed });

        var result = svc.GetByStatus(WorkStatus.New);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("T1", result[0].Title);
    }

    [TestMethod]
    public void GetByStatus_NoMatches_ReturnsEmpty()
    {
        var svc = new TaskService();
        svc.Add(new TaskItem { Status = WorkStatus.New });
        Assert.AreEqual(0, svc.GetByStatus(WorkStatus.Completed).Count);
    }

    [TestMethod]
    public void Search_FindsByTitle()
    {
        var svc = new TaskService();
        svc.Add(new TaskItem { Title = "Buy groceries" });
        svc.Add(new TaskItem { Title = "Call doctor" });

        var result = svc.Search("groceries");
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Buy groceries", result[0].Title);
    }

    [TestMethod]
    public void Search_FindsByDescription()
    {
        var svc = new TaskService();
        svc.Add(new TaskItem { Title = "Task",  Description = "Important meeting" });
        svc.Add(new TaskItem { Title = "Other", Description = "Buy milk" });
        Assert.AreEqual(1, svc.Search("meeting").Count);
    }

    [TestMethod]
    public void Search_IsCaseInsensitive()
    {
        var svc = new TaskService();
        svc.Add(new TaskItem { Title = "Buy Groceries" });
        Assert.AreEqual(1, svc.Search("GROCERIES").Count);
    }

    [TestMethod]
    public void Search_EmptyQuery_ReturnsAll()
    {
        var svc = new TaskService();
        svc.Add(new TaskItem { Title = "First" });
        svc.Add(new TaskItem { Title = "Second" });
        Assert.AreEqual(2, svc.Search("").Count);
    }

    [TestMethod]
    public void SaveAndLoad_PreservesData()
    {
        var svc = new TaskService();
        svc.Add(new TaskItem { Title = "T1", Priority = Priority.High, Status = WorkStatus.InProgress });
        svc.Add(new TaskItem { Title = "T2", DueDate = new DateTime(2025, 12, 31) });

        var path = Path.GetTempFileName();
        try
        {
            svc.SaveToJson(path);

            var svc2 = new TaskService();
            svc2.LoadFromJson(path);
            var tasks = svc2.GetAll();

            Assert.AreEqual(2, tasks.Count);
            Assert.AreEqual("T1",                      tasks[0].Title);
            Assert.AreEqual(Priority.High,             tasks[0].Priority);
            Assert.AreEqual(WorkStatus.InProgress,     tasks[0].Status);
            Assert.AreEqual(new DateTime(2025, 12, 31), tasks[1].DueDate);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void LoadFromJson_MissingFile_Throws()
        => new TaskService().LoadFromJson("no.json");

    [TestMethod]
    public void LoadFromJson_ResetsIdCounter()
    {
        var svc = new TaskService();
        svc.Add(new TaskItem { Title = "First" });
        svc.Add(new TaskItem { Title = "Second" });

        var path = Path.GetTempFileName();
        try
        {
            svc.SaveToJson(path);
            var svc2 = new TaskService();
            svc2.LoadFromJson(path);
            Assert.IsTrue(svc2.Add(new TaskItem { Title = "Third" }).Id > 2);
        }
        finally { File.Delete(path); }
    }
}
