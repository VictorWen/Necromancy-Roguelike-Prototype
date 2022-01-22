using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int numEnemies;
    [SerializeField] private Bounds bounds;
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] private EntityLedger ledger;

    private void Start()
    {
        for (int i = 0; i < numEnemies; i++)
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);

            EnemyController enemy = Instantiate(enemyPrefab);
            enemy.transform.position = new Vector3(x, y, 0);

            enemy.Initialize(ledger);
        }
    }
}
