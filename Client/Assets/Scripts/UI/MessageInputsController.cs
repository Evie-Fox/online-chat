using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessageInputsController : MonoBehaviour, IPointerDownHandler
{
    private Button _button;
    private TMP_InputField _inputField;
    private int _msSendMessageDelay = 550;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _inputField = GetComponentInParent<TMP_InputField>();
    }


    public async void OnPointerDown(PointerEventData eventData)
    {
        if (!NetworkManager.Instance.LoggedIn)
        {
            print("Not logged in");
            return;
        }
        if (string.IsNullOrWhiteSpace(_inputField.text))
        {
            print("Invalid message");
            return;
        }

        _button.interactable = false;
        _inputField.interactable = false;
        await NetworkManager.Instance.PostMessage(_inputField.text);
        _inputField.text = string.Empty;
        await Task.Delay(_msSendMessageDelay);
        _button.interactable = true;
        _inputField.interactable = true;
    }
}
