using UnityEngine;

public class MinionMovement : MonoBehaviour
{
    public float MovementSpeed = 5f;
    Rigidbody2D rb;
    public Transform Spawnpoint;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(-Spawnpoint.up * MovementSpeed, ForceMode2D.Impulse);
}
    public void Move()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(-Spawnpoint.up * MovementSpeed, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(-Spawnpoint.up * .000001f, ForceMode2D.Impulse);
    }
}
