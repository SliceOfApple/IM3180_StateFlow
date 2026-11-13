using UnityEngine;

public class Shielding : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public AudioClip sfxShieldActivated;

    // Delay until shield is active.
    public float repeatRate = 2.0f;
    public float timer = 8.0f;

    public GameObject shieldInstance;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.childCount==0)
        {
            timer += Time.deltaTime;
            if (timer >= repeatRate)
            {
                shieldInstance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, gameObject.transform);
                if (sfxShieldActivated != null) MusicManager.Instance.audioSource.PlayOneShot(sfxShieldActivated, 0.75f);
                timer = 0;
            }
        }
    }

    // For the combo.
    public void ForceGenerateShield()
    {
        if (shieldInstance != null) return;
        shieldInstance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, gameObject.transform);
        if (sfxShieldActivated != null) MusicManager.Instance.audioSource.PlayOneShot(sfxShieldActivated, 0.75f);
        timer = 0;
    }
}
