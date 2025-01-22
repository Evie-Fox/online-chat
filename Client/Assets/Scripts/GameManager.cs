using UnityEngine;
using static StaticLibrary;

public class GameManager : MonoBehaviour
{
    public static Player Player;
    public OnlinePlayersListController PlayersList;

    private MainTextPanelController _mainTextPanel;
    private NotificationPanelController _notificationPanel;

    private void Awake()
    {
        _mainTextPanel = FindFirstObjectByType<MainTextPanelController>();
        PlayersList = FindFirstObjectByType<OnlinePlayersListController>();
        _notificationPanel = FindFirstObjectByType<NotificationPanelController>();
    }

    public void ClearPlayersList()
    {
        string[] empty = {string.Empty};
        PlayersList.SetOnlinePlayersList(empty);
    }
    public void WriteMessageOnBoard(PlayerMessage message)
    {
        _mainTextPanel.WriteMessage(message);
    }
}
