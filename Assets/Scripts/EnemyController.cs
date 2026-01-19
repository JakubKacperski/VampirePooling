using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour, IManaged
{
    [SerializeField] public int Counter;

    [Header("References")] [SerializeField]
    private EnemyPool enemyPool;

    [SerializeField] private Transform enemyParent;

    [Header("Enemy Properties")] [SerializeField]
    private int enemyPoolSizePerType = 5;

    [SerializeField] private string enemyPrefabsPath = "Enemies";

    [Header("Spawner Settings")] [SerializeField]
    private int spawningSpeed = 1;

    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float maxEnemies = 200f;

    private float _timeSinceLastSpawn;
    private PlayerController _player;
    private List<GameObject> _enemyPrefabs = new();
    public int EnemiesKilled;

    public void Start()
    {
        if (!GameManager.Instance)
        {
            Debug.LogError("GameManager.Instance is null!");
            return;
        }

        GameManager.Instance.RegisterManagedObject(this);

        _player = FindObjectOfType<PlayerController>();
        if (!_player)
        {
            Debug.LogError("PlayerController not found in the scene!");
            return;
        }

        LoadEnemyPrefabs();

        if (!enemyPool)
        {
            Debug.LogError("EnemyPool reference is missing!");
            return;
        }

        if (_enemyPrefabs.Count > 0)
        {
            foreach (var prefab in _enemyPrefabs)
            {
                enemyPool.GrowPool(enemyPoolSizePerType, prefab, _player, this);
            }
        }
        else
        {
            Debug.LogError("No enemy prefabs could be loaded!");
        }
    }

    private void LoadEnemyPrefabs()
    {
        _enemyPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>(enemyPrefabsPath));
        if (_enemyPrefabs.Count == 0)
        {
            Debug.LogError($"Failed to load any enemy prefabs from path: {enemyPrefabsPath}");
        }
        else
        {
            Debug.Log($"Loaded {_enemyPrefabs.Count} enemy prefabs from Resources.");
        }
    }

    public void ManagedUpdate()
    {
        SpawnEnemyMultiple(spawningSpeed);
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.UnregisterManagedObject(this);
        }
    }

    private void SpawnEnemyMultiple(int currentSpawningSpeed)
    {
        _timeSinceLastSpawn += Time.deltaTime;
        if (!(_timeSinceLastSpawn >= 1f / currentSpawningSpeed && Counter < maxEnemies)) return;
        SpawnEnemy();
        _timeSinceLastSpawn = 0f;
    }

    private void SpawnEnemy()
    {
        if (!enemyPool || _enemyPrefabs.Count == 0 || !enemyParent)
        {
            Debug.LogError("Missing references for enemy spawning!");
            return;
        }

        var randomPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Count)];
        var enemy = enemyPool.GetEnemy(randomPrefab, _player, this);
        if (enemy)
        {
            Vector2 randomOffset = Random.insideUnitCircle;
            Vector3 spawnPosition = enemyParent.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            enemy.transform.position = spawnPosition;
            enemy.transform.rotation = enemyParent.rotation;
        }
        else
        {
            Debug.LogWarning("Failed to get enemy from pool!");
        }
    }
}