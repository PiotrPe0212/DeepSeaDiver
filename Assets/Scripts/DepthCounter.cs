using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthCounter : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    private GameObject player;


    private float depthCounter;
    private float prevDepthCounterVal;


    private GameObject[] _bulbesThousands;

    private GameObject[] _bulbesHundreds;

    private GameObject[] _bulbesTens;

    private GameObject[] _bulbesUnits;

    private string _counterString;
    private string _numberString;
    private int _thousandDigit;
    private int _thousandDigitPrev = 0;
    private int _hundredDigit;
    private int _hundredDigitPrev = 0;
    private int _tenDigit;
    private int _tenDigitPrev = 0;
    private int _unitDigit;
    private int _unitDigitPrev = 0;

    private bool _startGame;

    void Start()
    {
        
        _bulbesThousands = GameObject.FindGameObjectsWithTag("BulbThousands");
        _bulbesHundreds = GameObject.FindGameObjectsWithTag("BulbHundreds");
        _bulbesTens = GameObject.FindGameObjectsWithTag("BulbTens");
        _bulbesUnits = GameObject.FindGameObjectsWithTag("BulbUnits");
     
        bulbesReset(_bulbesThousands);
        bulbesReset(_bulbesHundreds);
        bulbesReset(_bulbesTens);
        bulbesReset(_bulbesUnits);

    }
    private void Awake()
    {
        _gameManager.InitGame += ResetCounter;
        _gameManager.ResetLevel += ResetCounter;
        _gameManager.EndGame += ResetCounter;
        
    }

    private void OnDestroy()
    {
        _gameManager.InitGame -= ResetCounter;
        _gameManager.ResetLevel -= ResetCounter;
        _gameManager.PauseGame -= ResetCounter;
    }
    
    void FixedUpdate()
    {
       
        player = GameObject.Find("diver");
        if (player)
        {
            _startGame = GameObject.Find("GameManager").GetComponent<GameManager>()._startGame;
            if (_startGame)
            {
                if (player.transform.position.y < 0)
                    depthCounter = Mathf.Abs(Mathf.Round(player.transform.position.y));
                else
                    depthCounter = 0;

                GameManager.Instance.DepthScore = depthCounter;
                getDecimals();
                numberChange();

                prevDepthCounterVal = depthCounter;

            }
        }
    }

    void getDecimals()
    {
        _counterString = depthCounter.ToString();
       
        switch (_counterString.Length)
        {

            case 1:
                
                _thousandDigit = 0;
                _hundredDigit = 0;
                _tenDigit = 0;
                _unitDigit = stringToDigit(0, 1);
               
                break;

            case 2:
                
                _thousandDigit = 0;
                _hundredDigit = 0;
                _tenDigit = stringToDigit(0, 1);
                _unitDigit = stringToDigit(1, 1);
                break;

            case 3:
                _thousandDigit = 0;
                _hundredDigit = stringToDigit(0, 1);
                _tenDigit = stringToDigit(1, 1);
                _unitDigit = stringToDigit(2, 1);
                break;

            case 4:
                _thousandDigit = stringToDigit(0, 1);
                _hundredDigit = stringToDigit(1, 1);
                _tenDigit = stringToDigit(2, 1);
                _unitDigit = stringToDigit(3, 1);
                break;
        }

    }

    void cleanPrevNumber(int type, int number)
    {
        switch (type)
        {
            case 1:
                _bulbesThousands[number].SetActive(false);
                break;
            case 2:
                _bulbesHundreds[number].SetActive(false);
                break;
            case 3:
                _bulbesTens[number].SetActive(false);
                break;
            case 4:
                _bulbesUnits[number].SetActive(false);
                break;

        }

    }

    void setNewNumber(int type, int number)
    {
        switch (type)
        {
            case 1:
                _bulbesThousands[number].SetActive(true);
                break;
            case 2:
                _bulbesHundreds[number].SetActive(true);
                break;
            case 3:
                _bulbesTens[number].SetActive(true);
                break;
            case 4:
                _bulbesUnits[number].SetActive(true);
                break;

        }

    }

    void numberChange()
    {
        if (_thousandDigit != _thousandDigitPrev)
        {
            cleanPrevNumber(1, _thousandDigitPrev);
            setNewNumber(1, _thousandDigit);
            _thousandDigitPrev = _thousandDigit;
        }
        if (_hundredDigit != _hundredDigitPrev)
        {
            cleanPrevNumber(2, _hundredDigitPrev);
            setNewNumber(2, _hundredDigit);
            _hundredDigitPrev = _hundredDigit;
        }

        if (_tenDigit != _tenDigitPrev)
        {
            cleanPrevNumber(3, _tenDigitPrev);
            setNewNumber(3, _tenDigit);
            _tenDigitPrev = _tenDigit;
        }
        if (_unitDigit != _unitDigitPrev)
        {
            cleanPrevNumber(4, _unitDigitPrev);
            setNewNumber(4, _unitDigit);
            _unitDigitPrev = _unitDigit;
        }
    }
    void bulbesReset(GameObject[] Bulbes)
    {
      
        for (int i = 0; i < Bulbes.Length; i++)
        {

            Bulbes[i].SetActive(false);
        }
        Bulbes[0].SetActive(true);

    }

    private void ResetCounter()
    {
        
    }
    int stringToDigit(int digitNumberStart, int digitNumberEnd)
    {
        int outNumber;

        _numberString = _counterString.Substring(digitNumberStart, digitNumberEnd);
        int.TryParse(_numberString, out outNumber);
        return outNumber;

    }
}
