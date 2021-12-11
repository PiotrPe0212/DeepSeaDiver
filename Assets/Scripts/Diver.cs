using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Diver : MonoBehaviour
{
    public static Diver Instance { get; private set; }
    public float Movespeed = 40;
    public float UpForce = 150;
    public float BustedUpForce = 400;
    public float RightForce = 40;
    public float BustedRightForce = 200;
    public AudioSource bubleSound;
    private Rigidbody2D _player;
    private BoxCollider2D _boxCollider;
    private GameObject _bubles;
    private float _prevVerticvalPos;
    public static bool _jumping;
    [SerializeField]
    private LayerMask platformLayerMask;

    [SerializeField] private diverAnimationController _diverAnimationController;
    private Vector2 _movement;
    public static bool UpDoublePress = false ;
    public static float HorizontalMove = 0f;
    public static bool IsGrounded;
    public static bool BigHight;
    public static float VerticalMove;
    public static float VerticalPos;
    public static bool _jetpackUse;
    public static bool WillGroundHitAnim;
    private bool _jumpStart;
    private bool  _waitingForAnim;
    private bool _jumpReset;


    public event Action JetPack;
    public event Action Jump;

    public event Action GetDamage;
    

    void Start()
    {
        Instance = this;
        _player = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _bubles = GameObject.Find("diver/bubles");
        _prevVerticvalPos = _player.position.y;
        _bubles.SetActive(false);
        ResetAfterJump();
    }


    void Update()
    {
        
        HorizontalMove = Input.GetAxisRaw("Horizontal");
        VerticalMove = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonUp("Jump") && !_jumpStart)
            _jumpStart = true;
        if (_jumping && !UpDoublePress && Input.GetButtonDown("Jump"))
        {
            UpDoublePress = true;
        }
    }
    void FixedUpdate()
    {

        VerticalPos = _player.position.y - _prevVerticvalPos;
        _prevVerticvalPos = _player.position.y;
        
        GroundCheck();

        if (!BigHight && !WillGroundHitAnim)
        { WillGroundHitAnim = true;
        }
        else if (WillGroundHitAnim && diverAnimationController._endOfHitGroundAnim)
            WillGroundHitAnim = false;

        if (OxygenCounter.Instance)
            OxygenCounter.Instance.Jetpack = _jetpackUse;
        
        DiverMove();
        JetpackUse();
        BubbleHandling();
    }

    void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("seadevil"))
        {
            transform.parent = collision.transform;
            IsGrounded = true;
        }

        if (collision.gameObject.CompareTag("spikes"))
        {
            OxygenCounter.Instance.Damage = true;
            IsGrounded = true;
        }
    }


    void OnCollisionExit2D(Collision2D other)
    {
        transform.parent = null;
        IsGrounded = false;
    }

    void BubbleHandling()
    {
        BubleSound();
        if (_jetpackUse)
            _bubles.SetActive(true);
        else
            _bubles.SetActive(false);

        if (VerticalMove == 1 && HorizontalMove == 0)
        {
            _bubles.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else if (HorizontalMove == 1)
        {
            _bubles.transform.eulerAngles = new Vector3(0, 0, 165);
            _bubles.transform.localPosition = new Vector3(-0.3f, -0.3f, 0);
        }
        else if (HorizontalMove == -1)
        {
            _bubles.transform.eulerAngles = new Vector3(0, 0, 195);
            _bubles.transform.localPosition = new Vector3(0.3f, -0.3f, 0);
        }
    }

 private void DiverMove()
    {
       
        if (_jumpStart && !_jumping)
        {
                Jump();
            _jumping = true;
             
            }
       

        DiverHorizontalMove();

    }

    public void JumpFunction()
    {
        _player.velocity += new Vector2(0, 6);
    }

    private void DiverHorizontalMove()
    {
        if (HorizontalMove > 0 && _player.velocity.x <5)
        {
            _player.AddForce(Vector2.right * Movespeed * HorizontalMove, ForceMode2D.Force);
        }
        else if (HorizontalMove < 0 &&  _player.velocity.x >-5)
        {
            _player.AddForce(Vector2.right * Movespeed * HorizontalMove, ForceMode2D.Force);
        }
        else
        {
            if(_player.velocity.x !=0 && VerticalMove == 0 )
                _player.velocity -= new Vector2(_player.velocity.x *0.1f, 0);
            else if(_player.velocity.x !=0 && VerticalMove != 0 )
                _player.velocity -= new Vector2(_player.velocity.x *0.05f, 0);
        }
    }

    private void JetpackUse()
    {
        if (UpDoublePress  && !_jetpackUse && VerticalPos>0)
        {
            Debug.Log("jump!AA" );
            _player.velocity += new Vector2(0, 8);
            _jetpackUse = true;
        }

        if (!(VerticalPos < 0)) return;
        UpDoublePress = false;
        _jetpackUse = false;
    }
    
    private void GroundCheck()
    {
 

        
        if (Physics2D.Raycast(new Vector2(_boxCollider.bounds.min.x-0.3f, _boxCollider.bounds.min.y), Vector2.down, 0.2f, platformLayerMask) || 
            Physics2D.Raycast(new Vector2(_boxCollider.bounds.max.x+0.3f, _boxCollider.bounds.min.y), Vector2.down,  0.2f, platformLayerMask))
         IsGrounded = true;
        else
            IsGrounded = false;

        
            BigHight = Physics2D.Raycast(_boxCollider.bounds.center, Vector2.down, 2f, platformLayerMask);
    }

    private void BubleSound()
    {
        if (_jetpackUse)
            bubleSound.Play();
        else
            bubleSound.Stop();
    }



    public void ResetAfterJump()
    {
        _jumpStart = false;
        _jumping = false;
        UpDoublePress = false;
    }
}
