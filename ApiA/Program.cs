using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration, "AzureAdB2C")
    .EnableTokenAcquisitionToCallDownstreamApi(options => builder.Configuration.Bind("AzureAdB2C", options))
    .AddInMemoryTokenCaches();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("/hello-a", [AllowAnonymous]
async (IConfiguration configuration, ITokenAcquisition tokenAcquisition) =>
{
    var scope = configuration["ApiB:Scope"]!;
    var accessToken = await tokenAcquisition.GetAccessTokenForAppAsync(scope);
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:6001")
    };
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var response = await httpClient.GetAsync("hello-b");

    if (!response.IsSuccessStatusCode)
        return Results.Problem($"Failed to call ApiB! Status Code{response.StatusCode}");

    return Results.Ok("Successfully called ApiB!");
})
.WithOpenApi();

app.Run();
