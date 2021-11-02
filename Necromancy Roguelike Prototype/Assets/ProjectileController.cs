using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private float direction;
    private float speed;
    private float lifeCounter = 0;
    private float lifespan = 10;

    public void Initialize(float direction, float speed)
    {
        this.direction = direction;
        this.speed = speed;
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime * new Vector3(Mathf.Cos(direction), Mathf.Sin(direction));
        lifeCounter += Time.deltaTime;
        if (lifeCounter >= lifespan)
            Destroy(gameObject);
    }
}
