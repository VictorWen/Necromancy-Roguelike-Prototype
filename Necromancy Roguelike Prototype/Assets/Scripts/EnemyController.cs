using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private ProjectileController projectilePrefab;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject soulPickup;

    [SerializeField] private float direction = 0;
    [SerializeField] private float movementSpeed = 2;
    [SerializeField] private float rotatingSpeed = 5;
    [SerializeField] private float followingDistance = 1;

    [SerializeField] private float shootingSpeed = 2;
    [SerializeField] private float projectileSpeed = 10;
    private float shootingCounter = 0;

    private void Start()
    {
        HealthScript health = GetComponent<HealthScript>();
        health.OnDeath += SpawnSoulDrop;
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 playerPosition = player.transform.position;
            if (Vector3.Distance(transform.position, playerPosition) >= followingDistance)
            {
                transform.position = transform.position + movementSpeed * (new Vector3(Mathf.Cos(direction), Mathf.Sin(direction))) * Time.deltaTime;
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

    private void SpawnSoulDrop(bool isPlayer)
    {
        if (isPlayer)
        {
            GameObject soul = Instantiate(soulPickup);
            soul.transform.position = transform.position;
        }
    }
}
