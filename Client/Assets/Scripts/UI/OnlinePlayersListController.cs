using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class OnlinePlayersListController : MonoBehaviour
{
    [SerializeField] private int msUpdateDelay;

    private TMP_Text _textBox;

    private void Awake()
    {
        _textBox = GetComponent<TMP_Text>();
        if (msUpdateDelay < 1)
        {
            throw new UnityException("Online players update delay is too low");
        }
        CheckForOnlinePlayersLoop();
    }
    private async void CheckForOnlinePlayersLoop()
    {
        while (!Application.exitCancellationToken.IsCancellationRequested)
        {
            await Task.Delay(msUpdateDelay);
            _textBox.text = await NetworkManager.Instance.GetPlayerOnlineString();
        }
    }
}
