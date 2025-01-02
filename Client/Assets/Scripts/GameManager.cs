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

    private void Awake()
    {
        PLAYER = player;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && NetworkManager.Instance.ws.State == System.Net.WebSockets.WebSocketState.Open)
        {
            print("Sending message");
            NetworkManager.Instance.PostMessage("LOL gadol");
        }
        if (Input.GetKeyDown(KeyCode.L) && NetworkManager.Instance.ws.State == System.Net.WebSockets.WebSocketState.Open)
        {
            print("Logging in");
            NetworkManager.Instance.LogIn(PLAYER);
        }
    }
}
