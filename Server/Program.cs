using MinimalGameServer.DataStructures;
using MinimalGameServer.Actions;
using Newtonsoft.Json;
using System.Net.WebSockets;
using MinimalGameServer;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseWebSockets();

CancellationTokenSource cts = new();


var commandTask = Task.Run(async () =>
{
    while (!cts.Token.IsCancellationRequested)
    {
        Console.Write(">");
        

        string command = Console.ReadLine().Trim().ToLower();

        switch (command)
        {
            case ("clear"):
                Console.Clear();
                break;

            case ("stop"):
            case ("shutdown"):
                Console.WriteLine("Shutting down server...");
                cts.Cancel();
                break;

            case ("list"):
            case ("players"):
            case ("listplayers"):
                Console.WriteLine(await ServerActions.ListPlayers());
                break;

            case ("dummy"):
            case ("adddummy"):
                Console.WriteLine("Dummy name: ");
                await ClientActions.LogIn(new(new(Console.ReadLine())));
                break;

            case ("kick"):
            case ("removeplayer"):
            case ("disconnectplayer"):
                Console.WriteLine("Player's ID: ");
                if (ServerData.PlayerDict.TryGetValue(Console.ReadLine(), out ClientPlayer client))
                {
                    await ServerActions.DisconnectPlayer(client);
                    break;
                }
                Console.WriteLine("Player ID not found");

                break;

            default:
                Console.WriteLine("Unknown command.");
                break;
        }
        await Task.Yield();
    }
});


app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        WebSocket ws = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("WebSocket connected");
        await ServerActions.WebSocketHandler(ws);
        Console.WriteLine("WebSocket closed");
        return;
    }
    context.Response.StatusCode = 400;
    Console.WriteLine("Bad web socket connection request");
});

app.MapGet("/", () => "Server is running.");

app.MapGet("/Echo", () => "All is fine on the backend");

app.MapGet("/PlayersOnline", async () =>
{
    string json = JsonConvert.SerializeObject(await ClientActions.PlayersOnline());
    return json;
});

var serverTask = app.RunAsync(cts.Token);
await Task.WhenAny(serverTask, commandTask);
cts.Cancel();
await serverTask;