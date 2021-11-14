using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenCounter : MonoBehaviour
{
    // Start is called before the first frame update

    float oxygenTimer;
    public float roundOxygenTimer;
    public float maxOxygenAngle = -110;
    public float minOxygenAngle = 60;
    public float _damageValue = 10;
    public float _addOxygenValue = 10;
    public GameObject _hitScreen;
    public GameObject clockHandPivot;
    public GameObject _jetPackUseBulb;
    public GameObject _jetPackCooldownBulb;
    private float _totalAngle;
    private float _timeToAngleProportion;
    private float _actualAngle;
    
    public static OxygenCounter Instance { get; private set; }
    private bool _jetpackUse;
    public bool Jetpack
    {
        get { return _jetpackUse; }
        set
        {
            _jetpackUse = value;

        }
    }
    private bool _hitDamage;
    public bool Damage
    {
        get { return _hitDamage; }
        set
        {
            _hitDamage = value;
            Debug.Log("HIT");

        }
    }

    private bool _addOxygen;
    private bool _startGame;
    private GameManager.State _state;
    private bool _oxygenLevelReset;
    public bool AddOxygen
    {
        get { return _addOxygen; }
        set
        {
            _addOxygen = value;

        }
    }
    float _moreOxygenUse;


    [SerializeField]


 
    void Start()
    {
        Instance = this;
        oxygenTimer = 1000;
        Instance = this;
        clockHandPivot.transform.eulerAngles = new Vector3(0, 0, maxOxygenAngle);
        _totalAngle = Mathf.Abs(minOxygenAngle) + Mathf.Abs(maxOxygenAngle);
        _timeToAngleProportion = _totalAngle / oxygenTimer;
        _actualAngle = maxOxygenAngle;
        _oxygenLevelReset = false;
      
      _hitScreen.SetActive(false);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _startGame = GameObject.Find("GameManager").GetComponent<GameManager>()._startGame;
        _oxygenLevelReset = GameObject.Find("GameManager").GetComponent<GameManager>()._oxygenLevelReset;
        _state = GameObject.Find("GameManager").GetComponent<GameManager>()._state;




        Debug.Log(_jetpackUse);
        if (_state != GameManager.State.PLAY)
        {
            if (_state != GameManager.State.PAUSE)
            {
                oxygenTimer = 1000;
                _actualAngle = maxOxygenAngle;
            }
        }
        else
        {
            if (_hitDamage)
                StartCoroutine(hitTimer());

            if (_jetpackUse)
            {
                _moreOxygenUse = 10;
            }
            else
            {
                _moreOxygenUse = 1;
            }
            if (oxygenTimer > 0)
            {

                oxygenTimer -= Time.deltaTime * _moreOxygenUse;
                _actualAngle += _timeToAngleProportion * Time.deltaTime * _moreOxygenUse;
                if (_hitDamage)
                {
                    oxygenTimer -= _damageValue;
                    _actualAngle += _timeToAngleProportion * _damageValue;
                    _hitDamage = false;
                    Debug.Log(oxygenTimer);
                }
                if (_addOxygen)
                {
                    oxygenTimer += _addOxygenValue;
                    _actualAngle -= _timeToAngleProportion * _addOxygenValue;
                    _addOxygen = false;
                }

            }
            roundOxygenTimer = Mathf.Round(oxygenTimer * 10.0f) * 0.1f;
            if (oxygenTimer <= 0)
            {
                roundOxygenTimer = 0;
                _actualAngle = minOxygenAngle;
            }
            GameManager.Instance.OxygenLevel = roundOxygenTimer;
            clockHandPivot.transform.eulerAngles = new Vector3(0, 0, _actualAngle);

            bulbControll();
        }

        

        IEnumerator hitTimer() {
            _hitScreen.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            _hitScreen.SetActive(false);
        }

        void bulbControll()
        {
            if (Diver._jetpackCooldownState)
                _jetPackCooldownBulb.SetActive(true);
            else
                _jetPackCooldownBulb.SetActive(false);

            if (_jetpackUse)
                _jetPackUseBulb.SetActive(true);
            else
                _jetPackUseBulb.SetActive(false);
        }
      


    }
}
