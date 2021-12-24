using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FallingParticle : MonoBehaviour
{
    [SerializeField] private float initialVelocity;
    [SerializeField] private float lifespan;
    [SerializeField] private float gravity = -9.81f;

    private float lifetime;

    private new Rigidbody2D rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.AddForce(new Vector2(0, initialVelocity * rigidbody.mass), ForceMode2D.Impulse);
        lifetime = 0;
    }

    private void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime >= lifespan)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        rigidbody.AddForce(new Vector2(0, gravity * rigidbody.mass));
    }
}
