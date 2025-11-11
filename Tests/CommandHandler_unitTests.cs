using Application;
using Application.Commands.AddTask;
using Application.Commands.DeleteTask;
using Application.Commands.UpdateTask;
using Application.Dtos;
using AutoMapper;
using Domain;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Tests;

public class CommandHandlerUnitTests
{
    // mock repo, validator and mapper
    private Mock<ITaskRepository> _mockRepo;
    private Mock<IValidator<TaskItem>> _mockAddTaskValidator;
    private Mock<IValidator<TaskItemDto>> _mockUpdateTaskValidator;
    private Mock<IMapper> _mockMapper;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<ITaskRepository>();
        _mockAddTaskValidator = new Mock<IValidator<TaskItem>>();
        _mockUpdateTaskValidator = new Mock<IValidator<TaskItemDto>>();
        _mockMapper = new Mock<IMapper>();
    }

    [Test]
    public async Task AddTaskCommandHandler_ShouldReturnAddTaskResultType()
    {
        // arrange
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
        _mockAddTaskValidator.Setup(v => v.ValidateAsync(It.IsAny<TaskItem>(), default))
            .ReturnsAsync(new ValidationResult());
        _mockMapper.Setup(m => m.Map<TaskItem>(It.IsAny<TaskItemDto>()))
            .Returns(new TaskItem());
        
        var command = new AddTaskCommand(It.IsAny<TaskItemDto>());
        var commandHandler = new AddTaskCommandHandler(_mockRepo.Object, _mockAddTaskValidator.Object, _mockMapper.Object);

        // act
        var result = await commandHandler.Handle(command, default);

        // assert
        result.Should().BeOfType<AddTaskResult>();
    }

    [Test]
    public async Task AddTaskCommandHandler_ShouldReturnUnsuccessful_WhenValidationFails()
    {
        // arrange
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
        _mockAddTaskValidator.Setup(v => v.ValidateAsync(It.IsAny<TaskItem>(), default))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("property", "error") }));
        _mockMapper.Setup(m => m.Map<TaskItem>(It.IsAny<TaskItemDto>()))
            .Returns(new TaskItem());

        var command = new AddTaskCommand(It.IsAny<TaskItemDto>());
        var commandHandler =
            new AddTaskCommandHandler(_mockRepo.Object, _mockAddTaskValidator.Object, _mockMapper.Object);

        // act
        var result = await commandHandler.Handle(command, default);

        // assert
        result.Success.Should().Be(false);
    }

    [Test]
    public async Task AddTaskCommandHandler_ShouldReturnSuccessful_WhenValidationPasses()
    {
        // arrange
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
        _mockAddTaskValidator.Setup(v => v.ValidateAsync(It.IsAny<TaskItem>(), default))
            .ReturnsAsync(new ValidationResult());
        _mockMapper.Setup(m => m.Map<TaskItem>(It.IsAny<TaskItemDto>()))
            .Returns(new TaskItem());

        var command = new AddTaskCommand(It.IsAny<TaskItemDto>());
        var commandHandler = new AddTaskCommandHandler(_mockRepo.Object, _mockAddTaskValidator.Object, _mockMapper.Object);

        // act
        var result = await commandHandler.Handle(command, default);

        // assert
        result.Success.Should().Be(true);
    }

    [Test]
    public async Task DeleteTaskCommandHandler_ShouldReturnFalse_WhenRepoReturnsNull()
    {
        // arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TaskItem)null!);

        var command = new DeleteTaskCommand(It.IsAny<int>());
        var commandHandler = new DeleteTaskCommandHandler(_mockRepo.Object);

        // act
        var result = await commandHandler.Handle(command, default);

        // assert
        result.Should().Be(false);
    }
    
    [Test]
    public async Task DeleteTaskCommandHandler_ShouldReturnTrue_WhenRepoReturnsTaskItem()
    {
        // arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new TaskItem());
        _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);

        var command = new DeleteTaskCommand(It.IsAny<int>());
        var commandHandler = new DeleteTaskCommandHandler(_mockRepo.Object);

        // act
        var result = await commandHandler.Handle(command, default);

        // assert
        result.Should().Be(true);
    }

    [Test]
    public async Task UpdateTaskCommandHandler_ShouldReturnFoundButUnsuccessful_WhenValidationFails()
    {
        // arrange
        _mockUpdateTaskValidator.Setup(v => v.ValidateAsync(It.IsAny<TaskItemDto>(), default))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("property", "error") }));
        
        var command = new UpdateTaskCommand(It.IsAny<int>(), It.IsAny<TaskItemDto>());
        var commandHandler = new UpdateTaskCommandHandler(_mockRepo.Object, _mockUpdateTaskValidator.Object);

        // act
        var result = await commandHandler.Handle(command, default);

        // assert
        result.NotFound.Should().Be(false);
        result.Success.Should().Be(false);
    }
    
    [Test]
    public async Task UpdateTaskCommandHandler_ShouldReturnNotFoundAndUnsuccessful_WhenRepoReturnsNull()
    {
        // arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TaskItem)null!);
        _mockUpdateTaskValidator.Setup(v => v.ValidateAsync(It.IsAny<TaskItemDto>(), default))
            .ReturnsAsync(new ValidationResult());
        
        var command = new UpdateTaskCommand(It.IsAny<int>(), It.IsAny<TaskItemDto>());
        var commandHandler = new UpdateTaskCommandHandler(_mockRepo.Object, _mockUpdateTaskValidator.Object);

        // act
        var result = await commandHandler.Handle(command, default);

        // assert
        result.NotFound.Should().Be(true);
        result.Success.Should().Be(false);
    }
    
    [Test]
    public async Task UpdateTaskCommandHandler_ShouldReturnSuccessful_WhenRepoReturnsTaskItemAndValidationPasses()
    {
        // arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new TaskItem());
        _mockUpdateTaskValidator.Setup(v => v.ValidateAsync(It.IsAny<TaskItemDto>(), default))
            .ReturnsAsync(new ValidationResult());
        
        var command = new UpdateTaskCommand(It.IsAny<int>(), new TaskItemDto());
        var commandHandler = new UpdateTaskCommandHandler(_mockRepo.Object, _mockUpdateTaskValidator.Object);

        // act
        var result = await commandHandler.Handle(command, default);

        // assert
        result.Success.Should().Be(true);
    }
}