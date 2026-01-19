using UnityEngine;
using Image = UnityEngine.UI.Image;

public class PlayerController : MonoBehaviour, IManaged
{
    [SerializeField] private int playerMovementSpeed = 5;
    [SerializeField] private Transform sprite;
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int health = 5;
    [SerializeField] private int xp = 100;
    [SerializeField] private float initialMaxXp = 200f;
    [SerializeField] private float xpMultiplier = 2f;
    [SerializeField] private float animationSpeed = 30f;
    [SerializeField] private float maxAnimationAngle = 90f;
    [SerializeField] private float animationMultiplier = 100f;

    private Image _imageHp;
    private Image _imageXp;

    public Vector2 PlayerInputVector;

    private Vector2 _playerMovementDelta;
    private float _animationAngle;
    private float _maxXp;
    private bool _isPlayerMoving;
    private Vector2 _playerInputVector;

    private const string HorizontalAxis = "Horizontal";
    private const string VerticalAxis = "Vertical";
    private const string HpTransformName = "HP";
    private const string XpTransformName = "XP";

    private void Start()
    {
        if (!GameManager.Instance)
        {
            Debug.LogError("GameManager.Instance is null!");
            return;
        }

        GameManager.Instance.RegisterManagedObject(this);
        health = maxHealth;
        _maxXp = initialMaxXp;

        var hpTransform = transform.Find(HpTransformName);
        var xpTransform = transform.Find(XpTransformName);

        if (!hpTransform)
        {
            Debug.LogError($"{HpTransformName} Transform not found!");
            return;
        }

        if (!xpTransform)
        {
            Debug.LogError($"{XpTransformName} Transform not found!");
            return;
        }

        _imageHp = hpTransform.GetComponent<Image>();
        _imageXp = xpTransform.GetComponent<Image>();

        if (!_imageHp)
        {
            Debug.LogError("HP Image component not found!");
            return;
        }

        if (!_imageXp)
        {
            Debug.LogError("XP Image component not found!");
            return;
        }

        xp = 0;
        _imageXp.fillAmount = 0;
    }

    public void ManagedUpdate()
    {
        CollectPlayerInput();
        MovePlayer();
        UpdatePlayerAnimation();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.UnregisterManagedObject(this);
        }
    }

    private void Upgrade()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.Upgrade();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null!");
        }
    }

    private void CollectPlayerInput()
    {
        PlayerInputVector.x = Input.GetAxisRaw(HorizontalAxis);
        PlayerInputVector.y = Input.GetAxisRaw(VerticalAxis);
    }

    private void MovePlayer()
    {
        _playerMovementDelta = PlayerInputVector.normalized * (playerMovementSpeed * Time.deltaTime);
        transform.Translate(_playerMovementDelta.x, _playerMovementDelta.y, 0);
    }

    private void UpdatePlayerAnimation()
    {
        _isPlayerMoving = PlayerInputVector.sqrMagnitude > 0;
        if (!_isPlayerMoving) return;

        if (!sprite)
        {
            Debug.LogError("Sprite Transform is null!");
            return;
        }

        var angle = Mathf.PingPong(Time.time * animationSpeed + (animationMultiplier * _imageHp.fillAmount),
            maxAnimationAngle);
        sprite.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void TakeDamage(int damage)
    {
        UpdateHealth(damage);
        health -= damage;

        if (health > 0) return;
        Debug.Log("Player has died!");
        GameManager.Instance.ResetGame();
    }

    public void TakeExperience(int aExperience)
    {
        UpdateExp(aExperience);
        xp += aExperience;
        if (xp >= _maxXp)
        {
            xp = 0;
            _imageXp.fillAmount = 0;
            _maxXp *= xpMultiplier;
            Upgrade();
        }
    }

    private void UpdateHealth(float damage)
    {
        if (_imageHp != null)
        {
            _imageHp.fillAmount -= damage / maxHealth;
        }
        else
        {
            Debug.LogError("HP Image is null!");
        }
    }

    private void UpdateExp(int aExperience)
    {
        if (_imageXp != null)
        {
            _imageXp.fillAmount += aExperience / _maxXp;
        }
        else
        {
            Debug.LogError("XP Image is null!");
        }
    }
}