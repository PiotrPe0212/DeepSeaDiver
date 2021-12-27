using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyHook : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    public static keyHook Instance { get; private set; }
    public Sprite[] spriteArray;
    SpriteRenderer spriteRenderer;

    private int _keysNumber;
    public int KeyGetting
    {
        get { return _keysNumber; }
        set
        {
            _keysNumber += value;
        }
    }

    private void Awake()
    {
        _gameManager.LoadLevel += ResetKeys;
        _gameManager.ResetLevel += ResetKeys;
        _gameManager.EndGame += ResetKeys;

    }

    private void OnDestroy()
    {
        _gameManager.LoadLevel -= ResetKeys;
        _gameManager.ResetLevel -= ResetKeys;
        _gameManager.EndGame -= ResetKeys;

    }
    void Start()
    {
        Instance = this;
        _keysNumber = 0;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sprite = spriteArray[_keysNumber];
    }

    private void ResetKeys()
    {
        _keysNumber = 0;
    }
}
