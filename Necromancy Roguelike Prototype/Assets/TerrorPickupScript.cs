using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrorPickupScript : MonoBehaviour
{
    [SerializeField] int terror = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.AddTerror(terror);
            Destroy(gameObject);
        }
    }
}
