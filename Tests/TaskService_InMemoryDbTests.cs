using Application;
using Domain;
using FluentAssertions;
using FluentValidation;
using Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class InMemoryDbTests
{
    private TaskDb _db;
    private DbTaskRepository _repo;
    private IValidator<TaskItem> _validator;
    private TaskService _service;
    
    [SetUp]
    public void Setup()
    {
        // create in-memory database for testing
        var options = new DbContextOptionsBuilder<TaskDb>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _db = new TaskDb(options);
        
        // organise db repo, fluent validator and task service
        _repo = new DbTaskRepository(_db);
        _validator = new TaskItemValidator();
        _service = new TaskService(_repo, _validator);
    }

    [Test]
    public async Task AddTask_ShouldReturnValidationProblem_WhenTitleHasNoLength()
    {
        // arrange
        var taskWithNoTitle = new TaskItem
        {
            Id = 1,
            Title = "",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };

        // act
        var result = await _service.AddTask(taskWithNoTitle);

        // assert
        result.Should().BeOfType<ValidationProblem>();
    }
    
    [Test]
    public async Task AddTask_ShouldReturnValidationProblem_WhenTitleHasLengthGreaterThan20()
    {
        // arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "the title of this task has a length greater than 20",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };

        // act
        var result = await _service.AddTask(task);

        // assert
        result.Should().BeOfType<ValidationProblem>();
    }
    
    [Test]
    public async Task AddTask_ShouldNotReturnValidationProblem_WhenTitleHasLengthBetween1and20()
    {
        // arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };

        // act
        var result = await _service.AddTask(task);

        // assert
        result.Should().NotBeOfType<ValidationProblem>();
    }
    
    [Test]
    public async Task AddTask_ShouldReturnCreated_WhenTitleHasLengthBetween1and20()
    {
        // arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };

        // act
        var result = await _service.AddTask(task);

        // assert
        result.Should().BeOfType<Created<TaskItem>>();
    }
    
    [Test]
    public async Task AddTask_ShouldReturnValidationProblem_WhenStatusIsTrue()
    {
        // arrange
        var taskWithTrueStatus = new TaskItem
        {
            Id = 1,
            Title = "test",
            Description = null,
            Status = true,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };

        // act
        var result = await _service.AddTask(taskWithTrueStatus);

        // assert
        result.Should().BeOfType<ValidationProblem>();
    }
    
    [Test]
    public async Task AddTask_ShouldNotReturnValidationProblem_WhenStatusIsFalse()
    {
        // arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "test",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };

        // act
        var result = await _service.AddTask(task);

        // assert
        result.Should().NotBeOfType<ValidationProblem>();
    }

    [Test]
    public async Task GetAllTasks_ShouldReturnBadRequest_WhenNoTasksAvailable()
    {
        // arrange
        
        // act
        var result = await _service.GetAllTasks();

        // assert
        result.Should().BeOfType<BadRequest<string>>();
    }
    
    [Test]
    public async Task GetAllTasks_ShouldNotReturnBadRequest_WhenTasksAvailable()
    {
        // arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        await _service.AddTask(task);
        
        // act
        var result = await _service.GetAllTasks();

        // assert
        result.Should().NotBeOfType<BadRequest>();
    }

    [Test]
    public async Task GetAllTasks_ShouldReturnOk_WhenTasksAvailable()
    {
        // arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        await _service.AddTask(task);
        
        // act
        var result = await _service.GetAllTasks();

        // assert
        result.Should().BeOfType<Ok<List<TaskItem>>>();
    }
    
    [Test]
    public async Task GetAllTasks_ShouldNotReturnOk_WhenTasksNotAvailable()
    {
        // arrange
        
        // act
        var result = await _service.GetAllTasks();

        // assert
        result.Should().NotBeOfType<Ok>();
    }

    [Test]
    public async Task GetAllTasks_ShouldReturnCorrectTasks_WhenTasksAvailable()
    {
        // arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        var taskList = new List<TaskItem>();
        taskList.Add(task);
        await _service.AddTask(task);
        
        // act
        var result = await _service.GetAllTasks();
        var okResult = result as Ok<List<TaskItem>>;

        // assert
        okResult!.Value.Should().BeEquivalentTo(taskList);
    }
    
    [Test]
    public async Task GetAllTasks_ShouldReturnCorrectLength_WhenTasksAvailable()
    {
        // arrange
        int numTasks = 3;
        for (int i = 1; i <= numTasks; i++)
        {
            var task = new TaskItem
            {
                Id = i,
                Title = $"task {i}",
                Description = null,
                Status = false,
                Priority = null,
                DueDate = null,
                CreatedAt = new DateTime(),
                UpdatedAt = null
            };
            await _service.AddTask(task);
        }
        
        // act
        var result = await _service.GetAllTasks();
        var okResult = result as Ok<List<TaskItem>>;

        // assert
        okResult!.Value.Should().HaveCount(numTasks);
    }

    [Test]
    public async Task GetTaskById_ShouldReturnBadRequest_WhenTaskNotAvailable()
    {
        // arrange
        
        // act
        const int id = 1;
        var result = await _service.GetTaskById(id);

        // assert
        result.Should().BeOfType<BadRequest<string>>();
    }

    [Test]
    public async Task GetTaskById_ShouldReturnOk_WhenTaskAvailable()
    {
        // arrange
        const int id = 1;
        var task = new TaskItem
        {
            Id = id,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        await _service.AddTask(task);

        // act
        var result = await _service.GetTaskById(id);

        // assert
        result.Should().BeOfType<Ok<TaskItem>>();
    }

    [Test]
    public async Task GetTaskById_ShouldReturnCorrectTask_WhenTaskAvailable()
    {
        // arrange
        const int id = 1;
        var task = new TaskItem
        {
            Id = id,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        await _service.AddTask(task);
        
        // act
        var result = await _service.GetTaskById(id);
        var okResult = result as Ok<TaskItem>;

        // assert
        okResult!.Value.Should().BeEquivalentTo(task);
    }

    [Test]
    public async Task GetCompleteTasks_ShouldReturnOnlyTasksWithTrueStatus_WhenTasksAvailable()
    {
        // arrange - cannot add a task with true status
        const int id = 1;
        var task = new TaskItem
        {
            Id = id,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        await _service.AddTask(task);
        await _service.UpdateTask(id, new TaskItem
        {
            Id = id,
            Title = "title",
            Description = null,
            Status = true,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        });

        // act
        var result = await _service.GetCompleteTasks();
        var okResult = result as Ok<List<TaskItem>>;

        // assert
        okResult!.Value.Should().AllSatisfy(t =>
        {
            t.Status.Should().Be(true);
        });
    }
    
    [Test]
    public async Task GetCompleteTasks_ShouldNotReturnAnyTasksWithFalseStatus_WhenTasksAvailable()
    {
        // arrange - cannot add a task with true status
        const int id = 1;
        for (int i = 1; i <= 2; i++)
        {
            var task = new TaskItem
            {
                Id = i,
                Title = $"task {i}",
                Description = null,
                Status = false,
                Priority = null,
                DueDate = null,
                CreatedAt = new DateTime(),
                UpdatedAt = null
            };
            await _service.AddTask(task);
        }
        await _service.UpdateTask(id, new TaskItem
        {
            Id = id,
            Title = $"task {id}",
            Description = null,
            Status = true,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        });

        // act
        var result = await _service.GetCompleteTasks();
        var okResult = result as Ok<List<TaskItem>>;

        // assert
        okResult!.Value.Should().NotContain(t => t.Status == false);
    }

    [Test]
    public async Task GetCompleteTasks_ShouldReturnBadRequest_WhenTasksNotAvailable()
    {
        // arrange
        
        // act
        var result = await _service.GetCompleteTasks();

        // assert
        result.Should().BeOfType<BadRequest<string>>();
    }

    [Test]
    public async Task GetCompleteTasks_ShouldReturnOk_WhenTasksAvailable()
    {
        // arrange - cannot add a task with true status
        const int id = 1;
        var task = new TaskItem
        {
            Id = id,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        await _service.AddTask(task);
        await _service.UpdateTask(id, new TaskItem
        {
            Id = id,
            Title = "title",
            Description = null,
            Status = true,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        });

        // act
        var result = await _service.GetCompleteTasks();

        // assert
        result.Should().BeOfType<Ok<List<TaskItem>>>();
    }

    [Test]
    public async Task UpdateTask_ShouldReturnNoContent_WhenGivenTaskWithValidId()
    {
        // arrange
        const int id = 1;
        var initialTask = new TaskItem
        {
            Id = id,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        await _service.AddTask(initialTask);
        var updatedTask = new TaskItem
        {
            Id = id,
            Title = "title",
            Description = "this task has been updated",
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };

        // act
        var result = await _service.UpdateTask(id, updatedTask);

        // assert
        result.Should().BeOfType<NoContent>();
    }

    [Test]
    public async Task UpdateTask_ShouldNotReturnNoContent_WhenGivenTaskWithInvalidId()
    {
        // arrange
        const int id = 1;
        var initialTask = new TaskItem
        {
            Id = id,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        await _service.AddTask(initialTask);
        var updatedTask = new TaskItem
        {
            Id = id,
            Title = "title",
            Description = "this task has been updated",
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };

        // act
        var result = await _service.UpdateTask(id+1, updatedTask);

        // assert
        result.Should().NotBeOfType<NoContent>(); 
    }
    
    [Test]
    public async Task UpdateTask_ShouldReturnBadRequest_WhenGivenTaskWithInvalidId()
    {
        // arrange
        const int id = 1;
        var initialTask = new TaskItem
        {
            Id = id,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        await _service.AddTask(initialTask);
        var updatedTask = new TaskItem
        {
            Id = id,
            Title = "title",
            Description = "this task has been updated",
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };

        // act
        var result = await _service.UpdateTask(id+1, updatedTask);

        // assert
        result.Should().BeOfType<BadRequest<string>>(); 
    }

    [Test]
    public async Task DeleteTask_ShouldReturnBadRequest_WhenGivenTaskWithInvalidId()
    {
        // arrange
        
        // act
        var result = await _service.DeleteTask(1);

        // assert
        result.Should().BeOfType<BadRequest<string>>();
    }

    [Test]
    public async Task DeleteTask_ShouldNotReturnBadRequest_WhenGivenTaskWithValidId()
    {
        // arrange
        const int id = 1;
        var task = new TaskItem
        {
            Id = id,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        await _service.AddTask(task);
        
        // act
        var result = await _service.DeleteTask(id);

        // assert
        result.Should().NotBeOfType<BadRequest<string>>();
    }

    [Test]
    public async Task DeleteTask_ShouldReturnNoContent_WhenGivenTaskWithValidId()
    {
        // arrange
        const int id = 1;
        var task = new TaskItem
        {
            Id = id,
            Title = "title",
            Description = null,
            Status = false,
            Priority = null,
            DueDate = null,
            CreatedAt = new DateTime(),
            UpdatedAt = null
        };
        await _service.AddTask(task);
        
        // act
        var result = await _service.DeleteTask(id);

        // assert
        result.Should().BeOfType<NoContent>();
    }

    [Test]
    public async Task DeleteTask_ShouldNotReturnNoContent_WhenGivenTaskWithInvalidId()
    {
        // arrange
        
        // act
        var result = await _service.DeleteTask(1);

        // assert
        result.Should().NotBeOfType<NoContent>();
    }
    
    [TearDown]
    public void TearDown()
    {
        _db.Database.EnsureDeleted();
        _db.Dispose();
    }
}