using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private List<IManaged> _managedObjects = new();
    public GameObject musicPlayer;

    public MainMenu _mainMenu;
    private AudioLowPassFilter _audioLowPassFilter;

    private bool _isPaused = false;
    public bool IsPaused => _isPaused;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _mainMenu = GameObject.Find("MainMenu").GetComponent<MainMenu>();
        _audioLowPassFilter = musicPlayer.GetComponent<AudioLowPassFilter>();
        PauseGame();
    }


    private void Update()
    {
        TogglePause();

        if (_isPaused) return;

        for (int i = _managedObjects.Count - 1; i >= 0; i--)
        {
            _managedObjects[i]?.ManagedUpdate();
        }
    }


    public void RegisterManagedObject(IManaged obj)
    {
        if (!_managedObjects.Contains(obj))
        {
            _managedObjects.Add(obj);
        }
    }

    public void UnregisterManagedObject(IManaged obj)
    {
        _managedObjects.Remove(obj);
    }

    public void TogglePause()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (_isPaused)
        {
            ResumeGame();
            _mainMenu.StartGame();
        }
        else
        {
            PauseGame();
            _mainMenu.StartMenu();
        }
    }

    public void PauseGame()
    {
        _isPaused = true;
        _audioLowPassFilter.enabled = true;
    }


    public void Upgrade()
    {
        PauseGame();
        _mainMenu.Upgrade();
    }

    public void ResumeGame()
    {
        _isPaused = false;
        _audioLowPassFilter.enabled = false;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

public interface IManaged
{
    void ManagedUpdate();
}