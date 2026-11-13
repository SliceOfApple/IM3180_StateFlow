using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public float BulletSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * BulletSpeed * Time.deltaTime);
    }
}
