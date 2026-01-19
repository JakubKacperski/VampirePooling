using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController;
    public GameObject settingsObject;
    public GameObject mainObject;
    public GameObject upgradeObject;
    public GameObject CounterObject;
    public ProjectileController projectileController;

    private TextMeshProUGUI _killCounter;


    public enum Menus
    {
        MainMenu = 0,
        Settings = 1,
        Upgrade = 2
    };

    private Menus _activeMenu;
    private Menus _lastMenu;

    private Dictionary<Menus, GameObject> _menuObjects = new Dictionary<Menus, GameObject>();

    public void Start()
    {
        _menuObjects.Add(Menus.MainMenu, mainObject);
        _menuObjects.Add(Menus.Settings, settingsObject);
        _menuObjects.Add(Menus.Upgrade, upgradeObject);

        foreach (var menuObject in _menuObjects.Values)
        {
            menuObject.SetActive(false);
        }

        mainObject.SetActive(true);


        _activeMenu = Menus.MainMenu;

        _killCounter = CounterObject.GetComponent<TextMeshProUGUI>();
    }

    public void StartGame()
    {
        mainObject.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    public void StartMenu()
    {
        _killCounter.text = "Enemies killed: " + _enemyController.EnemiesKilled.ToString();
        mainObject.SetActive(true);
    }


    public void Settings()
    {
        SwitchToMenu(Menus.Settings);
    }

    public void AddDagger()
    {
        projectileController._shootingSpeed += 5;
        projectileController._rotationSpeed += 30;

        upgradeObject.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    public void AddSpeed()
    {
        projectileController._projectileSpeed += 5;
        upgradeObject.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    public void AddDamage()
    {
        projectileController._projectileDamage += 5;
        upgradeObject.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    public void Upgrade()
    {
        SwitchToMenu(Menus.Upgrade);
    }


    public void Back()
    {
        SwitchToMenu(_lastMenu);
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        GameManager.Instance.ResetGame();
    }

    public void SwitchToMenu(Menus menu)
    {
        _lastMenu = _activeMenu;

        GameObject menuObject;
        if (_menuObjects.TryGetValue(_activeMenu, out menuObject))
        {
            menuObject.SetActive(false);
        }

        if (_menuObjects.TryGetValue(menu, out menuObject))
        {
            menuObject.SetActive(true);
        }

        _activeMenu = menu;
    }
}