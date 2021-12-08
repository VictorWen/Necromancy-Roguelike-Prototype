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

    public bool IsPlayerProjectile { get { return playerProjectile; } }

    public void Initialize(float direction, float speed, bool playerProjectile=false)
    {
        this.direction = direction;
        this.speed = speed;
        this.playerProjectile = playerProjectile;
    }

    private void Update()
    {
        Vector3 lastPosition = transform.position;
        transform.position += speed * Time.deltaTime * new Vector3(Mathf.Cos(direction), Mathf.Sin(direction));
        RaycastHit2D hit = Physics2D.Raycast(lastPosition, transform.position);
        
        if (hit.collider != null)
        {
            print(hit.collider);
            HealthScript health = hit.collider.GetComponent<HealthScript>();
            if (health != null && health.IsPlayerHealth != IsPlayerProjectile)
            {
                health.Damage(1);
                Destroy(gameObject);
            }
        }
        lifeCounter += Time.deltaTime;
        if (lifeCounter >= lifespan)
            Destroy(gameObject);
    }
}
