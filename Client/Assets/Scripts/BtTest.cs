using UnityEngine;
using UnityEngine.EventSystems;

public class BtTest : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        PostMsg();
    }

    public void PostMsg()
    {
        print("TestThis: " + gameObject.name);
    }
}
