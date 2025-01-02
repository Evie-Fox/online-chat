using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoginFieldController : MonoBehaviour, IPointerDownHandler
{
    private InputField textField;
    private UnityEngine.UI.Button loginButton;

    private void Awake()
    {
        textField = GetComponent<InputField>();
        loginButton = GetComponentInChildren<UnityEngine.UI.Button>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (textField != null)
        {
            print("something is entered");
        }
    }
}
