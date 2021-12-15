using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float acceleration = 5;
    [SerializeField] private ProjectileController playerProjectile;
    [SerializeField] private float bulletSpeed = 10;
    
    [SerializeField] private float soulPower = 0;
    [SerializeField] private float soulCost = 3;

    [SerializeField] private int maxMinions = 2;
    [SerializeField] private int currentMinions = 0;
    
    [SerializeField] private int bullets = 6;
    [SerializeField] private int maxBullets = 6;
    [SerializeField] private float reloadTime = 1.5f;

    [SerializeField] private EntityLedger ledger;

    [SerializeField] private Text soulPowerText;
    [SerializeField] private Text minionText;
    [SerializeField] private Text bulletText;
    
    private Text healthText;
    private HealthScript health;

    private Rigidbody2D rigidBody;
    private SpriteRenderer sprite;
    private float reloadTimer = 0;

    private List<BaseItem> items;

    public float ReloadTimeModifier { get; set; } = 1f;

    public void AddSoulPower(float soulPower)
    {
        this.soulPower += soulPower;
    }

    public void AddItem(BaseItem item)
    {
        items.Add(item);
        item.OnAdd(this);
    }

    public void RemoveItem(BaseItem item)
    {
        item.OnRemove(this);
        items.Remove(item);
    }

    public bool AddMinion()
    {
        if (currentMinions < maxMinions) 
        {
            currentMinions++;
            return true;
        } else
            return false;
    }

    public void RemoveMinion()
    {
        currentMinions--;
    }

    private void Awake()
    {
        items = new List<BaseItem>();
    }

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        healthText = GetComponentInChildren<Text>();
        health = GetComponent<HealthScript>();

        ledger.Player = this;
    }

    private void Update()
    {
        float hori = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        
        // Move
        if (hori != 0 || vert != 0)
        {
            Vector3 direction = new Vector3(hori, vert);
            rigidBody.AddForce(direction.normalized * acceleration * rigidBody.mass, ForceMode2D.Impulse);
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
            ReloadWeapon();
        }
        ReloadTimerTick();

        UpdateHealthText();
        UpdateSoulPowerText();
        UpdateMinionText();
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
            ReloadWeapon();
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

    private void ReloadWeapon()
    {
        reloadTimer = reloadTime * ReloadTimeModifier;
    }

    private void UpdateSoulPowerText()
    {
        soulPowerText.text = string.Format("<color=cyan>Soul Power: {0:d}</color>", (int) soulPower);
    }

    private void UpdateMinionText()
    {
        minionText.text = string.Format("<b>Minions:</b> {0:d}/{1:d}", currentMinions, maxMinions);
    }

    private void UpdateBulletText()
    {
        if (reloadTimer <= 0)
            bulletText.text = string.Format("<b>Bullets:</b> {0:d}", bullets);
        else
            bulletText.text = string.Format("<b>Reloading...</b> {0:f}s", reloadTimer);
    }

    private void UpdateHealthText()
    {
        healthText.text = string.Format("<color=red>{0:d} HP</color>", health.Health);
    }
}
