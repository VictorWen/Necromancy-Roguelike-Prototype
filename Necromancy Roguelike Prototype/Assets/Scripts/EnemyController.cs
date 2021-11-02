using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int Health { get { return health; } }

    [SerializeReference] private int health = 10;
    [SerializeReference] private float direction = 0;
    [SerializeReference] private float movementSpeed = 2;
    [SerializeReference] private float rotatingSpeed = 5;

    // Update is called once per frame
    private void Update()
    {
        transform.position = transform.position + movementSpeed * (new Vector3(Mathf.Cos(direction), Mathf.Sin(direction))) * Time.deltaTime;
        direction += Random.Range(-1, 2) * rotatingSpeed * Time.deltaTime;
    }
}
