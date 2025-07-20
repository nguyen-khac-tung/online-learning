using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Online_Learning.Configurations;
using Online_Learning.Models.DTOs.Response.Auth;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Implementations.Admin;
using Online_Learning.Repositories.Interfaces.Admin;
using Online_Learning.Services.Implementations.Admin;
using Online_Learning.Services.Interfaces.Admin;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<OnlineLearningContext>(option => option.UseSqlServer(
        builder.Configuration.GetConnectionString("MyCnn")
    ));

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		// convert value cua enum
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
		};
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Updated to match the new frontend port
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});


// DI Configuration
builder.Services.AddDependencyInjectionConfiguration(builder.Configuration);


var app = builder.Build();


var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

// Cấu hình Static Files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowFrontend"); // Use the specific CORS policy
app.UseCors(c =>
{
	c.AllowAnyHeader();
	c.AllowAnyMethod();
	c.AllowAnyOrigin();
});

app.UseHttpsRedirection();

app.UseRouting();
//app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
