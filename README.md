# Using Azure SignalR

This codebase demonstrates connection with Azure SignalR.

If you'd like to see a more complete implementation including authentication and authorization, [check out my dn6-mongo-react-valtio repo](https://github.com/CharlieDigital/dn6-mongo-react-valtio/tree/azure-signalr) which has a project that includes:

- .NET 6 Web api
- MongoDB backend
- React frontend
- Valtio state management
- Cognito authentication
- SignalR real-time integration

The key client side implementation is in the file `web/src/SignalRclient.ts` which is invoked in `web/src/main.ts`

## Running the Code

Create a free tier Azure SignalR service and be sure to set the mode to **Default** and not **Serverless**.

To run the backend:

```
cd api
export SignalRConnectionString="Endpoint=https://*****/.service.signalr.net;AccessKey=***********;Version=1.0;"
dotnet run
```

To run the frontend:

```
cd web
yarn
yarn dev
```

To test the SignalR functionality:

```
curl http://localhost:5092/echo/howdy
```

This will send the message "howdy" to connected clients in the console.

## Key Resources

SignalR documentation is really scattered; there are two key links that cleared up some weird behaviors for me:

I had been trying to implement the `negotiate` endpoint until I came across [this issue](https://github.com/dotnet/aspnetcore/issues/14979).  The reason is that in Azure Functions, it is required to implement this endpoint since there is no middleware available.  However, in ASP.NET Core, this is optional and may be an easy way to do authentication prior to handing over the connection token to Azure SignalR.

[A separate discussion on the SignalR repo](https://github.com/Azure/azure-signalr/issues/14) indicated that you do not need to `UseSignalR`.

## It Is Possible to Use Azure SignalR with Node?

I had thought this not possible, but that may not be the case and it may be possible to implement it [using the REST API](https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-quickstart-rest-api).

## Implementing `/negotiate`

It is possible to implement custom negotiation of the connection using the following code:

```csharp
app.MapPost("/negotiate", () =>
{
    Console.WriteLine($"Negotiating...");

    // Access key comes from the connection string
    var accessKey = "YOUR_ACCESS_KEY";
    var aud = "https://{YOUR_DOMAIN}.signalr.net/client/?hub={YOUR_HUB_NAME}";
    var exp = System.DateTimeOffset.UtcNow.AddDays(5).ToUnixTimeSeconds();

    var token = JwtBuilder.Create()
        .WithAlgorithm(new HMACSHA256Algorithm())
        .WithSecret(accessKey)
        .AddClaim("exp", exp)
        .AddClaim("aud", aud)
        .Encode();

    return new
    {
        url = "https://{YOUR_DOMAIN}.signalr.net/client/?hub={YOUR_HUB_NAME}",
        accessToken = token
    };
})
.WithName("negotiate");
```

I leave this here because it took forever for me to find it!

One reason to do this is to implement custom authorization.  It is possible to use `RequireAuthorizatio()` on the endpoint.

However, in some cases it may be simpler to just do it manually.

The specifics of this are available at these two links:

- [Authenticating with Azure SignalR using an AccessKey](https://github.com/Azure/azure-signalr/blob/dev/docs/rest-api.md#authenticate-via-azure-signalr-service-accesskey)
- [The SignalR Transport Protocol](https://github.com/aspnet/SignalR/blob/release/2.2/specs/TransportProtocols.md)