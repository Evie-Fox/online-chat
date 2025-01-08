using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class OnlinePlayersListController : MonoBehaviour
{
    private TMP_Text _textBox;

    private void Awake()
    {
        _textBox = GetComponent<TMP_Text>();
    }
    
    public void SetOnlinePlayersList(string[] onlinePlayers)
    {
        _textBox.text = string.Join("\n",onlinePlayers);
    }
}
