using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulPickupScript : MonoBehaviour
{
    [SerializeField] private float soulPower = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.AddSoulPower(soulPower);
            Destroy(gameObject);
        }
    }
}
