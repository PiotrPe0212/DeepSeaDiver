using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class seadevilController : MonoBehaviour
{
   
    private SpriteRenderer _fishRenderer;
    public float _xDistance = 10;
 public float _yDistance = 10;

 public bool _isclockDir = true;

 public float _waitTime = 1;

 public int _startPoint = 1;
 
public float _moveSpeed = 0.5f;
 float _initialXPos;
 float _actualXPos;

 float _actualXDistance;

 

 float _initialYPos;
 float _actualYPos;

 float _actualYDistance;




 float _direction;
 float _prevDirection;
     private Rigidbody2D _fish;
    private bool _visibility; 
    void Start()
    {
       _fish = GetComponent<Rigidbody2D>();
        _fishRenderer = GetComponent<SpriteRenderer>();
        _initialXPos = _fish.position.x;
       _initialYPos = _fish.position.y;
       _direction = _startPoint;
     
    }


    private void FixedUpdate() {

     
            _actualXPos = _fish.position.x;
            _actualYPos = _fish.position.y;
            _actualXDistance = Mathf.Abs(_actualXPos - _initialXPos);
            _actualYDistance = Mathf.Abs(_actualYPos - _initialYPos);

            if (_direction == 1)
            {
                _fishRenderer.flipX = true;
            }
            else if (_direction == 2)
            {
                _fishRenderer.flipX = false;
            }


            switch (_direction)
            {

                case 0:
                    //Wait
                    StartCoroutine(fishWaiting());
                    break;
                case 1:
                    //Right
                    transform.Translate(Vector2.right * _moveSpeed);
                    if (_actualXDistance >= _xDistance)
                    {
                        _prevDirection = _direction;
                        _direction = 0;

                    }
                    break;
                case 2:
                    //Left
                    transform.Translate(Vector2.left * _moveSpeed);
                    if (_actualXDistance >= _xDistance)
                    {
                        _prevDirection = _direction;
                        _direction = 0;

                    }
                    break;
                case 3:
                    //Up
                    transform.Translate(Vector2.up * _moveSpeed);
                    if (_actualYDistance >= _yDistance)
                    {
                        _prevDirection = _direction;
                        _direction = 0;

                    }
                    break;
                case 4:
                    //Down
                    transform.Translate(Vector2.down * _moveSpeed);
                    if (_actualYDistance >= _yDistance)
                    {
                        _prevDirection = _direction;
                        _direction = 0;

                    }
                    break;
            }
        
    }
    


    IEnumerator fishWaiting()
    {
        yield return new WaitForSeconds(_waitTime);
        _initialXPos = _fish.position.x;
       _initialYPos = _fish.position.y;       
switch (_prevDirection){
case 1:
if(_isclockDir)
{_direction = 4;}
else
{_direction = 3;}
break;
case 2:
 if(_isclockDir)
{_direction = 3;}
else
{_direction = 4;}
break;
case 3:
if(_isclockDir)
{_direction = 1;}
else
{_direction = 2;}
break;
case 4:
if(_isclockDir)
{_direction = 2;}
else
{_direction = 1;}
break;

}
    
    }


}
