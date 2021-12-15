using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityLedger : MonoBehaviour
{
    public PlayerController Player { get; set; }

    private List<EnemyController> enemies = new List<EnemyController>();
    private List<MinionController> minions = new List<MinionController>();

    public void AddEnemy(EnemyController enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(EnemyController enemy)
    {
        enemies.Remove(enemy);
    }

    public void AddMinion(MinionController minion)
    {
        minions.Add(minion);
    }

    public void RemoveMinion(MinionController minion)
    {
        minions.Remove(minion);
    }

    public EnemyController[] Enemies { get { return enemies.ToArray();  } }

    public MinionController[] Minions { get { return minions.ToArray(); } }
}
