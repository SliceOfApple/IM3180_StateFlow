using UnityEngine;
using UnityEngine.EventSystems;

public class ClickCheck : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {

        SetSelectedTurret.Instance.DeselectTurret();
        Debug.Log("Screen Click Started at: " + eventData.position);
    }
    
}
