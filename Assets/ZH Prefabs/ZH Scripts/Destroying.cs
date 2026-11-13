using UnityEngine;
using UnityEngine.EventSystems;

public class Destroying : MonoBehaviour
{
    public void PlacementComplete(PointerEventData eventData)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePosition.z = 0f;
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
        Collider2D[] hitColliders = Physics2D.OverlapPointAll(mousePosition2D);
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("TurretPlaced"))
            {
                hit.transform.GetComponentInChildren<Turret>().TakeDamageNoAni(99999);
                break;
            }

            else continue;
        }
        Destroy(this.gameObject);
    }
}
