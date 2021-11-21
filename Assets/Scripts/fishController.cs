using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class fishController : MonoBehaviour
{
    public AudioSource fishSound;
    private Animator _animator;
    private SpriteRenderer _fishRenderer;
    public GameObject fishLight;
    public Camera Cam;
    public float _xDistance = 10;
    public float _yDistance = 10;
    float _staticForceValue = 98;
    float _moveForceValue = 3;
    float _initialXPos;
    float _actualXPos;

    float _actualXDistance;
    bool _biteOn;


    float _initialYPos;
    float _actualYPos;

    float _actualYDistance;

    float _upCounter;
    float _downCounter;
    float _rightCounter;
    float _leftCounter;

    int[] directionArray = new int[] { 0, 1, 2, 3, 4 };
    public float _moveZoneSize = 2;

    float _direction;
    private float _horizontalMove;
    private Rigidbody2D _fish;
    private bool _visibility;
    void Start()
    {
        _fish = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _fishRenderer = GetComponent<SpriteRenderer>();
        _initialXPos = _fish.position.x;
        _initialYPos = _fish.position.y;
        _direction = 0;
        _leftCounter = 0;
        _rightCounter = 0;
        _upCounter = 0;
        _downCounter = 0;
        _biteOn = false;
    }


    private void FixedUpdate()
    {

        SoundControll();
        if (_fishRenderer.IsVisibleFrom(Cam))
            _visibility = true;
        else _visibility = false;
        if (_visibility)
        {

            if (_direction == 1)
            {
                _fishRenderer.flipX = true;
                fishLight.transform.position = transform.position + new Vector3(1.63f, 0.5f, 0);
            }
            else if (_direction == 2)
            {
                _fishRenderer.flipX = false;
                fishLight.transform.position = transform.position + new Vector3(-1.63f, 0.5f, 0);
            }


            _fish.AddForce(Vector2.up * _staticForceValue, ForceMode2D.Force);
            StartCoroutine(fishFloating());
            _actualXPos = _fish.position.x;
            _actualYPos = _fish.position.y;
            _actualXDistance = Mathf.Abs(_actualXPos - _initialXPos);
            _actualYDistance = Mathf.Abs(_actualYPos - _initialYPos);
            if (_animator.name == "bigFish")
            {
                if (!_biteOn)
                    _animator.Play("bigfishswim");
                if (_biteOn)
                    _animator.Play("bigfishbite");
            }
            else
            {
                if (!_biteOn)
                    _animator.Play("smallFishSwim");
                if (_biteOn)
                    _animator.Play("smallFishBite");
            }
            switch (_direction)
            {

                case 0:
                    //Wait
                    StartCoroutine(fishWaiting());
                    break;
                case 1:
                    //Right
                    if (_rightCounter == _moveZoneSize)
                    {
                        fishDirectionChangeY();
                    }
                    else
                    {
                        _fish.AddForce(Vector2.right * _moveForceValue, ForceMode2D.Force);

                        if (_actualXDistance >= _xDistance)
                        {
                            _rightCounter++;
                            if (_leftCounter > 0)
                            { _leftCounter--; }
                            fishDirectionChangeY();

                        }
                    }
                    break;
                case 2:
                    //Left
                    if (_leftCounter == _moveZoneSize)
                    {
                        fishDirectionChangeY();
                    }
                    else
                    {
                        _fish.AddForce(Vector2.left * _moveForceValue, ForceMode2D.Force);


                        if (_actualXDistance >= _xDistance)
                        {
                            _leftCounter++;
                            if (_rightCounter > 0)
                            { _rightCounter--; }
                            fishDirectionChangeY();
                        }
                    }
                    break;
                case 3:
                    //Up
                    if (_upCounter >= _moveZoneSize)
                    {
                        fishDirectionChangeX();
                    }
                    else
                    {
                        _fish.AddForce(Vector2.up * _moveForceValue, ForceMode2D.Force);

                        if (_actualYDistance >= _yDistance)
                        {
                            _upCounter++;
                            if (_downCounter > 0)
                            { _downCounter--; }
                            fishDirectionChangeX();
                        }
                    }
                    break;
                case 4:
                    //Down
                    if (_downCounter == _moveZoneSize)
                    {
                        fishDirectionChangeX();
                    }
                    else
                    {
                        _fish.AddForce(Vector2.down * _moveForceValue, ForceMode2D.Force);

                        if (_actualYDistance >= _yDistance)
                        {
                            _downCounter++;
                            if (_upCounter > 0)
                            { _upCounter--; }
                            fishDirectionChangeX();
                        }
                    }
                    break;

            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //_upCounter = 0;
        //_downCounter = 0;
        //_leftCounter = 0;
        //_rightCounter = 0;

        switch (_direction)
        {
            case 1:
                _direction = 2;
                break;
            case 2:
                _direction = 1;
                break;
            case 3:
                _direction = 4;
                break;
            case 4:
                _direction = 3;
                break;
        }
        if (collision.gameObject.name == "diver")
        {
            OxygenCounter.Instance.Damage = true;
            _biteOn = true;
        }

    }
    void OnCollisionExit2D(Collision2D collision)
    {
        _biteOn = false;
    }

    IEnumerator fishFloating()
    {
        yield return new WaitForSeconds(1);
        if (_staticForceValue == 98.2f)
        {
            _staticForceValue = 98;

        }
        else if (_staticForceValue == 98)
        {
            _staticForceValue = 98.2f;

        }

    }

    IEnumerator fishWaiting()
    {
        yield return new WaitForSeconds(1);
        fishDirectionChangeY();

    }

    static int RandomMove()
    {
        return Random.Range(0, 5);
    }

    void fishDirectionChange()
    {

        int random = Random.Range(0, 3);

        _initialXPos = _fish.position.x;
        _initialYPos = _fish.position.y;

        if (_direction != random)
        {
            _direction = random;
        }
        else if (random == 4)
        {
            _direction = random - 1;
        }
        else
        {
            _direction = random + 1;
        }



    }

    void fishDirectionChangeX()
    {

        int random = Random.Range(1, 3);

        _initialXPos = _fish.position.x;
        _initialYPos = _fish.position.y;
        _direction = random;


    }

    void fishDirectionChangeY()
    {

        int random = Random.Range(3, 5);

        _initialXPos = _fish.position.x;
        _initialYPos = _fish.position.y;
        _direction = random;


    }

    void SoundControll()
    {
        if (_biteOn)
            fishSound.pitch = 3;
        else
            fishSound.pitch = 0.8f;
    }

}
