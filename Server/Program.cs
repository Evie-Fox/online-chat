using MinimalGameServer.DataStructures;
using MinimalGameServer.Actions;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

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
                await ServerActions.DisconnectPlayer(Console.ReadLine());

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
/*
app.MapPost("/LogIn", async (Player player) =>
{
    IResult res =  await ClientActions.LogIn(player);
    return res; 
});
app.MapPost("/PostMessage", async (PlayerMessage msg) =>
{
    if (msg == null)
    {
        return Results.BadRequest("Null object sent");
    }

    await ClientActions.PostMessage(msg);
    return Results.Ok("Message received");
});
*/

var serverTask = app.RunAsync(cts.Token);
await Task.WhenAny(serverTask, commandTask);
cts.Cancel();
await serverTask;