using Application;
using Domain;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Tests;

public class UnitTests
{
    private Mock<ITaskRepository> _mockRepo;
    private Mock<IValidator<TaskItem>> _mockValidator;
    private TaskService _service;
    
    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<ITaskRepository>();
        _mockValidator = new Mock<IValidator<TaskItem>>();
        _service = new TaskService(_mockRepo.Object, _mockValidator.Object);
    }

    [Test]
    public async Task GetAllTasks_CallsRepositoryGetAllAsync()
    {
        // arrange
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TaskItem>());

        // act
        await _service.GetAllTasks();

        // assert
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetTaskById_CallsRepositoryGetByIdAsync()
    {
        // arrange
        const int id = 1;
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new TaskItem());

        // act
        await _service.GetTaskById(id);

        // assert
        _mockRepo.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Test]
    public async Task GetCompleteTasks_CallsRepositoryGetCompleteAsync()
    {
        // arrange
        _mockRepo.Setup(r => r.GetCompleteAsync()).ReturnsAsync(new List<TaskItem>());

        // act
        await _service.GetCompleteTasks();

        // assert
        _mockRepo.Verify(r => r.GetCompleteAsync(), Times.Once);
    }

    [Test]
    public async Task AddTask_CallsRepositoryAddAsync_WhenValidTaskProvided()
    {
        // arrange - needs to be a valid task
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
        _mockRepo.Setup(r => r.AddAsync(task)).Returns(Task.CompletedTask);
        _mockValidator.Setup(v => v.ValidateAsync(task, CancellationToken.None)).ReturnsAsync(new ValidationResult());

        // act
        await _service.AddTask(task);

        // assert
        _mockRepo.Verify(r => r.AddAsync(task), Times.Once);
    }

    [Test]
    public async Task UpdateTask_CallsRepositoryGetByIdAsync_WhenGivenValidId()
    {
        // arrange
        const int id = 1;
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new TaskItem());
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // act
        await _service.UpdateTask(1, new TaskItem());

        // assert
        _mockRepo.Verify(r => r.GetByIdAsync(id), Times.Once);
    }
    
    [Test]
    public async Task UpdateTask_CallsRepositorySaveChangesAsync_WhenGivenValidId()
    {
        // arrange
        const int id = 1;
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new TaskItem());
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // act
        await _service.UpdateTask(1, new TaskItem());

        // assert
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteTask_CallsRepositoryDeleteAsync_WhenGivenValidTask()
    {
        // arrange
        const int id = 1;
        var task = new TaskItem
        {
            Id = 1
        };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(task);
        _mockRepo.Setup(r => r.DeleteAsync(task)).Returns(Task.CompletedTask);
        
        // act
        await _service.DeleteTask(id);
        
        // assert
        _mockRepo.Verify(r => r.DeleteAsync(task), Times.Once);
    }

    [Test]
    public async Task GetAllTasks_ReturnsOk_WhenGetAllAsyncReturnsListOfTaskItems()
    {
        // arrange
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TaskItem>
        {
            new TaskItem()
        });
        
        // act
        var result = await _service.GetAllTasks();
        
        // assert
        result.Should().BeOfType<Ok<List<TaskItem>>>();
    }

    [Test]
    public async Task GetAllTasks_ReturnsBadRequest_WhenGetAllAsyncReturnsEmptyList()
    {
        // arrange
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TaskItem>());
        
        // act
        var result = await _service.GetAllTasks();

        // assert
        result.Should().BeOfType<BadRequest<string>>();
    }

    [Test]
    public async Task GetTaskById_ReturnsOk_WhenGetByIdAsyncReturnsTaskItem()
    {
        // arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new TaskItem());
        
        // act
        var result = await _service.GetTaskById(It.IsAny<int>());
        
        // assert
        result.Should().BeOfType<Ok<TaskItem>>();
    }
    
    [Test]
    public async Task GetTaskById_ReturnsBadRequest_WhenGetByIdAsyncReturnsNull()
    {
        // arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TaskItem)null);
        
        // act
        var result = await _service.GetTaskById(It.IsAny<int>());
        
        // assert
        result.Should().BeOfType<BadRequest<string>>();
    }

    [Test]
    public async Task GetCompleteTasks_ReturnsOk_WhenGetCompleteAsyncReturnsListOfTaskItems()
    {
        // arrange
        _mockRepo.Setup(r => r.GetCompleteAsync()).ReturnsAsync(new List<TaskItem>
        {
            new TaskItem()
        });
        
        // act
        var result = await _service.GetCompleteTasks();
        
        // assert
        result.Should().BeOfType<Ok<List<TaskItem>>>();
    }

    [Test]
    public async Task GetCompleteTasks_ReturnsBadRequest_WhenGetCompleteAsyncReturnsEmptyList()
    {
        // arrange
        _mockRepo.Setup(r => r.GetCompleteAsync()).ReturnsAsync(new List<TaskItem>());
        
        // act
        var result = await _service.GetCompleteTasks();
        
        // assert
        result.Should().BeOfType<BadRequest<string>>();
    }

    [Test]
    public async Task AddTask_ReturnsCreated_WhenValidatorReturnsTrue()
    {
        // arrange
        var task = new TaskItem();
        _mockRepo.Setup(r => r.AddAsync(task)).Returns(Task.CompletedTask);
        _mockValidator.Setup(v => v.ValidateAsync(task, CancellationToken.None)).ReturnsAsync(new ValidationResult());
        
        // act
        var result = await _service.AddTask(task);
        
        // assert
        result.Should().BeOfType<Created<TaskItem>>();
    }

    [Test]
    public async Task AddTask_ReturnsValidationProblem_WhenValidatorReturnsValidationFailure()
    {
        // arrange
        var task = new TaskItem();
        _mockRepo.Setup(r => r.AddAsync(task)).Returns(Task.CompletedTask);
        _mockValidator.Setup(v => v.ValidateAsync(task, CancellationToken.None))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("property", "error") }));
        
        // act
        var result = await _service.AddTask(task);
        
        // assert
        result.Should().BeOfType<ValidationProblem>();
    }

    [Test]
    public async Task UpdateTask_ReturnsBadRequest_WhenGetByIdAsyncReturnsNull()
    {
        // arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TaskItem)null);

        // act
        var result = await _service.UpdateTask(It.IsAny<int>(), new TaskItem());

        // assert
        result.Should().BeOfType<BadRequest<string>>();
    }

    [Test]
    public async Task UpdateTask_ReturnsNoContent_WhenGetByIdAsyncReturnsTaskItem()
    {
        // arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((new TaskItem()));

        // act
        var result = await _service.UpdateTask(It.IsAny<int>(), new TaskItem());

        // assert
        result.Should().BeOfType<NoContent>();
    }

    [Test]
    public async Task DeleteTask_ReturnsBadRequest_WhenGetByIdAsyncReturnsNull()
    {
        // arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TaskItem)null);

        // act
        var result = await _service.DeleteTask(It.IsAny<int>());

        // assert
        result.Should().BeOfType<BadRequest<string>>();
    }
    
    [Test]
    public async Task DeleteTask_ReturnsNoContent_WhenGetByIdAsyncReturnsTaskItem()
    {
        // arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new TaskItem());

        // act
        var result = await _service.DeleteTask(It.IsAny<int>());

        // assert
        result.Should().BeOfType<NoContent>();
    }
}