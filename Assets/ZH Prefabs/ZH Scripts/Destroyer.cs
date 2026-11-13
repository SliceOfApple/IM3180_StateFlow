using UnityEngine;
using UnityEngine.EventSystems;

public class Destroyer : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GameObject DestroyerPrefab;
    private GameObject activeTurret;
    public Color Color1 = Color.red;
    public Color Color2 = Color.blue;
    public Color TurretColor;

    public void OnPointerDown(PointerEventData eventData)
    {
        {
            Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            spawnPosition.z = 0f;
            activeTurret = Instantiate(DestroyerPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (activeTurret != null)
        {
            Vector3 dragPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            dragPosition.z = 0f;
            activeTurret.transform.position = dragPosition;
            TurretColor = Color.purple;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            mousePosition.z = 0f;
            Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
            Collider2D[] hitColliders = Physics2D.OverlapPointAll(mousePosition2D);
            foreach (Collider2D hit in hitColliders)
            {
                if (hit.CompareTag("TurretPlaced"))
                {
                    TurretColor = Color.red;
                }
            }
            TurretColor.a = 0.7f;
            activeTurret.GetComponent<SpriteRenderer>().color = TurretColor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (activeTurret != null)
        {
            Destroying turretScript = activeTurret.GetComponent<Destroying>();
            if (turretScript != null)
            {
                turretScript.PlacementComplete(eventData);
            }
            activeTurret = null;
        }
    }
}
