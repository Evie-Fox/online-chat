using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ServerAddressController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool setDefaultAddress = false;

    private string _defaultAddress = "localhost:7109";
    private const string CONNECTTEXT = "Connect", DISCONNECTTEXT = "Disconnect";
    private TMP_InputField _inputField;
    private TMP_Text _buttonText;
    private Button _button;

    private void Awake()
    {
        _inputField = GetComponentInParent<TMP_InputField>();
        _buttonText = GetComponentInChildren<TMP_Text>();
        _button = GetComponent<Button>();
        _buttonText.text = CONNECTTEXT;
    }

    private async void Start()
    {
        if (setDefaultAddress)
        {
            _button.interactable = false;
            _inputField.text = _defaultAddress;
            NetworkManager.Instance.Connect(_defaultAddress);

            int quarterSecondsToWait = 16;

            while (NetworkManager.Instance.ws.State != System.Net.WebSockets.WebSocketState.Open && quarterSecondsToWait > 0)
            {
                await Task.Delay(250);
                quarterSecondsToWait--;
            }
            if (NetworkManager.Instance.ws.State == System.Net.WebSockets.WebSocketState.Open)
            {
                _buttonText.text = DISCONNECTTEXT;
            }
            _button.interactable = true;
        }
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        _button.interactable = false;
        if (NetworkManager.Instance.ws.State == System.Net.WebSockets.WebSocketState.Open)
        {
            if (NetworkManager.Instance.LoggedIn)
            {
                await NetworkManager.Instance.LogOut(GameManager.Player);
            }
            if (await NetworkManager.Instance.Disconnect())
            {
                _buttonText.text = CONNECTTEXT;
            }
            _button.interactable = true;
            return;
        }

        NetworkManager.Instance.Connect(_inputField.text);

        int quarterSecondsToWait = 16;

        while (NetworkManager.Instance.ws.State != System.Net.WebSockets.WebSocketState.Open && quarterSecondsToWait > 0)
        {
            await Task.Delay(250);
            quarterSecondsToWait--;
        }
        if (NetworkManager.Instance.ws.State == System.Net.WebSockets.WebSocketState.Open)
        {
            _buttonText.text = DISCONNECTTEXT;
        }
        _button.interactable = true;
    }
}