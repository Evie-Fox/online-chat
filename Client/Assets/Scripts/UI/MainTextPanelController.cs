using TMPro;
using UnityEngine;
using static StaticLibrary;

public class MainTextPanelController : MonoBehaviour
{
    private TMP_Text _textBox;
    private bool _isFirstMessage = true;

    private void Awake()
    {
        _textBox = GetComponentInChildren<TMP_Text>();
    }
    public void WriteMessage(PlayerMessage message)
    {
        if (_isFirstMessage) 
        {
            _textBox.text = string.Empty; 
            _isFirstMessage = false;
        }
        _textBox.text += ($"\n{message.TimeSent.ToString("F2")},{message.Author}: {message.Content}");
    }
}
