using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private ProjectileController projectilePrefab;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject soulPickup;
    [SerializeField] private MinionController minionPrefab;

    [SerializeField] private float direction = 0;
    [SerializeField] private float acceleration = 2;
    [SerializeField] private float followingDistance = 1;

    [SerializeField] private float shootingSpeed = 2;
    [SerializeField] private float projectileSpeed = 10;
    private float shootingCounter = 0;

    private Rigidbody2D rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        HealthScript health = GetComponent<HealthScript>();
        health.OnDeath += Death;
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 playerPosition = player.transform.position;
            if (Vector3.Distance(transform.position, playerPosition) >= followingDistance)
            {
                rigidBody.AddForce(acceleration * (new Vector3(Mathf.Cos(direction), Mathf.Sin(direction))) * rigidBody.mass, ForceMode2D.Impulse);
            }

            float target_direction = Mathf.Atan2(playerPosition.y - transform.position.y, playerPosition.x - transform.position.x);
            direction = target_direction;

            shootingCounter += Time.deltaTime;
            if (shootingCounter >= shootingSpeed)
            {
                shootingCounter -= shootingSpeed;
                ProjectileController proj = Instantiate(projectilePrefab);
                proj.Initialize(direction, projectileSpeed);
                proj.transform.position = transform.position;
            }
        }
    }

    private void Death(DamageInfo info)
    {
        if (info.isPlayerDealtDamage)
        {
            if (info.isRevivalDamage && player.AddMinion())
            {
                // Revive
                MinionController minion = Instantiate(minionPrefab);
                minion.transform.position = transform.position;
                minion.Initialize(player);
            }
            else if (!info.isRevivalDamage)
            {
                // Drop Soul
                GameObject soul = Instantiate(soulPickup);
                soul.transform.position = transform.position;
            }
        }
    }
}
