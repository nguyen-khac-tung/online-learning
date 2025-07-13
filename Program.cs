using Microsoft.EntityFrameworkCore;
using Online_Learning.Configurations;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Implementations;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Implementations;
using Online_Learning.Services.Interfaces;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		// convert value cua enum
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OnlineLearningContext>(option => option.UseSqlServer(
		builder.Configuration.GetConnectionString("MyCnn")
	));
builder.Services.AddCors();

// Cấu hình DI
builder.Services.AddDependencyInjectionConfiguration(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseCors(c =>
{
	c.AllowAnyHeader();
	c.AllowAnyMethod();
	c.AllowAnyOrigin();
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
