using UnityEngine;

public class TurretHealth : MonoBehaviour
{
    public int health;
    public GameObject explode;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void TakeDamage(int  damage)
    {
        health -= damage;
        animator.SetTrigger("Taking Damage");
        if (health <= 0)
        {
            if (this.gameObject.transform.parent != null)
            {
                GameObject Tile = this.gameObject.transform.parent.gameObject;
                Tile.tag = "Place";
                int newLayer = LayerMask.NameToLayer("Tile");
                Tile.gameObject.layer = newLayer;
            }
            
            Instantiate(explode, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
