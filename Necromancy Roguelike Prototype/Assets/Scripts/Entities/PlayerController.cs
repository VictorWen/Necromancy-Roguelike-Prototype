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

    [SerializeField] private float dodgeTime = 0.75f;
    [SerializeField] private float dodgeCooldown = 1.5f;
    [SerializeField] private float dodgeForce = 4f;

    [SerializeField] private EntityLedger ledger;

    [SerializeField] private Text soulPowerText;
    [SerializeField] private Text minionText;
    [SerializeField] private Text bulletText;
    
    private Text healthText;
    private HealthScript health;

    private Rigidbody2D rigidBody;
    private SpriteRenderer sprite;
    private AimCone aimCone;
    
    private float reloadTimer = 0;
    private float dodgeTimer = 0;

    private Dictionary<string, BaseItem> items;

    public AttributeCalculator Attributes { get; private set; }

    public void AddSoulPower(float soulPower)
    {
        this.soulPower += soulPower;
    }

    public void AddItem(BaseItem item)
    {
        if (items.ContainsKey(item.ID)) {
            items[item.ID].Count += item.Count;
        }
        else
        {
            items.Add(item.ID, item);
        }

        items[item.ID].OnAdd(this);
    }

    public void RemoveItem(BaseItem item)
    {
        if (items.ContainsKey(item.ID))
        {
            items[item.ID].Count -= item.Count;
            if (items[item.ID].Count <= 0)
            {
                items.Remove(item.ID);
            }

            items[item.ID].OnRemove(this);
        }
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
        Attributes = new AttributeCalculator();
        items = new Dictionary<string, BaseItem>();
        dodgeTimer = dodgeCooldown;
    }

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        aimCone = GetComponent<AimCone>();

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

        AimAndShootTick();

        DodgeTick(hori, vert);

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

    private void AimAndShootTick()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (bullets > 0)
            {
                if (Input.GetMouseButtonDown(0) || soulPower >= soulCost)
                {
                    aimCone.Show();
                    aimCone.SetAngle(Mathf.PI / 4);
                }
            }
            else if (reloadTimer <= 0)
            {
                ReloadWeapon();
            }
        }

        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && aimCone.IsActive)
        {
            if (aimCone.Angle > 0)
                aimCone.SetAngle(Mathf.Max(0, aimCone.Angle - 0.5f * Time.deltaTime));

            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            aimCone.SetRotation(Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x));
        }

        if (Input.GetMouseButtonUp(0) && aimCone.IsActive)
        {
            ShootBullet(aimCone.RandomAim(), 1, DamageInfo.CreatePlayerDamageInfo());

            aimCone.Hide();
        }
        else if (Input.GetMouseButtonUp(1) && soulPower >= soulCost && aimCone.IsActive)
        {
            ShootBullet(aimCone.RandomAim(), 3, DamageInfo.CreateSoulDamageInfo());
            soulPower -= soulCost;

            aimCone.Hide();
        }
    }

    private void DodgeTick(float hori, float vert)
    {
        dodgeTimer = Mathf.Max(-dodgeCooldown, dodgeTimer - Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.LeftShift) && dodgeTimer == -dodgeCooldown && (hori != 0 || vert != 0))
        {
            dodgeTimer = dodgeTime;
            health.IsInvulnerable = true;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);

            if (hori != 0 || vert != 0)
            {
                Vector3 direction = new Vector3(hori, vert);
                rigidBody.AddForce(dodgeForce * direction.normalized * acceleration * rigidBody.mass, ForceMode2D.Impulse);
            }
        }
        if (health.IsInvulnerable && dodgeTimer <= 0)
        {
            health.IsInvulnerable = false;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        }
    }

    private void ShootBullet(float direction, int damage, DamageInfo bulletDamageInfo)
    {
        bullets--;
        ProjectileController bullet = Instantiate(playerProjectile);
        bullet.transform.position = transform.position;
        bullet.Initialize(direction, bulletSpeed, damage, bulletDamageInfo);
        Color color = bulletDamageInfo.isRevivalDamage ? Color.cyan : Color.white;
        bullet.SetColor(color);
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
        reloadTimer = reloadTime * Attributes.GetAttribute(Attribute.RELOAD_TIME_MULTIPLIER);
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
            bulletText.text = string.Format("<b>Reloading...</b> {0:f1}s", reloadTimer);
    }

    private void UpdateHealthText()
    {
        healthText.text = string.Format("<color=red>{0:d} HP</color>", health.Health);
    }
}
