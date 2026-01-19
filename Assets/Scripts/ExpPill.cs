using System;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class ExpPill : MonoBehaviour, IManaged
{
    [SerializeField] private float _speed;
    [SerializeField] private int _detectionRange;
    [SerializeField] private int _xp;


    private PlayerController _player;


    private float _distanceToPlayer;


    public void Initialize(PlayerController player)
    {
        _player = player;
        GameManager.Instance.RegisterManagedObject(this);
    }

    public void ManagedUpdate()
    {
        ChasePlayer();
    }


    private void ChasePlayer()
    {
        if (!_player)
        {
            return;
        }

        _distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (_distanceToPlayer < _detectionRange)
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            transform.position += direction * (_speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        _player.TakeExperience(_xp);
        gameObject.SetActive(false);
    }
}