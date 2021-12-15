using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private string itemID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            BaseItem item = null;
            switch(itemID) // NOTE: this is a bad way of doing this, will change to a better way in full production
            {
                case "bandolier":
                    item = new BandolierItem();
                    break;
                default:
                    item = new BandolierItem();
                    break;
            }
            player.AddItem(item);
            Destroy(gameObject);
        }
    }
}
