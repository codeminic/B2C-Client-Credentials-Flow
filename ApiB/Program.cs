using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration, "AzureAdB2C");

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/hello-b", () =>
{
    return Results.Ok(new { Message = "Hello from ApiB" });
});

app.Run();
