using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Placing : MonoBehaviour
{
    public GameObject TurretPrefab;
    public int cost = 100;
    public void PlacementComplete(PointerEventData eventData)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePosition.z = 0f;
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
        Collider2D[] hitColliders = Physics2D.OverlapPointAll(mousePosition2D);
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Place"))
            {
                hit.tag = "TurretPlaced";
                int newLayer = LayerMask.NameToLayer("TileFilled");
                hit.gameObject.layer = newLayer;
                GameObject Turret = Instantiate(TurretPrefab, hit.transform);
                SetSelectedTurret.Instance.SelectTurret(Turret, hit.gameObject);
                SetCurrency.Instance.ChangeCurrency(-1*cost);
                if (TutorialManager.Instance != null)
                {
                    if (TutorialManager.Instance.TurretTut == true)
                    {
                        TutorialManager.Instance.TurretTut = false;
                        TutorialManager.Instance.StartCutscene();
                        int number = int.Parse(Regex.Match(hit.name, @"\d+").Value);
                        SpawnAlien.Instance.SpawnSet((number%4)-1);
                    }
                }
                break;
            }
            else continue;
        }
        Destroy(this.gameObject);
    }
}
