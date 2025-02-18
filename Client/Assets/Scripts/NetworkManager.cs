using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using static StaticLibrary;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    private GameManager _gameMan;

    public bool LoggedIn = false;
    public ClientWebSocket ws;

    [SerializeField] private bool DEBUG = false;

    private Uri uri;
    private const string ADDRESSSSTART = "wss://", ADDRESSEND = "/ws";


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        Init();
    }

    private async void Init()
    {
        _gameMan = GetComponent<GameManager>();
    }

    public async Task WebSocketHandler(WebSocket ws)
    {
        await WebSocketHandler(ws, CancellationToken.None);
    }

    public async Task WebSocketHandler(WebSocket ws, CancellationToken ct)
    {
        byte bufferKByteSize = 4;
        byte[] buffer = new byte[1024 * bufferKByteSize];

        while (ws.State == WebSocketState.Open)
        {
            Array.Clear(buffer, 0, 1024 * bufferKByteSize);
            WebSocketReceiveResult results = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);

            ServerRequest req;

            if (results.MessageType == WebSocketMessageType.Close)
            {
                print("WebSocket closed by the server");
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing from server request", ct);
                LoggedIn = false;
                return;
            }

            if (results.MessageType == WebSocketMessageType.Text && results.Count > 0)
            {
                string msg = Encoding.UTF8.GetString(buffer, 0, results.Count);
                try
                {
                    req = JsonConvert.DeserializeObject<ServerRequest>(msg);
                }
                catch (Exception)
                {
                    print("Error: Invalid ServerRequest Json");
                    continue;
                }
                switch (req.RequestType)
                {
                    case ServerRequestType.Ok:
                        print("Server: OK");
                        break;
                    case ServerRequestType.Error:
                        print($"Server: ERROR, {req.Content}");
                        NotificationPanelController.Instance.ShowNotification($"Server: ERROR, {req.Content}");
                        break;
                    case ServerRequestType.NewMessage:
                        _gameMan.WriteMessageOnBoard(JsonConvert.DeserializeObject<PlayerMessage>(JsonConvert.SerializeObject(req.Content)));
                        break;
                    case ServerRequestType.PlayerList:
                        _gameMan.PlayersList.SetOnlinePlayersList(JsonConvert.DeserializeObject<PlayerNames>(JsonConvert.SerializeObject(req.Content)).Names);
                        break;
                    case ServerRequestType.LogToggle:
                        LoggedIn = !LoggedIn;
                        break;

                    default:
                        print("Something went wrong with the ServerRequest");
                        break;
                }
            }
        }
    }

    public async Task SendWS(ClientRequest req)
    {
        if (ws.State != WebSocketState.Open)
        {
            NotificationPanelController.Instance.ShowNotification("Not connected");
            return;
        }

        string json = JsonConvert.SerializeObject(req);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        if (DEBUG)
        {
            print(" \n" + json + "\n" + Encoding.UTF8.GetString(bytes));
        }
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task LogIn(Player player)
    {
        ClientRequest req = new(ClientRequestType.Login, player, null);
        await SendWS(req);
    }

    public async Task LogOut(Player player)
    {
        ClientRequest req = new(ClientRequestType.Logout, player, null);
        await SendWS(req);
        _gameMan.ClearPlayersList();
        return;

    }
    public async Task Connect(string address)
    {
        uri = new Uri(ADDRESSSSTART + address + ADDRESSEND);
        ws = new ClientWebSocket();
        try
        {
            await ws.ConnectAsync(uri, CancellationToken.None);
            print("Connected to websocket");
            WebSocketHandler(ws);
        }
        catch (WebSocketException e)
        {
            Console.WriteLine(e.Message);
            return;
        }
        return;

    }
    public async Task<bool> Disconnect()
    {
        if (ws.State != WebSocketState.Open)
        {
            print("Web socket is not open");
            return false;
        }

        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Normal closure", CancellationToken.None);

        if (ws.State == WebSocketState.Open)
        {
            print("Unsuccessful disconnection");
            return false;
        }
        return true;

    }

    public async Task PostMessage(string text = "This is a test msg")
    {
        PlayerMessage msg = new(GameManager.Player,DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), text);
        ClientRequest req = new(ClientRequestType.Message, GameManager.Player, msg);
        await SendWS(req);
    }
    
    private async void OnApplicationQuit()
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
        }
    }
}