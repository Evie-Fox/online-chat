using System;
using System.Linq;
using System.Text;
using UnityEngine;
using static StaticLibrary;

public class GameManager : MonoBehaviour
{
    public Player player;
    public static Player PLAYER;
    public string TestingText;
    public OnlinePlayersListController PlayersList;

    private MainTextPanelController _mainTextPanel;

    private void Awake()
    {
        PLAYER = player;
        _mainTextPanel = FindFirstObjectByType<MainTextPanelController>();
        PlayersList = FindFirstObjectByType<OnlinePlayersListController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && NetworkManager.Instance.ws.State == System.Net.WebSockets.WebSocketState.Open)
        {
            print("Sending message");
            NetworkManager.Instance.PostMessage(TestingText);
        }
        if (Input.GetKeyDown(KeyCode.L) && NetworkManager.Instance.ws.State == System.Net.WebSockets.WebSocketState.Open)
        {
            print("Logging in");
            NetworkManager.Instance.LogIn(PLAYER);
        }
        if (Input.GetKeyDown(KeyCode.O) && NetworkManager.Instance.ws.State == System.Net.WebSockets.WebSocketState.Open)
        {
            print("Logging out");
            NetworkManager.Instance.LogOut(PLAYER);
        }
    }
    public void WriteMessageOnBoard(PlayerMessage message)
    {
        _mainTextPanel.WriteMessage(message);
    }
}
