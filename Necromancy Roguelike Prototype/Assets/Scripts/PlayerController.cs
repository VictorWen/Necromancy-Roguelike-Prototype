using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5;
    [SerializeField] private ProjectileController playerProjectile;
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private float soulPower = 0;
    [SerializeField] private float soulCost = 3;
    [SerializeField] private int bullets = 6;
    [SerializeField] private int maxBullets = 6;
    [SerializeField] private float reloadTime = 1.5f;
    [SerializeField] private Text soulPowerText;
    [SerializeField] private Text bulletText;

    private SpriteRenderer sprite;
    private float reloadTimer = 0;

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
            ShootBullet(1, DamageInfo.CreatePlayerDamageInfo());
        }

        if (Input.GetMouseButtonDown(1) && soulPower >= soulCost)
        {
            ShootBullet(3, DamageInfo.CreateSoulDamageInfo());
            soulPower -= soulCost;
        }

        if (Input.GetKeyDown(KeyCode.R) && reloadTimer <= 0)
        {
            reloadTimer = reloadTime;
        }
        ReloadTimerTick();

        UpdateBulletText();
    }

    private void ShootBullet(int damage, DamageInfo bulletDamageInfo)
    {
        if (bullets > 0)
        {
            bullets--;
            ProjectileController bullet = Instantiate(playerProjectile);
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float direction = Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x);
            bullet.transform.position = transform.position;
            bullet.Initialize(direction, bulletSpeed, damage, bulletDamageInfo);
            Color color = bulletDamageInfo.isRevivalDamage ? Color.cyan : Color.white;
            bullet.SetColor(color);
        }
        else if (reloadTimer <= 0)
        {
            reloadTimer = reloadTime;
        }
    }

    private void ReloadTimerTick()
    {
        if (reloadTimer > 0)
        {
            reloadTimer = Mathf.Max(reloadTimer - Time.deltaTime, 0);
            if (reloadTimer <= 0)
                bullets = maxBullets;
        }   
    }

    private void UpdateSoulPowerText()
    {
        soulPowerText.text = string.Format("<color=cyan>Soul Power: {0:d}</color>", (int) soulPower);
    }

    private void UpdateBulletText()
    {
        if (reloadTimer <= 0)
            bulletText.text = string.Format("<b>Bullets:</b> {0:d}", bullets);
        else
            bulletText.text = string.Format("<b>Reloading...</b> {0:f}s", reloadTimer);
    }
}
