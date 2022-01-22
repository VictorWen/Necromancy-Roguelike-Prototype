using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EntityLedger ledger;
    [SerializeField] private ProjectileController projectilePrefab;
    [SerializeField] private GameObject soulPickup;
    [SerializeField] private GameObject terrorPickup;
    [SerializeField] private MinionController minionPrefab;

    [SerializeField] private float direction = 0;
    [SerializeField] private float acceleration = 2;
    [SerializeField] private float followingDistance = 1;
    [SerializeField] private float targetingDistance = 5;

    [SerializeField] private float shootingSpeed = 2;
    [SerializeField] private float projectileSpeed = 10;
    private float shootingTimer = 0;

    private AIState state;
    private Rigidbody2D rigidBody;

    private enum AIState
    {
        IDLE,
        FOLLOW,
        TARGET,
    }

    public void Initialize(EntityLedger ledger)
    {
        this.ledger = ledger;
    }

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        HealthScript health = GetComponent<HealthScript>();
        health.OnDeath += Death;

        ledger.AddEnemy(this);
    }

    private void Update()
    {
        if (ledger.Player != null)
        {
            Vector3 playerPosition = ledger.Player.transform.position;
            float distance = Vector3.Distance(playerPosition, transform.position);
            if (distance <= targetingDistance)
            {
                if (state == AIState.IDLE)
                {
                    state = AIState.TARGET;
                }
                else if (state == AIState.FOLLOW)
                {
                    state = AIState.TARGET;
                }
            }
            else if (state == AIState.TARGET)
            {
                state = AIState.FOLLOW;
            }

            if ((state == AIState.FOLLOW || state == AIState.TARGET) && Vector3.Distance(transform.position, playerPosition) >= followingDistance)
            {
                rigidBody.AddForce(acceleration * (new Vector3(Mathf.Cos(direction), Mathf.Sin(direction))) * rigidBody.mass, ForceMode2D.Impulse);

                float noise = distance * 0.15f;
                float target_direction = Mathf.Atan2(playerPosition.y - transform.position.y, playerPosition.x - transform.position.x) + Random.Range(-noise, noise);
                direction = target_direction;
            }

            if (state == AIState.TARGET)
            {
                shootingTimer += Time.deltaTime;
                if (shootingTimer >= shootingSpeed)
                {
                    shootingTimer -= shootingSpeed;
                    ProjectileController proj = Instantiate(projectilePrefab);
                    proj.Initialize(direction, projectileSpeed);
                    proj.transform.position = transform.position;
                }
            }
        }
    }

    private void Death(DamageInfo info)
    {
        if (info.isPlayerDealtDamage)
        {
            if (info.isRevivalDamage && ledger.Player.AddMinion())
            {
                // Revive
                MinionController minion = Instantiate(minionPrefab);
                minion.transform.position = transform.position;
                minion.Initialize(ledger);
            }
            else if (!info.isRevivalDamage)
            {
                // Drop Soul
                // GameObject soul = Instantiate(soulPickup);
                // soul.transform.position = transform.position;
                ledger.Player.AddSoulPower(10);
            }
        }
        else if (info.isPlayerFriendlyDamage)
        {
            GameObject terror = Instantiate(terrorPickup);
            terror.transform.position = transform.position;
        }
        ledger.RemoveEnemy(this);
    }
}
