using Application;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Tests;

// define in memory database
// test TaskService results with in memory database

public class Tests
{
    private TaskService _service;
    private TaskDb _db;
    
    [SetUp]
    public void Setup()
    {
        // create in-memory database for testing
        var options = new DbContextOptionsBuilder<TaskDb>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _db = new TaskDb(options);
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
    
    [TearDown]
    public void TearDown()
    {
        _db.Database.EnsureDeleted();
        _db.Dispose();
    }
}