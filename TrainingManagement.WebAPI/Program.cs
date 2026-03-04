using Carter;
using Scalar.AspNetCore;
using TrainingManagement.Auth.Commons.Extensions;
using TrainingManagement.WebAPI.Commons.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddTMAuthService(builder.Configuration);

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
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();

app.Run();
