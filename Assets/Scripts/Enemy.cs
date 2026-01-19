using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Image = UnityEngine.UI.Image;

public class Enemy : MonoBehaviour, IManaged
{
    [SerializeField] private int _damage;
    [SerializeField] private float _health;
    [SerializeField] private float _speed;
    [SerializeField] private int _detectionRange;
    [SerializeField] public float _animationSpeed;
    [SerializeField] private GameObject _expierenceObject;
    [SerializeField] private GameObject _explosionObject;
    [SerializeField] private GameObject _textObject;
    private PlayerController _player;
    private EnemyPool _myEnemyPool;
    private float _distanceToPlayer;
    private float _maxHealth;
    private Image _imageHp;
    private EnemyController _enemyController;
    private bool _isTouching = false;
    private GameObject _expPill;
    private ParticleSystem _particles;
    private TextMeshPro _text;
    private Animation _animation;


    public void Initialize(EnemyPool myEnemyPool, PlayerController player, EnemyController enemyController)
    {
        _myEnemyPool = myEnemyPool;
        _player = player;
        AttachHpCircleToThis();
        _imageHp = GetComponentInChildren<Image>();
        GameManager.Instance.RegisterManagedObject(this);
        _maxHealth = _health;
        _enemyController = enemyController;
        gameObject.tag = "Enemy";
    }

    public void ManagedUpdate()
    {
        ChasePlayer();
        Animate();
    }

    private void Animate()
    {
        if (!_isTouching)
        {
            transform.Rotate(0f, 0f, _animationSpeed * Time.deltaTime);
        }
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

    public void TakeDamage(float damage)
    {
        UpdateHealth(damage);
        _health -= damage;
        Text(damage);

        if (!(_health <= 0)) return;
        _health = _maxHealth + 10;
        _maxHealth = _health;
        _imageHp.fillAmount = 1;
        Explosion();
        SpawnExp();
        _myEnemyPool?.ReturnEnemy(this, _enemyController);
    }

    private void Explosion()
    {
        if (_particles)
        {
            _particles.Play();
            _particles.transform.position = transform.position;
            return;
        }

        var particlesObject = Instantiate(_explosionObject, transform.position, Quaternion.identity);
        _particles = particlesObject.GetComponent<ParticleSystem>();
    }

    private void Text(float damage)
    {
        if (_text)
        {
            _text.text = damage.ToString() + " " + "dmg";
            _animation.Play();
            _text.transform.position = transform.position;
            return;
        }

        var textObject = Instantiate(_textObject, transform.position, Quaternion.identity);
        _animation = textObject.GetComponent<Animation>();

        _text = textObject.GetComponent<TextMeshPro>();
        _text.text = damage.ToString(CultureInfo.InvariantCulture) + " " + "dmg";
    }

    private void SpawnExp()
    {
        if (_expPill)
        {
            _expPill.SetActive(true);
            _expPill.transform.position = transform.position;
            return;
        }

        var expObject = Instantiate(_expierenceObject, transform.position, Quaternion.identity);
        expObject.GetComponent<ExpPill>().Initialize(_player);
        _expPill = expObject;
    }

    public void UpdateHealth(float damage)
    {
        if (_imageHp != null) _imageHp.fillAmount -= damage / _maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        _isTouching = true;
        _player.TakeDamage(_damage);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        _isTouching = false;
    }

    public void AttachHpCircleToThis()
    {
        GameObject hpCirclePrefab = Resources.Load<GameObject>("HpCircle");

        if (hpCirclePrefab)
        {
            GameObject hpCircleInstance = Instantiate(hpCirclePrefab, transform);
            hpCircleInstance.transform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.LogError("Can't find Hpcircle in Resources");
        }
    }
}