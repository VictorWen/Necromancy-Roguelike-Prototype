using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    [SerializeField] private EntityLedger ledger;
    [SerializeField] private float followDistance = 1;
    [SerializeField] private float returnDistance = 4;
    [SerializeField] private float targetDistance = 2;
    [SerializeField] private float acceleration = 3;
    [SerializeField] private float attackCooldown = 1.5f;

    private MinionAIState state = MinionAIState.RETURN;
    private EnemyController target;
    private float cooldownTimer = 0;

    private Rigidbody2D rigidBody;

    private enum MinionAIState
    {
        FOLLOW,
        TARGET,
        RETURN
    }

    public void Initialize(EntityLedger ledger)
    {
        this.ledger = ledger;
    }

    private void Start()
    {
        HealthScript health = GetComponent<HealthScript>();
        health.OnDeath += (DamageInfo info) => ledger.Player.RemoveMinion();
        health.OnDeath += (DamageInfo info) => ledger.RemoveMinion(this);

        rigidBody = GetComponent<Rigidbody2D>();
        
        ledger.AddMinion(this);
    }

    private void Update()
    {
        GetTarget();

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
    }

    private void Follow()
    {
        Vector3 direction = ledger.Player.transform.position - transform.position;
        if (direction.magnitude > followDistance)
        {
            rigidBody.AddForce(direction.normalized * acceleration * rigidBody.mass, ForceMode2D.Impulse);
            return;
        }

        if (target != null && Vector3.Distance(target.transform.position, ledger.Player.transform.position) <= returnDistance)
        {
            state = MinionAIState.TARGET;
            return;
        }
    }

    private void Return()
    {
        Vector3 direction = ledger.Player.transform.position - transform.position;
        if (direction.magnitude <= followDistance)
        {
            state = MinionAIState.FOLLOW;
            return;
        }

        rigidBody.AddForce(direction.normalized * acceleration * rigidBody.mass, ForceMode2D.Impulse);
    }

    private void Target()
    {
        if (target == null || Vector3.Distance(ledger.Player.transform.position, transform.position) > returnDistance)
        {
            state = MinionAIState.RETURN;
            return;
        }

        Vector3 direction = target.transform.position - transform.position;
        if (direction.magnitude > 0.25)
            rigidBody.AddForce(direction.normalized * acceleration * rigidBody.mass, ForceMode2D.Impulse);
        else
        {
            if (cooldownTimer == 0)
            {
                HealthScript health = target.GetComponent<HealthScript>();
                if (health != null)
                {
                    health.Damage(1, DamageInfo.CreateMinionDamageInfo());
                    cooldownTimer = attackCooldown;
                }
            }
        }
    }

    private void GetTarget()
    {
        EnemyController[] enemies = ledger.Enemies;
        EnemyController closest = null;
        float closetDistance = Mathf.Infinity;
        foreach (EnemyController enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            float playerDistance = Vector3.Distance(enemy.transform.position, ledger.Player.transform.position);
            if (distance <= 2 && playerDistance <= returnDistance && distance < closetDistance)
            {
                closest = enemy;
                closetDistance = distance;
            }
        }
        target = closest;
    }
}
