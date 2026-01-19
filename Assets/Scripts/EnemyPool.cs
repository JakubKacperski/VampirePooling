using UnityEngine;
using System.Collections.Generic;

public class EnemyPool : MonoBehaviour
{
    public Queue<Enemy> _enemyPool;
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private GameObject _container;

    private void Awake()
    {
        _enemyPool = new Queue<Enemy>();
    }

    public void GrowPool(int amount, GameObject aEnemyPrefab, PlayerController playerController,
        EnemyController enemyController)
    {
        for (var i = 0; i < amount; i++)
        {
            var enemyObject = Instantiate(aEnemyPrefab, _container.transform);
            var enemy = enemyObject.GetComponent<Enemy>();
            if (!enemy)
            {
                enemy = enemyObject.AddComponent<Enemy>();
            }

            enemy.Initialize(this, playerController, enemyController);
            enemyObject.SetActive(false);
            _enemyPool.Enqueue(enemy);
        }
    }

    public Enemy GetEnemy(GameObject aEnemyPrefab, PlayerController playerController, EnemyController enemyController)
    {
        enemyController.Counter++;
        if (_enemyPool.Count <= 0)
        {
            GrowPool(1, aEnemyPrefab, playerController, enemyController);
        }

        var enemy = _enemyPool.Dequeue();
        enemy.gameObject.SetActive(true);
        return enemy;
    }

    public void ReturnEnemy(Enemy enemy, EnemyController enemyController)
    {
        enemy.gameObject.SetActive(false);
        _enemyPool.Enqueue(enemy);
        enemyController.Counter -= 1;
        enemyController.EnemiesKilled += 1;
    }
}