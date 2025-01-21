using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginButtonController : MonoBehaviour, IPointerClickHandler
{
    private LoginFieldController _loginFieldController;


    private const string LOGINTEXT = "Log in", LOGOUTTEXT = "Log out";
    private TMP_InputField _inputField;
    private TMP_Text _buttonText;
    private Button _button;
    
    
    private void Awake()
    {
        _buttonText = GetComponentInChildren<TMP_Text>();
        _button = GetComponent<Button>();
        _buttonText.text = LOGINTEXT;

        _loginFieldController = GetComponentInParent<LoginFieldController>();
        _inputField = _loginFieldController.GetComponent<TMP_InputField>();
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        _button.interactable = false;
        if (NetworkManager.Instance.LoggedIn)
        {
            await NetworkManager.Instance.LogOut(GameManager.Player);
            _buttonText.text = LOGINTEXT;
            _button.interactable = true;
            _inputField.interactable = true;
            return;
        }
        GameManager.Player = new(_inputField.text.Trim());

        if (string.IsNullOrEmpty(GameManager.Player.Name))
        {
            NotificationPanelController.Instance.ShowNotification("Invalid name");
            _button.interactable = true;
            return;
        }

        await NetworkManager.Instance.LogIn(GameManager.Player);

        int waitAmmout = 100;

        while (!NetworkManager.Instance.LoggedIn && waitAmmout > 0)
        {
            await Task.Delay(25);
            waitAmmout--;
        }
        _button.interactable = true;
        
        if (NetworkManager.Instance.LoggedIn)
        {
            _inputField.interactable = false;
            _buttonText.text = LOGOUTTEXT;
            print("Logged in as: " + _inputField.text);
            return;
        }
        print("Login failed");
    }

}
