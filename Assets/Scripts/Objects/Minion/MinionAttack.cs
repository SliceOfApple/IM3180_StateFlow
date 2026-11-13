using UnityEngine;

public class MinionAttack : MonoBehaviour
{
    public int Damage;
    public int PlayerDamage;
    private float timer;
    public float AttackDelay;
    public float MovementSpeed = 5f;
    public Rigidbody2D rb;
    private void OnCollisionEnter2D(Collision2D hit)
    {
        if (hit.gameObject.CompareTag("Turret"))
        {
            timer = 0.0f;
        }
    }
    private void OnCollisionStay2D(Collision2D hit)
    {
        if (hit.gameObject.CompareTag("Turret"))
        {
            timer += Time.deltaTime;
            if (timer >= AttackDelay)
            {
                hit.gameObject.GetComponent<Turret>().TakeDamage(Damage);
                timer = 0.0f;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        this.gameObject.GetComponent<MinionMovement>().Move();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Place"))
        {
            collision.tag = "AlienPlaced";
        }
        if (collision.gameObject.CompareTag("Hurtbox"))
        {
            SetPlayerHealth.Instance.TakeDamage(PlayerDamage);
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("AlienPlaced"))
        {
            collision.tag = "Place";
        }
    }
}
