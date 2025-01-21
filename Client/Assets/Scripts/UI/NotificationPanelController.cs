using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class NotificationPanelController : MonoBehaviour
{
    public static NotificationPanelController Instance;

    [SerializeField] private Vector3 hiddenPos;
    [SerializeField] private float slideSpeed, showDuration;

    private bool _midNotification;
    private TMP_Text _textField;
    private Vector3 _shownPos;
    private RectTransform _rect;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _rect = GetComponent<RectTransform>();
        _shownPos = _rect.localPosition;
        _rect.localPosition = hiddenPos;
        _textField = GetComponentInChildren<TMP_Text>();
    }

    private async Task SlideToPos(Vector3 targetPos)
    {
        if (!Application.isPlaying)
        { return; }
        Vector3 startPos = _rect.localPosition;
        float t = 0f;
        while (t < 1f)
        {
            _rect.localPosition = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));
            t += Time.deltaTime * slideSpeed;
            await Task.Yield();
        }
        _rect.localPosition = targetPos;
    }

    public async Task ShowNotification(string text)
    {
        while (_midNotification)
        {
            await Task.Delay(100);
            if (!Application.isPlaying)
            { return; }
        }
        _textField.text = text;
        _midNotification = true;
        await SlideToPos(_shownPos);
        await Task.Delay((int)(showDuration * 1000));
        await SlideToPos(hiddenPos);
        _midNotification = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShowNotification("Something is wrong");
        }
    }
}