using Saga.Orchestration.Persister;
using Saga.Orchestration.Utils;
using SagaByW.Orchestrators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ImportSaleOrchestrator>();
builder.Services.AddScoped<ISagaLogPersister,SagaLogPersister>();
builder.Services.AddScoped<ISagaLogger, SagaLogger>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseAuthorization();

app.MapControllers();

app.Run();
