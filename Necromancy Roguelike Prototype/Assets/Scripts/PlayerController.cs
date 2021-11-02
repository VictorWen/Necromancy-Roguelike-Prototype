using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int Health { get { return health; } }

    [SerializeReference] private int health = 10;
    [SerializeReference] private float movementSpeed = 5;

    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float hori = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        
        // Move
        if (hori != 0 || vert != 0)
        {
            Vector3 direction = new Vector3(hori, vert);
            direction /= Vector3.Distance(direction, new Vector3(0, 0));
            transform.position += direction * movementSpeed * Time.deltaTime;
        }

        // Flip sprite on direction change
        if (hori > 0 && sprite.flipX)
            sprite.flipX = false;
        else if (hori < 0 && !sprite.flipX)
            sprite.flipX = true;

        // Move Camera on top of player
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
    }
}
