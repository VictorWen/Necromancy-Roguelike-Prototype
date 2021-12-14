using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public bool IsPlayerProjectile { get { return playerProjectile; } }

    private float direction;
    private float speed;
    private bool playerProjectile;
    private float lifeCounter = 0;
    [SerializeField] private float lifespan = 10;

    private int damage;
    private DamageInfo damageInfo;

    private Rigidbody2D rigidBody;

    public void Initialize(float direction, float speed, bool playerProjectile=false)
    {
        this.direction = direction;
        this.speed = speed;
        this.playerProjectile = playerProjectile;
        this.damage = 1;
        this.damageInfo = playerProjectile ? DamageInfo.CreatePlayerDamageInfo() : DamageInfo.CreateEnemyDamageInfo();
    }

    public void Initialize(float direction, float speed, int damage, DamageInfo damageInfo)
    {
        this.direction = direction;
        this.speed = speed;
        this.damage = damage;
        this.playerProjectile = damageInfo.isPlayerFriendlyDamage;
        this.damageInfo = damageInfo;
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
        //Vector3 lastPosition = transform.position;
        //transform.position += speed * Time.deltaTime * new Vector3(Mathf.Cos(direction), Mathf.Sin(direction));
        //RaycastHit2D[] hits = Physics2D.RaycastAll(lastPosition, transform.position);

        //foreach (RaycastHit2D hit in hits)
        //{
        //    if (hit.collider != null)
        //    {
        //        HealthScript health = hit.collider.GetComponent<HealthScript>();
        //        if (health != null && health.IsPlayerHealth != IsPlayerProjectile)
        //        {
        //            health.Damage(damage, damageInfo);
        //            Destroy(gameObject);
        //            break;
        //        }
        //    }
        //}
        lifeCounter += Time.deltaTime;
        if (lifeCounter >= lifespan)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            HealthScript health = collider.GetComponent<HealthScript>();
            if (health != null && health.IsPlayerHealth != IsPlayerProjectile)
            {
                health.Damage(damage, damageInfo);
                Destroy(gameObject);
                return;
            }
        }
    }
}
