using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public int MDamage;
    public int BDamage;
    public bool hasnothit = true;
    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.gameObject.CompareTag("Minion") && hasnothit)
        {
            hit.gameObject.GetComponent<MinionHealth>().TakeDamage(MDamage);
            Destroy(gameObject);
            hasnothit = false;
        }

        if (hit.gameObject.CompareTag("Boss") && hasnothit)
        {
            Boss.Instance.TakeDamage(BDamage);
            Destroy(gameObject);
            hasnothit = false;
        }
  
    }
}
