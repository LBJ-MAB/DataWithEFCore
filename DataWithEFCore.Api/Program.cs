using Application;
using Domain;
using FluentValidation;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddDbContext<TaskDb>(opt => 
//     opt.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=TaskDB;Trusted_Connection=True;"));
builder.Services.AddScoped<ITaskRepository, DbTaskRepository>();
builder.Services.AddScoped<IValidator<TaskItem>, TaskItemValidator>();
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

var tasks = app.MapGroup("/tasks");
tasks.MapGet("/", async (ITaskService service) => await service.GetAllTasks());
tasks.MapGet("/{id}", async (ITaskService service, int id) => await service.GetTaskById(id: id));
tasks.MapGet("/complete", async (ITaskService service) => await service.GetCompleteTasks());
tasks.MapPost("/", async (ITaskService service, [FromBody] TaskItem task) => await service.AddTask(task: task));
tasks.MapPut("/{id}", async (ITaskService service, int id, [FromBody] TaskItem inputTask) => await service.UpdateTask(id: id, inputTask: inputTask));
tasks.MapDelete("/{id}", async (ITaskService service, int id) => await service.DeleteTask(id: id));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

public partial class Program { }