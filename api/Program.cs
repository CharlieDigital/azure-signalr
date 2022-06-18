using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR;

var builder = WebApplication.CreateBuilder(args);

var signalRConnectionString = Environment.GetEnvironmentVariable("SignalRConnectionString");

// Set up Azure SignalR
// https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-quickstart-dotnet-core
builder.Services
    .AddSignalR()
    .AddAzureSignalR(options => {
        options.Endpoints = new[] {
            new ServiceEndpoint(signalRConnectionString)
        };
    });

// Set up CORS to allow the front-end to call into these endpoints.
builder.Services.AddCors(b => {
    b.AddPolicy("policy", p =>
    {
        p.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// This must come first.
app.UseRouting();
app.UseCors("policy");

// Add authentication and authorization.
// See: https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-6.0

app.UseEndpoints(e =>
{
    // See: https://docs.microsoft.com/en-us/aspnet/core/signalr/configuration?view=aspnetcore-6.0&tabs=dotnet#advanced-http-configuration-options
    e.MapHub<NotificationHub>("/notifications");
});

app.MapGet("/test", () =>
{
    Console.WriteLine("Yup, working!");
});

app.MapGet("/echo/{message}", (IHubContext<NotificationHub, INotificationHub> hub, string message) =>
{
    hub.Clients.All.Send(message);
})
.WithName("echo");

app.Run();

/// <summary>
/// This interface allows for a strongly typed hub.
/// See: https://docs.microsoft.com/en-us/aspnet/core/signalr/hubcontext?view=aspnetcore-6.0#inject-a-strongly-typed-hubcontext
/// </summary>
public interface INotificationHub
{
    Task Send(string message);
}

/// <summary>
/// This class represents the hub.
/// </summary>
public class NotificationHub : Hub<INotificationHub>
{
    public Task Send(string message)
    {
        return Clients.All.Send(message);
    }
}