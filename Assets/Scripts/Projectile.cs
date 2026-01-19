using System;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour, IManaged
{
    private int _damage;
    private float _speed;
    private ProjectilePool _myProjectilePool;
    private Coroutine _returnCoroutine;

    public void Initialize(ProjectilePool myProjectilePool, float lifetime)
    {
        _myProjectilePool = myProjectilePool;
        GameManager.Instance.RegisterManagedObject(this);
        StartReturnTimer(lifetime);
    }

    public void ManagedUpdate()
    {
        transform.Translate(Vector2.up * (_speed * Time.deltaTime));
    }

    public void SetDmgVel(int damage, float speed)
    {
        _damage = damage;
        _speed = speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        if (collision.TryGetComponent(out Enemy component))
        {
            ReturnToPool();
            component.TakeDamage(_damage);
        }
    }

    public void StartReturnTimer(float lifetime)
    {
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
        }

        _returnCoroutine = StartCoroutine(ReturnAfterDelay(lifetime));
    }

    private IEnumerator ReturnAfterDelay(float delay)
    {
        yield return new PausableWait(delay);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }

        _myProjectilePool.ReturnProjectile(this);
    }

    private void OnDisable()
    {
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }
    }
}