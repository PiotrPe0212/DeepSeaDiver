using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class diverAnimationController : MonoBehaviour
{
    [SerializeField] private Diver _diver;
    private Animator _animator;
    private SpriteRenderer diverRenderer;

    public bool _isGrounded;
    private float _horizontalMove;
    public float _isVerticalMove;
   [SerializeField] private bool _jetpackUse;
    public bool _bigHight;
    public static bool _endOfHitGroundAnim;
    public static bool _endOfDeathAnim = false;
    private bool _deathSet;
    private bool _edgeCaught;
    private bool _jumped;
    public AudioSource _stepAudio;
    public AudioSource _fallHitAudio;
    public enum AnimationState { WAITING, WALKING, FALLINIT, FALLING, JUMP, UP, GROUNDHIT, JETPACK, DEATH, CLIMB }
    AnimationState _state;
    private bool isSwitching;
    private bool _startJumpAnimation = false;
    private bool _endOfJumpAnimation = false;
    private bool _endOfFallinitAnimation = false;
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
        _diver.Jump -= JumpAnimation;
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
                _diver.ResetAfterJump();
                _jumped = false;
                break;

            case AnimationState.WALKING:
                _animator.Play("walk");
                _diver.ResetAfterJump();
                _jumped = false;
                _stepAudio.Play();
                break;

            case AnimationState.FALLINIT:
                _endOfFallinitAnimation= false;
                _animator.Play("downInit");
                break;

            case AnimationState.FALLING:
                _animator.Play("fallingDown");
                break;

            case AnimationState.CLIMB:
                _animator.Play("Climb");
                break;
            case AnimationState.GROUNDHIT:
                
                    _animator.Play("afterFall");
                _fallHitAudio.PlayOneShot(_fallHitAudio.clip, 1);
                break;

            case AnimationState.JUMP:
                _endOfJumpAnimation = false;
                _animator.Play("Jump");
                break;

            case AnimationState.UP:
                _animator.Play("goingUp");
                break;

            case AnimationState.JETPACK:
                _animator.Play("jetpack");
                break;

            case AnimationState.DEATH:
                _endOfDeathAnim = false;
                if(_deathSet)
                _animator.Play("death");
                else
                _animator.Play("wait");
                break;
        }

    }


    private void FixedUpdate()
    {
        Debug.Log(_state);
        _isGrounded = Diver.IsGrounded;
        _horizontalMove = Diver.HorizontalMove;
        _isVerticalMove = Diver.VerticalPos;
        _jetpackUse = Diver._jetpackUse;
        _bigHight = Diver.WillGroundHitAnim;
        _edgeCaught = Diver.EdgeCaught;
        _deathSet = GameObject.Find("GameManager").GetComponent<GameManager>()._death;
       
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
                    UpdateWait();

                    break;

                case AnimationState.WALKING:
                    UpdateWalk();
                    break;

                case AnimationState.FALLINIT:
                    if (_endOfFallinitAnimation)
                    {
                        SwitchState(AnimationState.FALLING);
                    }
                    break;

                case AnimationState.CLIMB:
                    if(!_edgeCaught)
                        SwitchState(AnimationState.WAITING);
                    break;
                case AnimationState.FALLING:
                   UpdateFall();

                    break;

                case AnimationState.GROUNDHIT:
                   UpdateGroudHit();
                    break;


                case AnimationState.JUMP:
                    if (_endOfJumpAnimation)
                    {
                        SwitchState(AnimationState.UP);
                    }
                    break;

                case AnimationState.UP:
                   UpdateUp();

                    break;

                case AnimationState.JETPACK:
                    if (!_jetpackUse)
                        SwitchState(AnimationState.FALLINIT);

                    if (_edgeCaught)
                        SwitchState(AnimationState.CLIMB);
                    
                    if (_deathSet)
                        SwitchState(AnimationState.DEATH);

                    break;

                case AnimationState.DEATH:

                    if(!_deathSet)
                        SwitchState(AnimationState.WAITING);
                    break;
            }
        }
    }

   private void UpdateWait()
    {
        if (_isGrounded && _horizontalMove != 0)
        {
            SwitchState(AnimationState.WALKING);
        }

        if (_startJumpAnimation)
        {
            SwitchState(AnimationState.JUMP);
        }

        if (_jetpackUse)
            SwitchState(AnimationState.JETPACK);

        if (_deathSet)
            SwitchState(AnimationState.DEATH);
    }

   private void UpdateWalk()
   {
       
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

        if (_jetpackUse)
            SwitchState(AnimationState.JETPACK);

        if (_deathSet)
           SwitchState(AnimationState.DEATH);

   }
   

   private void UpdateFall()
   {
       if (_isGrounded && _bigHight)
       {
           SwitchState(AnimationState.GROUNDHIT);
       }
       else if(_isGrounded &&  !_bigHight)
           SwitchState(AnimationState.WAITING);

        if (_jetpackUse)
            SwitchState(AnimationState.JETPACK);
        if (_edgeCaught)
            SwitchState(AnimationState.CLIMB);

        if (_deathSet)
           SwitchState(AnimationState.DEATH);
   }

   private void UpdateGroudHit()
   {
       
       if (_endOfHitGroundAnim && _horizontalMove == 0)
       {
           SwitchState(AnimationState.WAITING);
       }
       else if (_endOfHitGroundAnim && _horizontalMove != 0)
       {
           SwitchState(AnimationState.WALKING);
       }

       if (_deathSet)
           SwitchState(AnimationState.DEATH);
 
   }

  

   private void UpdateUp()
   {
       if (!_isGrounded  && _isVerticalMove < 0)
       {
           SwitchState(AnimationState.FALLINIT);
       }
       if (_jetpackUse)
           SwitchState(AnimationState.JETPACK);
      if (_isGrounded && _isVerticalMove < 0)
          SwitchState(AnimationState.WAITING);
        if (_edgeCaught)
            SwitchState(AnimationState.CLIMB);

        if (_deathSet)
           SwitchState(AnimationState.DEATH);
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
                break;

            case AnimationState.FALLING:

                break;

            case AnimationState.CLIMB:
                break;

            case AnimationState.GROUNDHIT:
                _endOfHitGroundAnim = false;
                break;


            case AnimationState.JUMP:
                _startJumpAnimation = false;
                if(!_jumped)
            _diver.JumpFunction();
                _jumped = true;
                break;

            case AnimationState.UP:

                break;

            case AnimationState.JETPACK:
                break;

            case AnimationState.DEATH:
                _endOfDeathAnim = false;
                break;
        }

    }

    void EndOfJumpAnimation()
    {
        _endOfJumpAnimation = true;
     
    }

    void EndOfFallInitAnim()
    {
        _endOfFallinitAnimation= true;
    }

    void EndOfGroudHitAnim()
    {
        _endOfHitGroundAnim = true;
    }

    void EndOfDeathAnim()
    {
        _endOfDeathAnim = true;
    }
    private void JumpAnimation()
    {
        _startJumpAnimation = true;
    }
}
