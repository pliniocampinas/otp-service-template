using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using otp_service_template;
using otp_service_template.Services;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(o => {
    o.LoggingFields = HttpLoggingFields.All;
    o.CombineLogs = true;
});
builder.Services.AddSingleton<AppSettings>();
builder.Services.AddSingleton<CacheService>();
builder.Services.AddScoped<IOneTimePasswordService, OneTimePasswordService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddHostedService<StartupService>();
builder.Services.AddHostedService<TimedHostedService>();
builder.Services.AddExceptionHandler<ApiErrorHandler>();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpLogging();
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = Text.Plain;
        await context.Response.WriteAsync("An exception was thrown.");
    });
});
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapDefaultControllerRoute();
app.MapGet("/health", () => "healthy");

app.Run();