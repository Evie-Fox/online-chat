using System.Net.WebSockets;
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
            await ConnectToDefaultAddress();
        }
    }
    public async void OnPointerClick(PointerEventData eventData)
    {
        await ConnectToggle();
    }



    private async Task ConnectToggle()
    {
        _button.interactable = false;
        if (NetworkManager.Instance.ws != null && NetworkManager.Instance.ws.State == System.Net.WebSockets.WebSocketState.Open)
        {
            if (NetworkManager.Instance.LoggedIn)
            {
                await NetworkManager.Instance.LogOut(GameManager.Player);
            }
            if (await NetworkManager.Instance.Disconnect())
            {
                _buttonText.text = CONNECTTEXT;
            }
            _inputField.interactable = true;
            _button.interactable = true;
            return;
        }

        if (string.IsNullOrEmpty(_inputField.text))
        {
            NotificationPanelController.Instance.ShowNotification("Invalid server address");
            _button.interactable = true;
            return;
        }


        await NetworkManager.Instance.Connect(_inputField.text);
        
        if (NetworkManager.Instance.ws.State == System.Net.WebSockets.WebSocketState.Open)
        {
            _buttonText.text = DISCONNECTTEXT;
            _inputField.interactable = false;
        }
        else
        {
            NotificationPanelController.Instance.ShowNotification("Server not found");
        }
        _button.interactable = true;
    }
    private async Task ConnectToDefaultAddress()
    {
        _button.interactable = false;
        _inputField.text = _defaultAddress;
        await NetworkManager.Instance.Connect(_defaultAddress);

        if (NetworkManager.Instance.ws.State == WebSocketState.Open)
        {
            _buttonText.text = DISCONNECTTEXT;
        }

        _button.interactable = true;
    }
}