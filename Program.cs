using Microsoft.AspNetCore.Mvc;
using otp_service_template;
using otp_service_template.Services;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(o => 
{
    // TODO: Configure options to remove excess log info.
});
builder.Services.AddSingleton<AppSettings>();
builder.Services.AddScoped<IOneTimePasswordService, OneTimePasswordService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddHostedService<StartupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();
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

app.MapGet("/health", () =>
{
    return "healthy";
})
.WithName("Health")
.WithOpenApi();

app.MapPost("/register", async (
    [FromBody] RegisterUserRequest request, 
    [FromServices] IUserService userService, 
    [FromServices] IOneTimePasswordService otpService) =>
{
    if (string.IsNullOrEmpty(request?.Email))
        return Results.BadRequest(ApiError.InvalidEmail.AsApiResponse());

    var user = await userService.GetByEmail(request.Email);

    if (user is not null)
    {
        return Results.NoContent();
    }

    await userService.Save(new User()
    {
        Email = request.Email,
        FullName = request.FullName,
        Status = UserStatus.Created
    });

    await otpService.RequestValidation(request.Email);

    return Results.NoContent();
})
.WithName("RegisterUser")
.WithOpenApi();

app.MapPost("/register-confirmation", async (
    [FromBody] ConfirmRegisterRequest request, 
    [FromServices] IUserService userService, 
    [FromServices] IOneTimePasswordService otpService) =>
{
    if (string.IsNullOrEmpty(request?.Email))
        return Results.BadRequest(ApiError.InvalidEmail.AsApiResponse());

    var user = await userService.GetByEmail(request.Email);

    if (user is not null)
    {
        return Results.BadRequest();
    }

    await otpService.ConfirmValidation(request.Email, request.Password);

    return Results.Ok();
})
.WithName("ConfirmRegister")
.WithOpenApi();

app.MapPost("/validation-start", async (
    [FromServices] IOneTimePasswordService otpService, 
    [FromBody] StartValidationRequest request) =>
{
    if(string.IsNullOrEmpty(request.Email))
        return Results.BadRequest("Email required");

    await otpService.RequestValidation(request.Email);

    return Results.Ok();
})
.WithName("StartOtpRequest")
.WithOpenApi();

app.MapPost("/validation-confirmation", async (
    [FromServices] IOneTimePasswordService otpService, 
    [FromBody] ConfirmValidationRequest request) =>
{
    if(string.IsNullOrEmpty(request.Email))
        return Results.BadRequest("Email required");

    if(string.IsNullOrEmpty(request.Password))
        return Results.BadRequest("Password required");

    var result = await otpService.ConfirmValidation(request.Email, request.Password);

    if (result.Status != ConfirmationStatus.Authorized)
        return Results.Unauthorized();

    return Results.Ok(new {
        result.Token
    });
})
.WithName("ConfirmOtpRequest")
.WithOpenApi();

app.MapPost("/user", async (
    [FromServices] IOneTimePasswordService otpService, 
    [FromBody] string token) =>
{
    // TODO: get user data from token.
    var IsValid = await otpService.VerifyToken(token);
    if (IsValid == false)
        return Results.Unauthorized();

    return Results.Ok("Authorized");
})
.WithName("GetUser")
.WithOpenApi();

app.Run();