using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class murenaController : MonoBehaviour
{

   
    private Animator _animator;

    private Collider2D _detectingBox;
    private Vector2 _detectVector;

    private Vector2 _detectScaleVector;

    private bool _diverDetected;

    private bool _attackState;
    private bool _endAttackState;

    private bool _colisionDetected;

    private float _initialPos;
    private float _actualPos;
    private float _attacPos;
    private float _direction;
    // right
    // left  z180
    // up z90
    // down z270

    public GameObject _murenaEyeLight;

    public LayerMask _layerToDetect;

    public float attacDistance = 10;
   

    // Start is called before the first frame update
    void Start()
    {
        
        _animator = GetComponent<Animator>();
        _diverDetected = false;
        _direction = transform.eulerAngles.z; 
        if(_direction == 0 || _direction == 180)
        {
            _initialPos = transform.position.x;
            if (_direction == 180)
                _attacPos = _initialPos - attacDistance;
        else
                _attacPos = _initialPos + attacDistance;

           
        }
        else
        {
            _initialPos = transform.position.y;

            if (_direction == 270 )
                _attacPos = _initialPos - attacDistance;
            else
                _attacPos = _initialPos + attacDistance;

            
        }
           

        
        _attackState = false;
        _endAttackState = false;
        _colisionDetected = false;
      
        _murenaEyeLight.SetActive(false);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        diverDetection();

        
        if (_diverDetected && !_attackState && !_endAttackState )
        {
            StartCoroutine(murenaWaiting());
        }

        if (_attackState)
        {
            attacFunction();
        }
        if (_endAttackState)
        {
            comeBackFunction();
        }
        if(!_attackState && !_endAttackState)
            _animator.Play("murenaWait");

    }


    IEnumerator murenaWaiting()
    {
        
        _murenaEyeLight.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        _attackState = true;
        _diverDetected = false;

    }
     
    void attacFunction()
    {
       
        if ((((_actualPos < _attacPos) && (_direction == 0 || _direction == 90))
            || ((_actualPos > _attacPos) && (_direction == 180 || _direction == 270)))
            && !_colisionDetected)
        {
            if(_direction == 0)
            transform.position = transform.position + new Vector3(0.1f, 0, 0);
            else if (_direction == 90)
                transform.position = transform.position + new Vector3(0, 0.1f, 0);
            else if (_direction == 180)
                transform.position = transform.position + new Vector3(-0.1f, 0, 0);
            else if(_direction == 270)
                transform.position = transform.position + new Vector3(0, -0.1f, 0);
            _animator.Play("murenaSwim");
          
        }
        else if (((_actualPos > _attacPos) && (_direction == 0 || _direction == 90))
            || ((_actualPos < _attacPos) && (_direction == 180 || _direction == 270)))
        {
            _endAttackState = true;
            _attackState = false;
        }
        else if (_colisionDetected)
        {
            if (!_endAttackState)
                StartCoroutine(attacWait());
        }
       


        Debug.Log(_endAttackState + "attacstate");
    }

    void comeBackFunction()
    {
        if (((_actualPos > _initialPos) && (_direction == 0 || _direction ==90))
            || ((_actualPos < _initialPos) && (_direction == 180 || _direction == 270)))
        {
            if (_direction == 0)
                transform.position = transform.position - new Vector3(0.1f, 0, 0);
            else if (_direction == 90)
                transform.position = transform.position - new Vector3(0, 0.1f, 0);
            else if (_direction == 180)
                transform.position = transform.position + new Vector3(0.1f, 0, 0);
            else if (_direction == 270)
                transform.position = transform.position + new Vector3(0, 0.1f, 0);
            _animator.Play("murenaSwim");
        }
        else
        {
            _endAttackState = false;
            _colisionDetected = false;
            _murenaEyeLight.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.name == "diver")
        {
            _colisionDetected = true;
           
            
            Debug.Log("murenahit");
        }
    }


    void diverDetection()
    {

        if (_direction == 0 || _direction == 180)
        {
            _detectScaleVector = new Vector2(gameObject.transform.localScale.x * 2, gameObject.transform.localScale.y * 0.9f);
            _actualPos = transform.position.x;
            if(_direction ==0)
            _detectVector = new Vector2(gameObject.transform.position.x + 2.2f, gameObject.transform.position.y);
            else
                _detectVector = new Vector2(gameObject.transform.position.x - 2.2f, gameObject.transform.position.y);
        }
        else if (_direction == 90 || _direction == 270)
        {
            
            _detectScaleVector = new Vector2(gameObject.transform.localScale.x * 0.8f, gameObject.transform.localScale.y * 2);
            _actualPos = transform.position.y;
            if(_direction == 90)
                _detectVector = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 2.4f);
            else
                _detectVector = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 2.4f);

        }
        _detectingBox = Physics2D.OverlapBox(_detectVector, _detectScaleVector, 0, _layerToDetect, -1, 3);
        if (_detectingBox != null)
        {

            Debug.Log(_detectingBox.name);
            if (_detectingBox.name == "diver")
            {
                _diverDetected = true;
            }
        }
        else
        {
            _diverDetected = false;
        }

    }

    IEnumerator attacWait()
    {
        _animator.Play("murenaBite");
        yield return new WaitForSeconds(1);
        _endAttackState = true;
        _attackState = false;
    }
    
}
