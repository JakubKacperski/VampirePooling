using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private GameObject _container;

    public Queue<Projectile> _projectilePool;

    private void Awake()
    {
        _projectilePool = new Queue<Projectile>();
    }

    public void GrowPool(int amount, GameObject projectilePrefab)
    {
        for (int i = 0; i < amount; i++)
        {
            Projectile projectile = CreateProjectile(projectilePrefab);
            AddProjectileToPool(projectile);
        }
    }

    private Projectile CreateProjectile(GameObject projectilePrefab)
    {
        GameObject projectileObject = Instantiate(projectilePrefab, _container.transform);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Initialize(this, 5);
        return projectile;
    }

    private void AddProjectileToPool(Projectile projectile)
    {
        _projectilePool.Enqueue(projectile);
    }

    public Projectile GetProjectile(GameObject aProjectilePrefab)
    {
        Debug.Log(_projectilePool.Count);

        if (_projectilePool.Count <= 10)
        {
            GrowPool(2, aProjectilePrefab);
        }

        var projectile = _projectilePool.Dequeue();
        projectile.gameObject.SetActive(true);
        return projectile;
    }


    public void ReturnProjectile(Projectile projectile)
    {
        _projectilePool.Enqueue(projectile);
        Debug.Log("powrot");
        projectile.gameObject.SetActive(false);
    }
}