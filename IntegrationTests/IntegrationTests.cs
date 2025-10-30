using Application;
using Domain;
using FluentAssertions;
using Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net;
using FluentValidation;

// using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace IntegrationTests;

public class IntegrationTests
{
    private HttpClient CreateTestClient()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                services.AddDbContext<TaskDb>(opt => 
                    opt.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
            }));
        return application.CreateClient();
    }
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task AddTask_ShouldReturnCreatedStatusCode_WhenValidTaskGiven()
    {
        // arrange - do not include id for task
        var task = new TaskItem
        {
            Title = "Task 1",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        var client = CreateTestClient();

        // act
        var result = await client.PostAsJsonAsync("/tasks", task);

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);  
    }

    [Test]
    public async Task AddTask_ShouldReturnBadRequestCode_WhenTitleIsEmpty()
    {
        // arrange
        var task = new TaskItem
        {
            Title = "    ",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        var client = CreateTestClient();

        // act
        var result = await client.PostAsJsonAsync("/tasks", task);

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task AddTask_ShouldReturnBadRequestCode_WhenTitleIsTooLong()
    {
        // arrange
        var task = new TaskItem
        {
            Title = "This title is too long",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        var client = CreateTestClient();

        // act
        var result = await client.PostAsJsonAsync("/tasks", task);

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task AddTask_ShouldReturnBadRequestCode_WhenStatusIsTrue()
    {
        // arrange
        var task = new TaskItem
        {
            Title = "Title",
            Description = null,
            Status = true,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        var client = CreateTestClient();

        // act
        var result = await client.PostAsJsonAsync("/tasks", task);

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task GetAllTasks_ShouldReturnBadRequestStatusCode_WhenNoTasksAvailable()
    {
        // arrange
        var client = CreateTestClient();

        // act
        var result = await client.GetAsync("/tasks");

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);  
    }

    [Test]
    public async Task GetAllTasks_ShouldReturnOkStatusCode_WhenTasksAvailable()
    {
        // arrange
        var client = CreateTestClient();
        var task = new TaskItem
        {
            Title = "Task 1",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        await client.PostAsJsonAsync("/tasks", task);

        // act
        var result = await client.GetAsync("/tasks");

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // code 400
        // no tasks are being returned
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}