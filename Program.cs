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

// Cấu hình DI_Admin
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();

builder.Services.AddScoped<Online_Learning.Services.Interfaces.Admin.ICourseService, Online_Learning.Services.Implementations.Admin.CourseService>();
builder.Services.AddScoped<Online_Learning.Repositories.Interfaces.Admin.ICourseRepository, Online_Learning.Repositories.Implementations.Admin.CourseRepository>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<ILessonRepository, Online_Learning.Repositories.Implementations.Admin.LessonRepository>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IOptionService, OptionService>();
builder.Services.AddScoped<IOptionRepository, OptionRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<Online_Learning.Repositories.Interfaces.Admin.ICategoryRepository, Online_Learning.Repositories.Implementations.Admin.CategoryRepository>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<ILevelService, LevelService>();
builder.Services.AddScoped<ILevelRepository, LevelRepository>();

var app = builder.Build();
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
//    RequestPath = "/uploads"
//});
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
