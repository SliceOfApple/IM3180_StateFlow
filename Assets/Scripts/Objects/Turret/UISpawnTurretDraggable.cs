using UnityEngine;
using UnityEngine.EventSystems;

public class UISpawnTurretDraggable : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GameObject TurretPrefab;
    private GameObject activeTurret;
    public int cost = 100;
    public Color Color1 = Color.red;
    public Color Color2 = Color.blue;
    public Color TurretColor;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (SetCurrency.Instance.CheckCurrency() >= cost)
        {
            Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            spawnPosition.z = 0f;
            activeTurret = Instantiate(TurretPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (activeTurret != null)
        {
            Vector3 dragPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            dragPosition.z = 0f;
            activeTurret.transform.position = dragPosition;
            TurretColor = Color.red;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            mousePosition.z = 0f;
            Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
            Collider2D[] hitColliders = Physics2D.OverlapPointAll(mousePosition2D);
            foreach (Collider2D hit in hitColliders)
            {
                if (hit.CompareTag("Place"))
                {
                    TurretColor = Color.blue;
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
            Placing turretScript = activeTurret.GetComponent<Placing>();
            if (turretScript != null)
            {
                turretScript.PlacementComplete(eventData);
            }
            activeTurret = null;
        }
    }
}
