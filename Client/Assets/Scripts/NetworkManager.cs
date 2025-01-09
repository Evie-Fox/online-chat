using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static StaticLibrary;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    private GameManager _gameMan;

    public bool LoggedIn = false;
    public ClientWebSocket ws;

    [SerializeField] private bool DEBUG = false;

    private const string URL = "wss://localhost:7109/ws";
    private Uri uri;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        Init();
    }

    private async void Init()
    {
        _gameMan = GetComponent<GameManager>();
        ws = new ClientWebSocket();
        uri = new Uri(URL);
        try
        {
            await ws.ConnectAsync(uri, CancellationToken.None);
            print("Connected to websocket");
            await WebSocketHandler(ws);
        }
        catch (WebSocketException e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
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

    public async Task SendWS(string contents)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(contents);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task SendWS(ClientRequest req)
    {
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
        return;

        //TODO: proper disconnection
        int secondsToWait = 5;
        while (ws.State != WebSocketState.Closed && secondsToWait > 0)
        {
            await Task.Delay(1000);
            secondsToWait--;
        }

        if (ws.State == WebSocketState.Open)
        {
            print("Unsuccessful logout");
            return;
        }
    }

    public async Task PostMessage(string text = "This is a test msg")
    {
        PlayerMessage msg = new(GameManager.Player,Time.time, text);
        ClientRequest req = new(ClientRequestType.Message, GameManager.Player, msg);
        await SendWS(req);
    }
    
    public async Task<string[]> GetPlayersOnline()
    {
        UnityWebRequest req = new(); 
        await req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success)
        {
            return new string[] { "Dummy list:", "Player 1", "Player 2", "Player 3" };
        }
        PlayerNames names = JsonUtility.FromJson<PlayerNames>(req.downloadHandler.text);
        return names.Names;
    }

    public async Task<string> GetPlayerOnlineString()
    {
        string[] names = await GetPlayersOnline();
        return "Players online:\n" + String.Join(" \n", names);
    }

    private async void OnApplicationQuit()
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
        }
    }
}