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

    public bool LoggedIn = false;
    public ClientWebSocket ws;

    private UnityWebRequest mainRequest;
    private const string URL = "https://localhost:7109";
    private Uri uri;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        Init();
    }

    private async void Init()
    {
        ws = new ClientWebSocket();
        uri = new Uri("wss://localhost:7109/ws");
        try
        {
            await ws.ConnectAsync(uri, CancellationToken.None);
            Console.WriteLine("Connected to websocket");
            WebSocketHandler(ws);
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
        byte bufferByteSize = 4;
        byte[] buffer = new byte[1024 * bufferByteSize];

        while (ws.State == WebSocketState.Open)
        {
            //WebSocketReceiveResult results = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
            WebSocketReceiveResult results = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);

            ServerRequest req;

            if (results.MessageType == WebSocketMessageType.Close)
            {
                print("WebSocket closed by the server");
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing from server request", ct);
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
        byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req));
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async void TestEcho()
    {
        /*
        UnityWebRequest req = CreateRequest(URL + "/Echo", RequestType.GET);

        await req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            print($"Sonething went wrong: {req.result} ({req.error})");
        }
        print(req.downloadHandler.text);
        */
    }

    public async Task LogIn(Player player)
    {
        /*
        UnityWebRequest req = CreateRequest(URL + "/LogIn", RequestType.POST, player);
        await req.SendWebRequest();

        if (req.error == null)
        {
            LoggedIn = true;
            return true;
        }
        print("failed login request: " + req.downloadHandler.text);
        return false;
        */
        ClientRequest req = new(ClientRequestType.Login, player, null);
        await SendWS(req);
    }

    public async Task PostMessage(string text = "This is a test msg")
    {
        /*
        UnityWebRequest req = CreateRequest("https://localhost:7109/PostMessage", RequestType.POST, msg);
        await req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            print($"Sonething went wrong: {req.result} ({req.error})");
            return null;
        }
        print(req.downloadHandler.text.ToString());
        return null;
        */
        PlayerMessage msg = new(Time.time, text);
        ClientRequest req = new(ClientRequestType.Message, GameManager.PLAYER, JsonConvert.SerializeObject(msg));
        await SendWS(req);
    }

    public async Task<string[]> GetPlayersOnline()
    {
        UnityWebRequest req = CreateRequest(URL + "/PlayersOnline", RequestType.GET);
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

    public UnityWebRequest CreateRequest(string url, RequestType type, object content = null)
    {
        UnityWebRequest req = new UnityWebRequest(url, type.ToString());

        req.downloadHandler = new DownloadHandlerBuffer();

        if (content != null)
        {
            string json = JsonUtility.ToJson(content);
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        }
        req.SetRequestHeader("Content-Type", "application/json");
        return req;
    }

    public enum RequestType
    {
        GET = 0,
        POST = 1
    }

    private async void TestRequest(object data = null)
    {
        UnityWebRequest req;
        req = UnityWebRequest.Post(URL, JsonUtility.ToJson(new PlayerMessage(0, "Msg to mock worked")), "application/json");
        await req.SendWebRequest();
        PlayerMessage msg = JsonUtility.FromJson<PlayerMessage>(req.downloadHandler.text);
        print(msg.Content);
    }

    //PlayerMessage msg = new() {Content = "Yea, I said it!", Id = 22}; LOOK UP MORE ON THIS

    private async void OnApplicationQuit()
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
        }
    }
}