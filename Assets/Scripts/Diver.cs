using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Diver : MonoBehaviour
{
    public static Diver Instance { get; private set; }
    private GameManager GameManager;
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
    public static bool LeftWallDetect;
    public static bool RightWallDetect;
    public static bool LeftEdgeDetect;
    public static bool RightEdgeDetect;
    public static bool LeftUpperWallDetect;
    public static bool RightUpperWallDetect;
    public static bool EdgeCaught;
    public static bool ClimbingUp;
    public static float VerticalMove;
    public static float VerticalPos;
    private float RightEdgeMod = 1f;
    private float LeftEdgeMod= 1f;
    private float noGroundMod = 1f;
    public static bool _jetpackUse;
    private static bool _jetpackUsed;
    public static bool WillGroundHitAnim;
    private bool seaDevilOn;
   [SerializeField] private bool _jumpStart;


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
        ResetParameters();
    }

    private void Awake()
    {
        GameManager = FindObjectOfType<GameManager>();
        GameManager.InitGame += ResetParameters;
        GameManager.LoadLevel += ResetParameters;
        GameManager.ResetLevel += ResetParameters;
    }

    private void OnDestroy()
    {
        GameManager.InitGame -= ResetParameters;
        GameManager.LoadLevel -= ResetParameters;
        GameManager.ResetLevel -= ResetParameters;
    }

    void Update()
    {

        HorizontalMove = Input.GetAxisRaw("Horizontal");
        VerticalMove = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonUp("Jump") && !_jumpStart && !EdgeCaught && !_jetpackUse && !_jetpackUsed && IsGrounded)
            _jumpStart = true;
        else if(Input.GetButtonUp("Jump") && EdgeCaught && !ClimbingUp)
            ClimbingUp = true;
       else if (Input.GetButtonUp("Jetpack") && !_jetpackUse && !_jetpackUsed)
        {
            _jetpackUse = true;
        }

       
    }
    void FixedUpdate()
    {

        VerticalPos =(float)Math.Round((_player.position.y - _prevVerticvalPos)*1000)/1000;
        _prevVerticvalPos = _player.position.y;
        
        GroundCheck();
        WallDetection();
        EdgeDetection();
        WallToClimbDetection();

        
            if (!RightUpperWallDetect && RightWallDetect)
            {
                EdgeCaught = true;
            }
            else if (!LeftUpperWallDetect && LeftWallDetect)
            {
                EdgeCaught = true;
            }
            else
                EdgeCaught = false;
       
        if (EdgeCaught)
            Climbing();

        if (!BigHight && !WillGroundHitAnim)
         WillGroundHitAnim = true;
      
        if (WillGroundHitAnim && diverAnimationController._endOfHitGroundAnim)
            WillGroundHitAnim = false;

        if (OxygenCounter.Instance)
            OxygenCounter.Instance.Jetpack = _jetpackUse;
        
        DiverMove();
        if(_jetpackUse)
        JetpackUse();
        
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

    private void Climbing()
    {
        _player.constraints = RigidbodyConstraints2D.FreezePosition;
        _jumpStart = false;
        _jumping = false;

        if (ClimbingUp)
        {
            if(RightWallDetect)
            _player.transform.position = _player.transform.position + new Vector3(1f, 1.1f, 0);
            else if(LeftWallDetect)
               _player.transform.position = _player.transform.position + new Vector3(-1f, 1.1f, 0);
            EdgeCaught = false;
            ClimbingUp = false;
            _player.constraints = RigidbodyConstraints2D.None;
            _player.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (VerticalMove < 0)
        {
            _player.transform.position = _player.transform.position + new Vector3(0, -0.3f, 0);
            EdgeCaught = false;
            ClimbingUp = false;
            _player.constraints = RigidbodyConstraints2D.None;
            _player.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
   

    private void DiverHorizontalMove()
    {
        Vector2 horizontalMoveVector = new Vector2(Movespeed * 0.1f * noGroundMod, 0);
        if (HorizontalMove > 0 && _player.velocity.x <1.5f && !RightWallDetect )
        {
            
                _player.velocity += Vector2.right + horizontalMoveVector;
           

        }
        else if (HorizontalMove < 0 &&  _player.velocity.x >-1.5f && !LeftWallDetect )
        {
            _player.velocity -= Vector2.right + horizontalMoveVector;
        }
        else
        {
            if(_player.velocity.x !=0 && VerticalMove == 0 )
                _player.velocity -= new Vector2(_player.velocity.x *0.2f, 0);
            else if(_player.velocity.x !=0 && VerticalMove != 0 )
                _player.velocity -= new Vector2(_player.velocity.x *0.1f, 0);
        }

    }
    public void JumpFunction()
    {
        //activated by diveranimationscript
        _player.velocity += new Vector2(0, 6);
    }
    private void JetpackUse()
    {
        if (!_jetpackUsed)
        {
            _player.velocity += new Vector2(0, 8);
            _jetpackUsed = true;
        }

        if (_jetpackUsed && VerticalPos < -0.05 )
        _jetpackUse = false;
        BubbleHandling();

    }
    
    private void GroundCheck()
    {
        Vector2 GroundVector = new Vector2(_boxCollider.bounds.center.x, _boxCollider.bounds.center.y);
         
        IsGrounded = ReycastFunction(GroundVector, Vector2.down, 0.4f);
        if (IsGrounded)
            noGroundMod = 1f;
        else
            noGroundMod = 0.5f;

        BigHight = Physics2D.Raycast(_boxCollider.bounds.center, Vector2.down, 4f, platformLayerMask);
    }

    private void WallDetection()
    {
        Vector2 WallVector = new Vector2(_boxCollider.bounds.center.x, _boxCollider.bounds.center.y+0.3f);

        LeftWallDetect = ReycastFunction(WallVector, Vector2.left, 0.25f);
        RightWallDetect = ReycastFunction(WallVector, Vector2.right, 0.25f);

    }

    private void EdgeDetection()
    {
        Vector2 LeftEdgeVector = new Vector2(_boxCollider.bounds.center.x - 0.6f, _boxCollider.bounds.center.y);
        Vector2 RightEdgeVector = new Vector2(_boxCollider.bounds.center.x + 0.6f, _boxCollider.bounds.center.y);

        LeftEdgeDetect = !ReycastFunction(LeftEdgeVector, Vector2.down, 0.4f);
        if (LeftEdgeDetect && IsGrounded)
            LeftEdgeMod = 0.4f;
        else
            LeftEdgeMod = 1f;

        RightEdgeDetect = !ReycastFunction(RightEdgeVector, Vector2.down, 0.4f);
        if (RightEdgeDetect && IsGrounded)
            RightEdgeMod = 0.4f;
        else
            RightEdgeMod = 1f;
    }

    private void WallToClimbDetection()
    {

        Vector2 UpperWallVector = new Vector2(_boxCollider.bounds.center.x, _boxCollider.bounds.center.y + 0.5f);

        LeftUpperWallDetect = ReycastFunction(UpperWallVector, Vector2.left, 0.4f);
        RightUpperWallDetect = ReycastFunction(UpperWallVector, Vector2.right, 0.4f);
    }

    bool ReycastFunction(Vector2 value, Vector2 direction, float distance)
    {

        return Physics2D.Raycast(value, direction, distance, platformLayerMask);
    }



    void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("seadevil"))
        {
            seaDevilOn = true;
            transform.parent = collision.transform;
            IsGrounded = true;
            noGroundMod = 1f;
        }

        if (collision.gameObject.CompareTag("spikes"))
        {
            OxygenCounter.Instance.Damage = true;
            IsGrounded = true;
            noGroundMod = 1f;
        }
    }


    void OnCollisionExit2D(Collision2D other)
    {
        transform.parent = null;
        IsGrounded = false;
        seaDevilOn = false;
    }
    private void BubleSound()
    {
        if (_jetpackUse)
            bubleSound.Play();
        else
            bubleSound.Stop();
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

    public void ResetAfterJump()
    {
        _jumpStart = false;
        _jumping = false;
        UpDoublePress = false;
        _jetpackUse = false;
        _jetpackUsed = false;
    }



    private void ResetParameters()
    {
        EdgeCaught = false;
        ClimbingUp = false;
        IsGrounded = false;
        BigHight = false;
        RightWallDetect = false;
        LeftWallDetect = false;
        RightEdgeDetect = false;
        LeftEdgeDetect = false;
        LeftUpperWallDetect = false;
        RightUpperWallDetect = false;
        _bubles.SetActive(false);
        ResetAfterJump();

    }
 

}
