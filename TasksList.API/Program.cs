using TasksList.API.Middleware;
using TasksList.Application.Repositories;
using TasksList.Application.Services;
using TasksList.Infrastructure.Repositories;
using TasksList.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<ITaskListRepository, MongoTaskListRepository>();
builder.Services.AddScoped<ITaskListService, TaskListService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();
