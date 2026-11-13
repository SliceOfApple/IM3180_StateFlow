using UnityEngine;

public class ShieldHealth : MonoBehaviour
{
    public int health;
    public GameObject explode;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Instantiate(explode, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
