using System.Collections.Generic;
using UnityEngine;

public class UptimePrep : MonoBehaviour
{
    public List<GameObject> minions = new List<GameObject>();
    public void FreezeMinions()
    {
        foreach (GameObject obj in minions)
        {
            if (obj.TryGetComponent(out Rigidbody2D rb))
            {
                rb.simulated = false ;
            }
        }
    }
}
