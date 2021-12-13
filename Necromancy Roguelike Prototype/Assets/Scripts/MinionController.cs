using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float followDistance = 1;
    [SerializeField] private float returnDistance = 4;
    [SerializeField] private float movementSpeed = 3;
    [SerializeField] private float attackCooldown = 2;

    [SerializeField] private Vector3 velocity = new Vector3();

    private MinionAIState state = MinionAIState.RETURN;
    private EnemyController target;
    private float cooldownTimer = 0;

    private enum MinionAIState
    {
        FOLLOW,
        TARGET,
        RETURN
    }

    private void Update()
    {
        switch (state)
        {
            case MinionAIState.FOLLOW:
                Follow();
                break;
            case MinionAIState.RETURN:
                Return();
                break;
            case MinionAIState.TARGET:
                Target();
                break;
        }

        if (cooldownTimer > 0)
            cooldownTimer = Mathf.Max(0, cooldownTimer - Time.deltaTime);
        transform.position += velocity * Time.deltaTime;
    }

    private void Follow()
    {
        Vector3 direction = player.transform.position - transform.position;
        if (direction.magnitude > followDistance)
        {
            velocity = direction.normalized * movementSpeed;
            return;
        }
        velocity = new Vector3(0, 0, 0);

        if (target != null && Vector3.Distance(target.transform.position, player.transform.position) <= returnDistance)
        {
            state = MinionAIState.TARGET;
            return;
        }
    }

    private void Return()
    {
        Vector3 direction = player.transform.position - transform.position;
        if (direction.magnitude <= followDistance)
        {
            state = MinionAIState.FOLLOW;
            return;
        }

        velocity = direction.normalized * movementSpeed;
    }

    private void Target()
    {
        if (target == null || Vector3.Distance(player.transform.position, transform.position) > returnDistance)
        {
            state = MinionAIState.RETURN;
            return;
        }

        Vector3 direction = target.transform.position - transform.position;
        if (direction.magnitude > 0.25)
            velocity = direction.normalized * movementSpeed;
        else
        {
            velocity = new Vector3(0, 0, 0);
            if (cooldownTimer == 0)
            {
                HealthScript health = target.GetComponent<HealthScript>();
                if (health != null)
                {
                    health.Damage(1);
                    cooldownTimer = attackCooldown;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
            target = enemy;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
        if (enemy != null && target == enemy)
            target = null;
    }
}
