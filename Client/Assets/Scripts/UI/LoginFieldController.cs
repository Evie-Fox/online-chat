using UnityEngine;
using UnityEngine.UI;

public class LoginFieldController : MonoBehaviour
{
    private InputField textField;
    private Button loginButton;

    private void Awake()
    {
        textField = GetComponent<InputField>();
        loginButton = GetComponentInChildren<Button>();
    }
}
