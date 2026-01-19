using System;
using UnityEngine;

public class ProjectileController : MonoBehaviour, IManaged
{
    [Header("References")] [SerializeField]
    private ProjectilePool _projectilePool;

    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileParent;

    [Header("Projectile Properties")] [SerializeField]
    public float _projectileSpeed;

    [SerializeField] public int _projectileDamage;
    [SerializeField] private float _projectileLifetime;
    [SerializeField] private int _projectilePoolSize;


    [Header("Spawner Settings")] [SerializeField]
    public float _shootingSpeed = 1;

    [SerializeField] public float _rotationSpeed = 200f;

    private float _timeSinceLastSpawn;


    public ProjectileController(float projectileSpeed, int projectileDamage)
    {
        _projectileSpeed = projectileSpeed;
        _projectileDamage = projectileDamage;
    }

    public void Start()
    {
        GameManager.Instance.RegisterManagedObject(this);
        _projectilePool.GrowPool(_projectilePoolSize, _projectilePrefab);
    }

    public void ManagedUpdate()
    {
        SpawnProjectileMultiple(_shootingSpeed);
        transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        GameManager.Instance.UnregisterManagedObject(this);
    }

    public void SpawnProjectileMultiple(float _shootingSpeed)
    {
        _timeSinceLastSpawn += Time.deltaTime;

        if (!(_timeSinceLastSpawn >= 1f / _shootingSpeed)) return;
        SpawnProjectile();
        _timeSinceLastSpawn = 0f;
    }


    private void SpawnProjectile()
    {
        var projectile = _projectilePool.GetProjectile(_projectilePrefab);
        projectile.transform.position = _projectileParent.position;
        projectile.transform.rotation = gameObject.transform.rotation;
        projectile.SetDmgVel(_projectileDamage, _projectileSpeed);
        //_projectilePool.ReturnProjectile(projectile, _projectileLifetime);
        projectile.StartReturnTimer(_projectileLifetime);
    }
}