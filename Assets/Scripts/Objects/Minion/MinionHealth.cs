using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class MinionHealth : MonoBehaviour
{
    public int Health;
    public GameObject explode;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void TakeDamage(int Damage)
    {
        Health -= Damage;
        animator.SetTrigger("Taking Damage");
        if (Health <= 0)
        {
            Instantiate(explode, this.transform.position, Quaternion.identity);
            if (TutorialManager.Instance != null)
            {
                if (TutorialManager.Instance.ShootTut == true)
                {
                    TutorialManager.Instance.ShootTut = false;
                    TutorialManager.Instance.StartCutscene();
                }
            }
            Destroy(this.gameObject);
        }
    }
}
