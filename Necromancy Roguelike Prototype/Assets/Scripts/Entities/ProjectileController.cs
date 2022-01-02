using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private float direction;
    private float speed;
    private bool playerProjectile;
    private float lifeCounter = 0;
    [SerializeField] private float lifespan = 10;

    private int damage;
    private DamageInfo damageInfo;

    private float criticalChance = AttributeCalculator.GlobalDefaultValues[Attribute.CRITICAL_CHANCE];
    private float criticalMultiplier = AttributeCalculator.GlobalDefaultValues[Attribute.CRITICAL_MULTIPLIER];
    [SerializeField] private Canvas textParticlePrefab;

    private Rigidbody2D rigidBody;

    public bool IsPlayerProjectile { get { return playerProjectile; } }

    public void Initialize(float direction, float speed, bool playerProjectile=false)
    {
        this.direction = direction;
        this.speed = speed;
        this.playerProjectile = playerProjectile;
        this.damage = 1;
        this.damageInfo = playerProjectile ? DamageInfo.CreatePlayerDamageInfo() : DamageInfo.CreateEnemyDamageInfo();
    }

    public void Initialize(float direction, float speed, int damage, DamageInfo damageInfo, float criticalChance = -1, float criticalMultiplier = -1)
    {
        this.direction = direction;
        this.speed = speed;
        this.damage = damage;
        this.playerProjectile = damageInfo.isPlayerFriendlyDamage;
        this.damageInfo = damageInfo;

        if (criticalChance >= 0)
            this.criticalChance = criticalChance;
        if (criticalMultiplier >= 0)
            this.criticalMultiplier = criticalMultiplier;
    }

    public void SetColor(Color color)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = color;
    }

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(Mathf.Cos(direction), Mathf.Sin(direction)) * speed;
    }

    private void Update()
    {
        lifeCounter += Time.deltaTime;
        if (lifeCounter >= lifespan)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            HealthScript health = collider.GetComponent<HealthScript>();
            if (health != null && !health.IsInvulnerable && health.IsPlayerHealth != IsPlayerProjectile)
            {

                // Critical chance roll
                if (Random.Range(0f, 1f) <= criticalChance)
                {
                    damage = (int)(damage * criticalMultiplier + 0.5f);
                    // Show some effect
                    Canvas c = Instantiate(textParticlePrefab);
                    c.transform.position = transform.position;
                }

                health.Damage(damage, damageInfo);
                Destroy(gameObject);
                return;
            }
            else if (collider.CompareTag("Projectile Wall"))
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}
