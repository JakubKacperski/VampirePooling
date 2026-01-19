using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject Enemy;
    [SerializeField] private int MaxEnemies = 10;
    [SerializeField] private float TimeBetweenSpawns = 2f;
    public int Counter;


    private float _timeUntilNextSpawn;

    private void Start()
    {
        _timeUntilNextSpawn = TimeBetweenSpawns;
    }

    private void Update()
    {
        _timeUntilNextSpawn -= Time.deltaTime;

        if (!(_timeUntilNextSpawn <= 0) || Counter >= MaxEnemies) return;
        Counter += SpawnEnemy();
        _timeUntilNextSpawn = TimeBetweenSpawns;
    }

    private int SpawnEnemy()
    {
        var enemy = Instantiate(Enemy, transform.position, transform.rotation);
        enemy.GetComponent<Rigidbody2D>().velocity = transform.up * 100;
        return 1;
    }
}