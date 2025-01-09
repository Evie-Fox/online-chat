using System;
using System.Linq;
using System.Text;
using UnityEngine;
using static StaticLibrary;

public class GameManager : MonoBehaviour
{
    public Player player;
    public static Player Player;
    public string TestingText;
    public OnlinePlayersListController PlayersList;

    private MainTextPanelController _mainTextPanel;

    private void Awake()
    {
        Player = player;
        _mainTextPanel = FindFirstObjectByType<MainTextPanelController>();
        PlayersList = FindFirstObjectByType<OnlinePlayersListController>();
    }

    public void WriteMessageOnBoard(PlayerMessage message)
    {
        _mainTextPanel.WriteMessage(message);
    }
}
