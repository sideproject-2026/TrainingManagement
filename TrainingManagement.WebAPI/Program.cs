using Carter;
using Scalar.AspNetCore;
using TrainingManagement.Auth.Commons.Extensions;
using TrainingManagement.WebAPI.Commons.Extensions;
using TrainingManagement.Center.Commons.Extensions;
using TrainingManagement.WebAPI.Commons.Errors;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApiVersioning(opt => opt.ReportApiVersions = true);

builder.Services
    .AddTMAuthService(builder.Configuration)
    .AddCenterService(builder.Configuration);

builder.Services.AddValidation();
builder.Services.AddCarter();

//setup CORS for http://locaolhost:3000
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policyBuilder =>
        {
            policyBuilder.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
        });
});

var app = builder.Build();

app.MapDefaultEndpoints();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    await app.UseAuthMigration();
    await app.UseCenterMigration();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();

app.Run();
