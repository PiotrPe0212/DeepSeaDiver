using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{


    public GameObject MenuPanel;
    public GameObject LoadPanel;
    public GameObject LevelCompPanel;
    public GameObject GamePanel;
    public GameObject GameOverPanel;
    public GameObject EndGamePanel;
    public GameObject PauseGamePanel;
    public GameObject ChestInfo;
    public GameObject GameInfo;
    public AudioSource MenuMusic;
    public AudioSource GameMusic;
    public AudioSource ClickSound;

    string[] _levels;

    float[] _winConditions;

    float _highestDepth;
    bool isSwitching;
    bool _timer;
    public bool _startGame;
    public bool _oxygenLevelReset;
    public  bool _resetLevel;
    bool _pause;
    public  bool _death = false;
    private bool _gameInfoShowed = false;

    int _currentLevel;
    float _currentWinCondition;
    public static GameManager Instance { get; private set; }

    public enum State { MENU, INIT, PLAY, LEVELCOMP, LOADLEVEL, PAUSE, GAMEOVER }
    public State _state;

    private float _depthScore;
    public float DepthScore
    {
        get { return _depthScore; }
        set
        {
            _depthScore = value;
        }
    }

    private float _oxygenLevel;
    public float OxygenLevel
    {
        get { return _oxygenLevel; }
        set
        {
            _oxygenLevel = value;
        }
    }

    private int _keysNumber;

    public int KeyGetting
    {
        get { return _keysNumber; }
        set
        {
            _keysNumber += value;
            Debug.Log(_keysNumber + "KEYS");
        }
    }

    private bool _chest;

    public GameManager()
    {
        _levels = new string[] { "Level11", "Level22","Level33","Level44" };
        _winConditions = new float[] { 1, 1, 2, 3 };
    }

    public bool NearChest
    {
        get { return _chest; }
        set
        {
            _chest = value;
        }
    }

    public event Action InitGame;
    public event Action LoadLevel;
    public event Action ResetLevel;
    public event Action PauseGame;
    public event Action EndGame;
    

    public void PlayClicked()
    {
        ClickSound.Play();
        SwitchState(State.INIT, 0);
    }
    public void RestartClicked()
    {
        ClickSound.Play();
        _resetLevel = true;
        ResetAfterDeath();
    }
    public void MainMenuClicked()
    {
        ClickSound.Play();
        SwitchState(State.MENU, 0.5f);
    }

    public void ReturnClicked()
    {
        ClickSound.Play();
        SwitchState(State.PLAY, 0.2f);
    }
    public void ExitClicked()
    {
        ClickSound.Play();
        Debug.Log("Exitting...");
        Application.Quit();
    }



    void ResetAfterLvlComp()
    {
        ResetLevel();
        _oxygenLevelReset = true;
        _oxygenLevel = 1000;
        _highestDepth = 0;
        _timer = false;
        _keysNumber = 0;
        _chest = false;
    }

    void ResetAfterDeath()
    {
        ResetAfterLvlComp();
        _death = false;
        
    }

    void ResetAll()
    {
        ResetAfterDeath();
        _currentLevel = 1;
        _currentWinCondition = _winConditions[_currentLevel - 1];
        _startGame = false;
        LoadPanel.SetActive(false);
        GameOverPanel.SetActive(false);
        LevelCompPanel.SetActive(false);
        PauseGamePanel.SetActive(false);
        EndGamePanel.SetActive(false);
        GamePanel.SetActive(false);
        ChestInfo.SetActive(false);
        GameInfo.SetActive(false);
        _resetLevel = false;
        _pause = false;
        _highestDepth = 0;
        _gameInfoShowed = false;
    }

    void Start()
    {
        Instance = this;
        ResetAll();
        SwitchState(State.MENU, 0.5f);
    }



    public void SwitchState(State newState, float delay )
    {

        StartCoroutine(SwitchDelay(newState, delay));

    }

    IEnumerator SwitchDelay(State newState, float delay)
    {
        isSwitching = true;
        yield return new WaitForSecondsRealtime(delay);
        EndState();
        _state = newState;
        BeginState(newState);
        isSwitching = false;

    }

    void BeginState(State newState)
    {

        switch (newState)
        {
            case State.MENU:
                MenuPanel.SetActive(true);
                MenuMusic.Play();
                InitGame();
                if(SceneManager.sceneCount > 1)
                SceneManager.UnloadSceneAsync(_levels[_currentLevel - 1]);
                ResetAll();
               
                break;
            case State.INIT:
                _startGame = true;
                SwitchState(State.LOADLEVEL, 0);
                break;
            case State.PLAY:
                GameMusic.Play();
                if(!_gameInfoShowed)
                GameInfo.SetActive(true);
                _timer = false;
                break;
            case State.LEVELCOMP:
                _timer = false;
                if (_currentLevel == _levels.Length)
                    EndGamePanel.SetActive(true);
                else
                    LevelCompPanel.SetActive(true);
                SceneManager.UnloadSceneAsync(_levels[_currentLevel - 1]);
                _currentLevel++;
                _currentWinCondition = _winConditions[_currentLevel - 1];
                break;
            case State.LOADLEVEL:
                ResetAfterLvlComp();
                _timer = false;
                LoadPanel.SetActive(true);
                LoadLevel();
                if (_resetLevel)
                {
                    
                    SceneManager.UnloadSceneAsync(_levels[_currentLevel - 1]);
                }
                SceneManager.LoadScene(_levels[_currentLevel - 1], LoadSceneMode.Additive);
                GamePanel.SetActive(true);
                
                break;
            case State.PAUSE:
                Time.timeScale = 0;
                PauseGamePanel.SetActive(true);
                _pause = true;
           
                break;
            case State.GAMEOVER:
                ResetAfterDeath();
                GameOverPanel.SetActive(true);
                GamePanel.SetActive(false);
                EndGame();
                break;
        }

    }
    void Update()
    {

        if (!isSwitching)
        {
            switch (_state)
            {
                case State.MENU:
                    break;
                case State.INIT:
                    if (!_timer)
                        StartCoroutine(timeForLoad());
                    break;
                case State.PLAY:
                    PlayStateCode();
                   
                    break;
                case State.LEVELCOMP:
                    if (!_timer)
                        StartCoroutine(timeForLvlComp());
                    break;
                case State.LOADLEVEL:
                    Debug.Log("Load");
                    if (!_timer)
                        StartCoroutine(timeForLoad());
                    break;
                case State.PAUSE:
                    if (Input.GetButton("Cancel"))
                    {
                        SwitchState(State.PLAY, 0.2f);
                    }

                    if (_resetLevel)
                    {
                        SwitchState(State.LOADLEVEL, 0.2f);
                    }
                    break;
                case State.GAMEOVER:
                    if (_resetLevel)
                    {
                        SwitchState(State.LOADLEVEL, 0.2f);
                    }
                    break;
            }
        }

    }

    void EndState()
    {

        switch (_state)
        {
            case State.MENU:
                MenuPanel.SetActive(false);
                MenuMusic.Stop();
                _oxygenLevelReset = false;
                break;
            case State.INIT:
                break;
            case State.PLAY:
                GameMusic.Stop();
                break;
            case State.LEVELCOMP:
                LevelCompPanel.SetActive(false);
                break;
            case State.LOADLEVEL:
                LoadPanel.SetActive(false);
                _oxygenLevelReset = false;
                _resetLevel = false;
                Time.timeScale = 1;
                break;
            case State.PAUSE:
                PauseGamePanel.SetActive(false);
                Time.timeScale = 1;
                break;
            case State.GAMEOVER:
                GameOverPanel.SetActive(false);
                GamePanel.SetActive(true);
                break;
        }
    }

    void PlayStateCode()
    {
        if (!_timer)
            StartCoroutine(gameInfoTimer());

        if (Input.GetButton("Cancel"))
        {
            SwitchState(State.PAUSE, 0.1f);
        }
        if (_depthScore > _highestDepth)
            _highestDepth = _depthScore;

        if (_chest && _keysNumber < _currentWinCondition)
            ChestInfo.SetActive(true);
        else
            ChestInfo.SetActive(false);

        if (_keysNumber == _currentWinCondition && _chest)
            SwitchState(State.LEVELCOMP, 0.5f);

        if (_oxygenLevel == 0)
        {
            _death = true;
            if (diverAnimationController._endOfDeathAnim)
                SwitchState(State.GAMEOVER, 0.5f);
        }
        else
        {
            _death = false;
        }
    }

    IEnumerator timeForLoad()
    {
        _timer = true;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2);
        SwitchState(State.PLAY, 0.5f);


    }

    IEnumerator timeForLvlComp()
    {
        _timer = true;
        yield return new WaitForSecondsRealtime(2);
        SwitchState(State.LOADLEVEL, 0.5f);

    }

    IEnumerator gameInfoTimer()
    {
        _timer = true;
        yield return new WaitForSecondsRealtime(5);
        GameInfo.SetActive(false);
        _gameInfoShowed = true;
    }

}
