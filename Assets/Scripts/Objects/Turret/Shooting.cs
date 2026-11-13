using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public AudioClip sfxShoot;

    public float repeatRate = 2.0f;

    private float timer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= repeatRate)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            timer -= repeatRate;
            if (sfxShoot != null) MusicManager.Instance.audioSource.PlayOneShot(sfxShoot, 0.375f);
        }
    }
}
