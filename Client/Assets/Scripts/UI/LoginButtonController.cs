using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static StaticLibrary;

public class LoginButtonController : MonoBehaviour, IPointerClickHandler
{
    private LoginFieldController _loginFieldController;


    private const string LOGINTEXT = "Log in", LOGOUTTEXT = "Log out";
    private TMP_InputField _inputField;
    private TMP_Text _buttonText;
    
    
    private void Awake()
    {
        _buttonText = GetComponentInChildren<TMP_Text>();
        _buttonText.text = LOGINTEXT;

        _loginFieldController = GetComponentInParent<LoginFieldController>();
        _inputField = _loginFieldController.GetComponent<TMP_InputField>();
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        if (NetworkManager.Instance.LoggedIn)
        {
            print("LOG OUT!!!");
            await NetworkManager.Instance.LogOut(GameManager.PLAYER);
            /*DELETE THIS LATER!!!!*///NetworkManager.Instance.LoggedIn = false;/*FOR REAL!*/
            _buttonText.text = LOGINTEXT;
            _inputField.interactable = true;
            return;
        }
        GameManager.PLAYER = new(_inputField.text.Trim());
        print(GameManager.PLAYER.Id);
        await NetworkManager.Instance.LogIn(GameManager.PLAYER);
        //print(_inputField.text);
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
