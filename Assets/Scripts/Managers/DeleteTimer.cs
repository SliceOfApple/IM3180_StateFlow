using UnityEngine;

public class DeleteTimer : MonoBehaviour
{
    public float deletetime = 3.0f;

    private float timer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= deletetime)
        {
            Destroy(this.gameObject);
        }
    }
}
