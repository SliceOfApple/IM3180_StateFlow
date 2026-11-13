using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class SelectTile : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.gameObject.CompareTag("TurretPlaced"))
        {
            if (this.transform.childCount != 0)
            {
                SetSelectedTurret.Instance.SelectTurret(this.transform.GetChild(0).gameObject, this.gameObject);
            }
        }
        else if (!this.gameObject.CompareTag("Pause"))
        {
            SetSelectedTurret.Instance.DeselectTurret();
        }
    }
}
