using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BtTest : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        PostMsg();
    }

    public void PostMsg()
    {
        print("TestThis"!);
    }
}
