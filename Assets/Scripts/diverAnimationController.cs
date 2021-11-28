using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class diverAnimationController : MonoBehaviour
{
    [SerializeField] private Diver _diver;
    private Animator _animator;
    private SpriteRenderer diverRenderer;

    private bool _isGrounded;
    private bool _isUpPressed;
    private float _horizontalMove;
    private float _isVerticalMove;
    private bool _jetpackUse;
    private bool _endOfAnimation = false;
    private bool _bigHight;
    public static bool _startUp;
    public static bool _endOfHitGroundAnim;
    public static bool _endOfDeathAnim = false;
    private bool _deathSet;
    private bool _reset;
    public AudioSource _stepAudio;
    public AudioSource _fallHitAudio;
    public enum AnimationState { WAITING, WALKING, FALLINIT, FALLING, JUMP, UP, GROUNDHIT, JETPACK, DEATH }
    AnimationState _state;
    private bool isSwitching;
    private bool _startJumpAnimation = false;
    public event Action AnimationEnded;
    void Start()
    {

        diverRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        SwitchState(AnimationState.WAITING);
    }

    private void Awake()
    {
        _diver.Jump += JumpAnimation;
    }

    private void OnDestroy()
    {
        _diver.Jump += JumpAnimation;
    }

    public void SwitchState(AnimationState newState, float delay = 0)
    {
        StartCoroutine(SwitchDelay(newState, delay));
    }

    IEnumerator SwitchDelay(AnimationState newState, float delay)
    {
        isSwitching = true;
        yield return new WaitForSeconds(delay);
        EndState();
        _state = newState;
        BeginState(newState);
        isSwitching = false;

    }

    void BeginState(AnimationState newState)
    {

        switch (newState)
        {
            case AnimationState.WAITING:
                _animator.Play("wait");
                break;

            case AnimationState.WALKING:
                _animator.Play("walk");
                _stepAudio.Play();
                break;

            case AnimationState.FALLINIT:
                _animator.Play("downInit");
                break;

            case AnimationState.FALLING:
                _animator.Play("fallingDown");
                break;

            case AnimationState.GROUNDHIT:
                
                    _animator.Play("afterFall");
                _fallHitAudio.PlayOneShot(_fallHitAudio.clip, 1);
                break;

            case AnimationState.JUMP:
                _endOfHitGroundAnim = false;
                _animator.Play("Jump");
                break;

            case AnimationState.UP:
                _animator.Play("goingUp");
                break;

            case AnimationState.JETPACK:
                _animator.Play("jetpack");
                break;

            case AnimationState.DEATH:
                if(_deathSet)
                _animator.Play("death");
                else
                _animator.Play("wait");
                break;
        }

    }


    private void FixedUpdate()
    {
        _isUpPressed = Diver._jumping;
        _isGrounded = Diver.IsGrounded;
        _horizontalMove = Diver.HorizontalMove;
        _isVerticalMove = Diver.VerticalMove;
        _jetpackUse = Diver._jetpackUse;
        _bigHight = Diver.WillGroundHitAnim;
        _deathSet = GameObject.Find("GameManager").GetComponent<GameManager>()._death;
        _reset = GameObject.Find("GameManager").GetComponent<GameManager>()._resetLevel;
        if (_horizontalMove == -1)
        {
            diverRenderer.flipX = true;
        }
        else if (_horizontalMove == 1)
        {
            diverRenderer.flipX = false;
        }


        if (!isSwitching)
        {
            switch (_state)
            {
                case AnimationState.WAITING:
                    if (_isGrounded && _horizontalMove != 0)
                    {
                        SwitchState(AnimationState.WALKING);
                    }

                    if (_startJumpAnimation)
                    {
                        SwitchState(AnimationState.JUMP);
                    }

                    if (_deathSet)
                        SwitchState(AnimationState.DEATH);

                    break;

                case AnimationState.WALKING:


                    if (_horizontalMove == 0)
                    {
                        SwitchState(AnimationState.WAITING);
                    }

                    if (!_isGrounded  && _isVerticalMove <= 0 && _bigHight)
                    {
                        SwitchState(AnimationState.FALLINIT);
                    }

                    if (_startJumpAnimation)
                    {
                        SwitchState(AnimationState.JUMP);
                    }

                    if (_deathSet)
                        SwitchState(AnimationState.DEATH);

                    break;

                case AnimationState.FALLINIT:
                    if (_endOfAnimation)
                    {
                        SwitchState(AnimationState.FALLING);
                    }
                    break;

                case AnimationState.FALLING:
                    if (_isGrounded && _bigHight)
                    {
                        SwitchState(AnimationState.GROUNDHIT);
                    }
                    else if(_isGrounded &&  !_bigHight)
                        SwitchState(AnimationState.WAITING);

                    if (_startJumpAnimation)
                    {
                        SwitchState(AnimationState.JUMP);
                    }

                    if (_deathSet)
                        SwitchState(AnimationState.DEATH);

                    break;

                case AnimationState.GROUNDHIT:
                    if (_endOfAnimation && _horizontalMove == 0)
                    {
                        SwitchState(AnimationState.WAITING);
                    }
                    else if (_endOfAnimation && _horizontalMove != 0)
                    {
                        SwitchState(AnimationState.WALKING);
                    }

                    if (_deathSet)
                        SwitchState(AnimationState.DEATH);

                    break;


                case AnimationState.JUMP:
                    if (_endOfAnimation)
                    {
                        SwitchState(AnimationState.UP);
                    }
                    break;

                case AnimationState.UP:
                    if (!_isGrounded  && !_isUpPressed )
                    {
                        SwitchState(AnimationState.FALLINIT);
                    }
                    if (_jetpackUse)
                        SwitchState(AnimationState.JETPACK);
                    if (_isGrounded && _horizontalMove == 0)
                        SwitchState(AnimationState.WAITING);
                    else if (_isGrounded && _horizontalMove != 0)
                        SwitchState(AnimationState.WALKING);

                    if (_deathSet)
                        SwitchState(AnimationState.DEATH);

                    break;

                case AnimationState.JETPACK:
                    if (!_jetpackUse)
                        SwitchState(AnimationState.FALLINIT);

                    if (_deathSet)
                        SwitchState(AnimationState.DEATH);

                    break;

                case AnimationState.DEATH:

                    if (_endOfAnimation)
                        _endOfDeathAnim = true;
                    
                    if(!_deathSet)
                        SwitchState(AnimationState.WAITING);
                    break;
            }
        }
    }

    void EndState()
    {
        switch (_state)
        {
            case AnimationState.WAITING:

                break;

            case AnimationState.WALKING:
                _stepAudio.Stop();

                break;

            case AnimationState.FALLINIT:
                _endOfAnimation = false;
                _startUp = false;
                break;

            case AnimationState.FALLING:

                break;

            case AnimationState.GROUNDHIT:
                _endOfAnimation = false;
                _endOfHitGroundAnim = true;
                break;


            case AnimationState.JUMP:
                _startJumpAnimation = false;
                _endOfAnimation = false;
                _startUp = false;
                break;

            case AnimationState.UP:

                break;

            case AnimationState.JETPACK:
                break;

            case AnimationState.DEATH:
                _endOfDeathAnim = false;
                _endOfAnimation = false;
                break;
        }

    }

    void EndOfAnimation()
    {
        _endOfAnimation = true;
        AnimationEnded();

    }

    private void JumpAnimation()
    {
        _startJumpAnimation = true;
    }
}
