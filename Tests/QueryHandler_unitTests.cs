using Application;
using Application.Dtos;
using Application.Queries.GetAllTasks;
using Application.Queries.GetCompleteTasks;
using Application.Queries.GetTaskById;
using AutoMapper;
using Domain;
using FluentAssertions;
using Moq;

namespace Tests;

public class QueryHandlerTests
{
    private Mock<ITaskRepository> _mockRepo;
    private Mock<IMapper> _mockMapper;
    
    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<ITaskRepository>();
        _mockMapper = new Mock<IMapper>();
    }

    [Test]
    public async Task GetAllTasksQueryHandler_ShouldReturnNull_WhenRepoReturnsNull()
    {
        // arrange
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync((List<TaskItem>)null!);

        var query = new GetAllTasksQuery();
        var queryHandler = new GetAllTasksQueryHandler(_mockRepo.Object, _mockMapper.Object);

        // act
        var result = await queryHandler.Handle(query, default);

        // assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetAllTasksQueryHandler_ShouldReturnDtoList_WhenRepoReturnsTaskItemList()
    {
        // arrange
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TaskItem> { new TaskItem() });
        _mockMapper.Setup(m => m.Map<List<TaskItemDto>>(It.IsAny<List<TaskItem>>()))
            .Returns(new List<TaskItemDto>{ new TaskItemDto() });

        var query = new GetAllTasksQuery();
        var queryHandler = new GetAllTasksQueryHandler(_mockRepo.Object, _mockMapper.Object);

        // act
        var result = await queryHandler.Handle(query, default);

        // assert
        result.Should().BeOfType<List<TaskItemDto>>();
    }

    [Test]
    public async Task GetCompleteTasksQueryHandler_ShouldReturnNull_WhenRepoReturnsNull()
    {
        // arrange
        _mockRepo.Setup(r => r.GetCompleteAsync()).ReturnsAsync((List<TaskItem>)null!);

        var query = new GetCompleteTasksQuery();
        var queryHandler = new GetCompleteTasksQueryHandler(_mockRepo.Object, _mockMapper.Object);

        // act
        var result = await queryHandler.Handle(query, default);

        // assert
        result.Should().BeNull();
    }
    
    [Test]
    public async Task GetCompleteTasksQueryHandler_ShouldReturnDtoList_WhenRepoReturnsTaskItemList()
    {
        // arrange
        _mockRepo.Setup(r => r.GetCompleteAsync()).ReturnsAsync(new List<TaskItem> { new TaskItem() });
        _mockMapper.Setup(m => m.Map<List<TaskItemDto>>(It.IsAny<List<TaskItem>>()))
            .Returns(new List<TaskItemDto>{ new TaskItemDto() });

        var query = new GetCompleteTasksQuery();
        var queryHandler = new GetCompleteTasksQueryHandler(_mockRepo.Object, _mockMapper.Object);

        // act
        var result = await queryHandler.Handle(query, default);

        // assert
        result.Should().BeOfType<List<TaskItemDto>>();
    }

    [Test]
    public async Task GetTaskByIdQueryHandler_ShouldReturnNull_WhenRepoReturnsNull()
    {
        // arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TaskItem)null!);

        var query = new GetTaskByIdQuery(It.IsAny<int>());
        var queryHandler = new GetTaskByIdQueryHandler(_mockRepo.Object, _mockMapper.Object);

        // act
        var result = await queryHandler.Handle(query, default);

        // assert
        result.Should().BeNull();
    }
    
    [Test]
    public async Task GetTaskByIdQueryHandler_ShouldReturnTaskItemDto_WhenRepoReturnsTaskItem()
    {
        // arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new TaskItem());
        _mockMapper.Setup(m => m.Map<TaskItemDto>(It.IsAny<TaskItem>())).Returns(new TaskItemDto());

        var query = new GetTaskByIdQuery(It.IsAny<int>());
        var queryHandler = new GetTaskByIdQueryHandler(_mockRepo.Object, _mockMapper.Object);

        // act
        var result = await queryHandler.Handle(query, default);

        // assert
        result.Should().BeOfType<TaskItemDto>();
    }
}