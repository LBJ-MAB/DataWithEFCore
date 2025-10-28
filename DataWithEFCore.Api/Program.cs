using Application;
using Domain;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TaskDb>();
builder.Services.AddScoped<ITaskRepository, DbTaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

var tasks = app.MapGroup("/tasks");
tasks.MapGet("/", async (TaskService service) => await service.GetAllTasks());
tasks.MapGet("/{id}", async (int id, TaskService service) => await service.GetTaskById(id: id));
tasks.MapGet("/complete", async (TaskService service) => await service.GetCompleteTasks());
tasks.MapPost("/", async (TaskService service, TaskItem task) => await service.AddTask(task: task));
tasks.MapPut("/{id}", async (TaskService service, int id, TaskItem inputTask) => await service.UpdateTask(id: id, inputTask: inputTask));
tasks.MapDelete("/{id}", async (TaskService service, int id) => await service.DeleteTask(id: id));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();