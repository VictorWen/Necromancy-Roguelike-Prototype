using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int Health { get { return health; } }

    [SerializeField] private ProjectileController projectilePrefab;
    [SerializeField] private PlayerController player;
    
    [SerializeField] private int health = 10;
    [SerializeField] private float direction = 0;
    [SerializeField] private float movementSpeed = 2;
    [SerializeField] private float rotatingSpeed = 5;

    [SerializeField] private float shootingSpeed = 2;
    [SerializeField] private float projectileSpeed = 10;
    private float shootingCounter = 0;

    // Update is called once per frame
    private void Update()
    {
        transform.position = transform.position + movementSpeed * (new Vector3(Mathf.Cos(direction), Mathf.Sin(direction))) * Time.deltaTime;
        Vector3 playerPosition = player.transform.position;
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
