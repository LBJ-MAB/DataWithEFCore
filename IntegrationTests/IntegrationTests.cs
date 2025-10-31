using Bogus;
using Domain;
using FluentAssertions;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace IntegrationTests;

public class IntegrationTests
{
    private HttpClient CreateTestClient()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                services.AddDbContext<TaskDb>(opt => 
                    opt.UseInMemoryDatabase(databaseName: "TestDatabase"));
            }));

        ClearDatabase(application);
        
        return application.CreateClient();
    }

    private void ClearDatabase(WebApplicationFactory<Program> application)
    {
        using (var scope = application.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TaskDb>();
            db.Tasks.RemoveRange(db.Tasks);
            db.SaveChanges();
        }
    }

    private TaskItem CreateBogusTask(string? title = null, bool status = false)
    {
        var taskFaker = new Faker<TaskItem>()
            .RuleFor(t => t.Title, f => title ?? f.Random.Word())
            .RuleFor(t => t.Status, _=> status)
            .RuleFor(t => t.CreatedAt, f => f.DateTimeReference.GetValueOrDefault())
            .RuleFor(t => t.Description, f => f.Random.String())
            .RuleFor(t => t.Priority, f => f.Random.Int())
            .RuleFor(t => t.DueDate, f => f.DateTimeReference.GetValueOrDefault())
            .RuleFor(t => t.UpdatedAt, f => f.DateTimeReference.GetValueOrDefault());

        return taskFaker.Generate(1).ElementAt(0); 
    }
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task PostRequest_ShouldReturnCreatedStatusCode_WhenValidTaskGiven()
    {
        // arrange
        var task = CreateBogusTask();
        var client = CreateTestClient();

        // act
        var result = await client.PostAsJsonAsync("/tasks", task);

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);  
    }

    [Test]
    public async Task PostRequest_ShouldReturnBadRequestCode_WhenTitleIsEmpty()
    {
        // arrange
        var task = CreateBogusTask(title: "   ");
        var client = CreateTestClient();

        // act
        var result = await client.PostAsJsonAsync("/tasks", task);

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task PostRequest_ShouldReturnBadRequestCode_WhenTitleIsTooLong()
    {
        // arrange
        var task = CreateBogusTask(title: "this title is too long");
        var client = CreateTestClient();

        // act
        var result = await client.PostAsJsonAsync("/tasks", task);

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task PostRequest_ShouldReturnBadRequestCode_WhenStatusIsTrue()
    {
        // arrange
        var task = CreateBogusTask(status: true);
        var client = CreateTestClient();

        // act
        var result = await client.PostAsJsonAsync("/tasks", task);

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task GetRequest_ShouldReturnBadRequestStatusCode_WhenNoTasksAvailable()
    {
        // arrange
        var client = CreateTestClient();

        // act
        var result = await client.GetAsync("/tasks");

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);  
    }

    [Test]
    public async Task GetRequest_ShouldReturnOkStatusCode_WhenTasksAvailable()
    {
        // arrange
        var client = CreateTestClient();
        // test fails if you don't provide a valid title!???
        var task = CreateBogusTask(title : "title");
        
        await client.PostAsJsonAsync("/tasks", task);

        // act
        var result = await client.GetAsync("/tasks");

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetRequestWithId_ShouldReturnBadRequestStatusCode_WhenInvalidIdGiven()
    {
        // arrange
        var client = CreateTestClient();
        const int id = 4;
        
        // act
        var result = await client.GetAsync($"/tasks/{id}");

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task GetRequestWithId_ShouldReturnOKStatusCode_WhenValidIdGiven()
    {
        // arrange
        var client = CreateTestClient();
        const int id = 1;
        var task = CreateBogusTask();
        await client.PostAsJsonAsync("/tasks", task);
        
        // act
        var result = await client.GetAsync($"/tasks/{id}");

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetRequestWithComplete_ShouldReturnBadRequest_WhenNoCompleteTasksAvailable()
    {
        // arrange
        var client = CreateTestClient();

        // act
        var result = await client.GetAsync("/tasks/complete");

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task GetRequestWithComplete_ShouldReturnOK_WhenCompleteTasksAvailable()
    {
        // arrange
        var client = CreateTestClient();
        const int id = 1;
        var task = CreateBogusTask();
        await client.PostAsJsonAsync("/tasks", task);   // pass - code 201
        var completeTask = CreateBogusTask(status: true);
        await client.PutAsJsonAsync($"/tasks/{id}", completeTask); // pass - code 204
        
        // act
        var result = await client.GetAsync("/tasks/complete");

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task PutRequest_ShouldReturnBadRequest_WhenInvalidIdGiven()
    {
        // arrange
        var client = CreateTestClient();
        const int id = 1;
        var task = CreateBogusTask();
        
        // act
        var result = await client.PutAsJsonAsync($"/tasks/{id}", task);

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task PutRequest_ShouldReturnNoContent_WhenValidIdGiven()
    {
        // arrange
        var client = CreateTestClient();
        const int id = 1;
        var task = CreateBogusTask();
        await client.PostAsJsonAsync("/tasks", task);
        
        // act
        var result = await client.PutAsJsonAsync($"/tasks/{id}", task);

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task DeleteRequest_ShouldReturnBadRequest_WhenInvalidIdGiven()
    {
        // arrange
        var client = CreateTestClient();
        const int id = 1;
        
        // act
        var result = await client.DeleteAsync($"/tasks/{id}");

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task DeleteRequest_ShouldReturnNoContent_WhenValidIdGiven()
    {
        // arrange
        var client = CreateTestClient();
        const int id = 1;
        var task = CreateBogusTask();
        await client.PostAsJsonAsync("/tasks", task);
        
        // act
        var result = await client.DeleteAsync($"/tasks/{id}");

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task GetRequest_ShouldReturnCorrectTasks_AfterPostRequest()
    {
        // arrange
        var client = CreateTestClient();
        const int id = 1;
        var task = new TaskItem
        {
            Title = "task",
            Status = false,
            CreatedAt = new DateTime()
        };
        await client.PostAsJsonAsync("/tasks", task);
        var expectedTask = new TaskItem
        {
            Id = id,
            Title = "task",
            Status = false,
            CreatedAt = new DateTime()
        };

        // act
        var result = await client.GetAsync("/tasks");
        var taskList = await result.Content.ReadFromJsonAsync<List<TaskItem>>();

        // assert
        taskList.Should().ContainEquivalentOf(expectedTask, "because this task was added to the db");
    }

    [Test]
    public async Task GetRequestWithId_ShouldReturnCorrectTask_AfterPostRequest()
    {
        // arrange
        var client = CreateTestClient();
        const int id = 1;
        var task = new TaskItem
        {
            Title = "task",
            Status = false,
            CreatedAt = new DateTime()
        };
        await client.PostAsJsonAsync("/tasks", task);
        var expectedTask = new TaskItem
        {
            Id = id,
            Title = "task",
            Status = false,
            CreatedAt = new DateTime()
        };

        // act
        var result = await client.GetAsync($"/tasks/{id}");
        var returnedTask = await result.Content.ReadFromJsonAsync<TaskItem>();

        // assert
        returnedTask.Should().BeEquivalentTo(expectedTask, "because this task was added to the db");
    }

    [Test]
    public async Task GetRequestWithComplete_ShouldReturnCorrectTask_AfterPostAndPutRequest()
    {
        // arrange
        var client = CreateTestClient();
        const int id = 1;
        var expectedTask = new TaskItem
        {
            Id = id,
            Title = "task",
            Status = true,
            CreatedAt = new DateTime()
        };
        await client.PostAsJsonAsync("/tasks", new TaskItem
        {
            Title = "task",
            Status = false,
            CreatedAt = new DateTime()
        });
        await client.PutAsJsonAsync($"/tasks/{id}", new TaskItem
        {
            Title = "task",
            Status = true,
            CreatedAt = new DateTime()
        });

        // act
        var result = await client.GetAsync("/tasks/complete");
        var taskListResult = await result.Content.ReadFromJsonAsync<List<TaskItem>>();

        // assert
        taskListResult.Should().ContainEquivalentOf(expectedTask);
    }

    [Test]
    public async Task GetRequest_ShouldNotReturnDeletedTask_AfterDeleteRequest()
    {
        // arrange
        var client = CreateTestClient();
        const int id = 1;
        var task = new TaskItem
        {
            Title = "task",
            Status = false,
            CreatedAt = new DateTime()
        };
        await client.PostAsJsonAsync("/tasks", task);
        await client.PostAsJsonAsync("/tasks", task);
        await client.DeleteAsync($"/tasks/{id}");

        // act
        var result = await client.GetAsync("/tasks");
        
        // assert
        try
        {
            var taskListResult = await result.Content.ReadFromJsonAsync<List<TaskItem>>();
            taskListResult.Should().NotContain(t => t.Id == id);
        }
        catch
        {
            var stringResult = await result.Content.ReadAsStringAsync();
            stringResult.Should().Be("no tasks were found");
        }
    }
}