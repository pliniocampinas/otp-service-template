using Microsoft.AspNetCore.Mvc;
using otp_service_template;
using otp_service_template.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<AppSettings>();
builder.Services.AddScoped<IOneTimePasswordService, OneTimePasswordService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapGet("/health", () =>
{
    return "healthy";
})
.WithName("Health")
.WithOpenApi();

app.MapPost("/register", async ([FromBody] RegisterUserRequest request) =>
{
    await Task.Delay(1000);
    return Results.Created();
})
.WithName("register")
.WithOpenApi();

app.MapPost("/validation-start", async ([FromServices] IOneTimePasswordService otpService, [FromBody] StartValidationRequest request) =>
{
    if(string.IsNullOrEmpty(request.Email))
        return Results.BadRequest("Email required");

    await otpService.RequestValidation(request.Email);

    return Results.Ok();
})
.WithName("StartOtpRequest")
.WithOpenApi();

app.MapPost("/validation-confirmation", async ([FromServices] IOneTimePasswordService otpService, [FromBody] ConfirmValidationRequest request) =>
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

app.MapPost("/user", async ([FromServices] IOneTimePasswordService otpService, [FromBody] string token) =>
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

class RegisterUserRequest()
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

class StartValidationRequest()
{
    public string Email { get; set; } = string.Empty;
}

class ConfirmValidationRequest()
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
