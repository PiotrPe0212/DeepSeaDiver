using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hotSteam : MonoBehaviour
{
    public int _steamOnTime = 2;
    public int _steamOffTime = 4;
    public LayerMask _layerToDetect;

    ParticleSystem _particleSys;

    bool _steamOn;
    bool _coroutineOn;
    bool _coroutineOff;
    private bool _diverDetected;

    bool _isHited;
    private Collider2D _detectingBox;
    private Vector2 _detectVector;
    private Vector2 _detectScaleVector;

    void Start()
    {
        _particleSys = GetComponent<ParticleSystem>();
        _steamOn = false;
        _coroutineOn = true;
        _coroutineOff = false;
        _isHited = false;
        _detectVector = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 2);
        _detectScaleVector = new Vector2(gameObject.transform.localScale.x * 0.9f, gameObject.transform.localScale.y * 2);
        _detectingBox = Physics2D.OverlapBox(_detectVector, _detectScaleVector, 0, _layerToDetect, -1, 3);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_coroutineOn)
            StartCoroutine(HotSteamTimerOn());
        if (_coroutineOff)
            StartCoroutine(HotSteamTimerOff());
        DiverDetection();
    }
    IEnumerator HotSteamTimerOn()
    {
        if (_coroutineOn)
        {

            var em = _particleSys.emission;
            em.enabled = true;
            em.rateOverTime = 100f;
            _steamOn = true;
            _coroutineOn = false;
        }
        yield return new WaitForSeconds(_steamOnTime);
        _coroutineOff = true;


    }
    IEnumerator HotSteamTimerOff()
    {
        if (_coroutineOff)
        {

            var em = _particleSys.emission;
            em.enabled = true;
            em.rateOverTime = 5f;
            _steamOn = false;
            _coroutineOff = false;
        }
        yield return new WaitForSeconds(_steamOffTime);
        _coroutineOn = true;

    }

    IEnumerator WaitAfterHit()
    {
        _isHited = true;
        yield return new WaitForSeconds(0.5f);
        _isHited = false;
    }

    void DiverDetection()
    {
        _detectVector = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 2);
        _detectScaleVector = new Vector2(gameObject.transform.localScale.x * 0.9f, gameObject.transform.localScale.y * 2);
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

        Debug.Log(_diverDetected);
        if (_diverDetected && _steamOn)
        {
            Debug.Log("hittttt");
            if (!_isHited)
            {
                OxygenCounter.Instance.Damage = true;
                StartCoroutine(WaitAfterHit());
            }

        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (true)
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(_detectVector, _detectScaleVector);
    }


}
