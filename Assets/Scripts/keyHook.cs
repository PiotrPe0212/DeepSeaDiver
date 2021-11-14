using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyHook : MonoBehaviour
{
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
}
