using System;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5;
    [SerializeReference] private ProjectileController playerProjectile;
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private float soulPower = 0;
    [SerializeReference] private Text HUDText;

    private SpriteRenderer sprite;

    public void AddSoulPower(float soulPower)
    {
        this.soulPower += soulPower;
    }

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        UpdateSoulPowerText();

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

        if (Input.GetMouseButtonDown(0))
        {
            ProjectileController bullet = Instantiate(playerProjectile);
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float direction = Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x);
            bullet.transform.position = transform.position;
            bullet.Initialize(direction, bulletSpeed, true);
        }
    }
    
    private void UpdateSoulPowerText()
    {
        HUDText.text = String.Format("<color=cyan>Soul Power: {0:d}</color>", (int) soulPower);
    }
}
