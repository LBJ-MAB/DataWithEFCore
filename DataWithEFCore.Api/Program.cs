using Application;
using Application.Commands.AddTask;
using Application.Commands.DeleteTask;
using Application.Commands.UpdateTask;
using Application.Dtos;
using Application.Mapping;
using Application.Queries.GetAllTasks;
using Application.Queries.GetCompleteTasks;
using Application.Queries.GetTaskById;
using Application.Validation;
using AutoMapper;
using Domain;
using FluentValidation;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TaskDb>(opt => 
     opt.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=TaskDB;Trusted_Connection=True;"));
builder.Services.AddScoped<ITaskRepository, DbTaskRepository>();
builder.Services.AddScoped<IValidator<TaskItem>, AddTaskValidator>();
builder.Services.AddScoped<IValidator<TaskItemDto>, UpdateTaskValidator>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(GetTaskByIdQueryHandler).Assembly));
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

var app = builder.Build();
app.MapGet("/", () => "add /swagger to url");

var tasks = app.MapGroup("/tasks");
tasks.MapPost("/", async ([FromBody] TaskItemDto taskItemDto, ISender sender) =>
{
    var command = new AddTaskCommand(taskItemDto);
    var result = await sender.Send(command);
    return result.Success ? Results.Created($"/tasks/{result.TaskItem!.Id}", result) : Results.ValidationProblem(result.Errors!);
});
tasks.MapGet("/{id}", async (int id, ISender sender) =>
{
    var query = new GetTaskByIdQuery(id);
    var result = await sender.Send(query);
    return result is null ? Results.NotFound() : Results.Ok(result);
});
tasks.MapGet("/", async (ISender sender) =>
{
    var query = new GetAllTasksQuery();
    var result = await sender.Send(query);
    return result is null ? Results.NotFound() : Results.Ok(result);
});
tasks.MapGet("/complete", async (ISender sender) =>
{
    var query = new GetCompleteTasksQuery();
    var result = await sender.Send(query);
    return result is null ? Results.NotFound() : Results.Ok(result);
});
tasks.MapPut("/{id}", async (ISender sender, int id, [FromBody] TaskItemDto inputTaskDto) =>
{
    var command = new UpdateTaskCommand(id, inputTaskDto);
    var result = await sender.Send(command);
    return result.Success ? Results.NoContent() : result.NotFound ? Results.NotFound() : Results.ValidationProblem(result.Errors!);
});
tasks.MapDelete("/{id}", async (ISender sender, int id) =>
{
    var command = new DeleteTaskCommand(id);
    var result = await sender.Send(command);
    return result ? Results.NoContent() : Results.NotFound();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

public partial class Program { }