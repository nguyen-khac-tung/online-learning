using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Online_Learning.Configurations;
using Online_Learning.Models.Entities;
using Online_Learning.Services.Implementations;
using Online_Learning.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OnlineLearningContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));
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
builder.Services.AddScoped<IFileService, FileService>();

// Optional: Add custom dependency injection configuration if defined
// builder.Services.AddDependencyInjectionConfiguration(builder.Configuration);

var app = builder.Build();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/uploads"
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseCors("AllowFrontend"); // Use the specific CORS policy
app.UseRouting();
app.UseStaticFiles();
app.UseAuthorization();

// Disable HTTPS redirection in development to avoid issues with frontend
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();