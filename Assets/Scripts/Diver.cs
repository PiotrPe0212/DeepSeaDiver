using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diver : MonoBehaviour
{
    public static Diver Instance { get; private set; }
    public float Movespeed = 40;
    public float UpForce = 150;
    public float BustedUpForce = 400;
    public float RightForce = 40;
    public float BustedRightForce = 200;
    private float Timer = 0f;
    private float JetpackTimer = 0f;
    public AudioSource bubleSound;
    private Rigidbody2D player;
    private BoxCollider2D _boxCollider;
    private GameObject _bubles;
    private float prevVerticvalPos;
  
    public static bool _jetpackCooldownState;
    [SerializeField]
    private LayerMask platformLayerMask;

    private Vector2 movement;
    public static bool isUpPressed = false;
    public static bool gettingUp = false;
    public static float horizontalMove = 0f;
    public static bool isGrounded;
    public static bool bigHight;
    public static float verticalMove;
    public static float verticalPos;
    public static bool _jetpackUse;
    public static bool willGroundHitAnim;
   

    void Start()
    {
        Instance = this;
        player = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _bubles = GameObject.Find("diver/bubles");
        prevVerticvalPos = player.position.y;
        _bubles.SetActive(false);
        _jetpackCooldownState = false;
      

        // _jetPackCooldownBulb.SetActive(false);
    }

    void Update()
    {
        
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        if (isUpPressed == false)
        {
            if (Input.GetKey("up"))
                isUpPressed = true;
        }
        if (!Input.GetKey("up"))
            isUpPressed = false;
       

    }
    void FixedUpdate()
    {
        Debug.Log(UpForce);
       
        verticalPos = player.position.y - prevVerticvalPos;
        prevVerticvalPos = player.position.y;

        GroundCheck();
       
        if (!bigHight && !willGroundHitAnim)
            willGroundHitAnim = true;
        else if (willGroundHitAnim && diverAnimationController._endOfHitGroundAnim)
            willGroundHitAnim = false;

        if (OxygenCounter.Instance)
            OxygenCounter.Instance.Jetpack = _jetpackUse;

        jetpackHandling();
        MovementControl();
        BubbleHandling();
    }

    void OnCollisionStay2D(Collision2D colision)
    {

        if (colision.gameObject.tag == "seadevil")
        {
            transform.parent = colision.transform;
            isGrounded = true;
        }

        if (colision.gameObject.tag == "spikes")
        {
            OxygenCounter.Instance.Damage = true;
        }
    }


    void OnCollisionExit2D(Collision2D other)
    {
        transform.parent = null;
        isGrounded = false;
    }

    void BubbleHandling()
    {
        BubleSound();
        if (_jetpackUse)
            _bubles.SetActive(true);
        else
            _bubles.SetActive(false);

        if (verticalMove == 1 && horizontalMove == 0)
        {
            _bubles.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else if (horizontalMove == 1)
        {
            _bubles.transform.eulerAngles = new Vector3(0, 0, 165);
            _bubles.transform.localPosition = new Vector3(-0.3f, -0.3f, 0);
        }
        else if (horizontalMove == -1)
        {
            _bubles.transform.eulerAngles = new Vector3(0, 0, 195);
            _bubles.transform.localPosition = new Vector3(0.3f, -0.3f, 0);
        }
    }


    void MovementControl()
    {
        float UpForceMode;
        float RightForceMode;

        if (_jetpackUse)
        {
            UpForceMode = BustedUpForce;
            RightForceMode = BustedRightForce;
        }
        else
        {
            UpForceMode = UpForce;
            RightForceMode = RightForce;
        }
        if (verticalMove > 0)
        {
            UpForceReduction();
            if (player.transform.position.y < 10)
                player.AddForce(Vector2.up * UpForceMode * verticalMove, ForceMode2D.Force);


        }
        else
        {
            if(UpForce < 150)
            StartCoroutine(UpForceReset());
        }
            

        if (verticalPos == 0 && isGrounded == true)
        {
            player.AddForce(Vector2.right * Movespeed * horizontalMove, ForceMode2D.Force);
        }
        else
        {
            player.AddForce(Vector2.right * RightForceMode * horizontalMove, ForceMode2D.Force);
            // _jetpackUse = true;

        }

        if (isGrounded || (verticalPos < 0 && horizontalMove == 0))
        {
            _jetpackUse = false;
        }

        player.velocity = movement;
    }

    void GroundCheck()
    {
 

        
        if (Physics2D.Raycast(new Vector2(_boxCollider.bounds.min.x-0.3f, _boxCollider.bounds.min.y), Vector2.down, 0.2f, platformLayerMask) || Physics2D.Raycast(new Vector2(_boxCollider.bounds.max.x+0.3f, _boxCollider.bounds.min.y), Vector2.down,  0.2f, platformLayerMask))
            isGrounded = true;
        else
            isGrounded = false;

        bigHight = Physics2D.Raycast(_boxCollider.bounds.center, Vector2.down,  2f, platformLayerMask);
    }

    void BubleSound()
    {
        if (_jetpackUse)
            bubleSound.Play();
        else
            bubleSound.Stop();
    }

    void jetpackHandling() {
        if (!_jetpackCooldownState)
        {
            if (_jetpackUse == false)
            {
                if ((Input.GetKey("space") && isUpPressed) || (Input.GetKey("space") && !isGrounded && horizontalMove != 0))
                    _jetpackUse = true;
                
            }
            if (!Input.GetKey("space"))
                _jetpackUse = false;
        }
        
        Debug.Log(JetpackTimer);
        if (_jetpackUse)
        {
            JetpackTimer += Time.deltaTime;
            Debug.Log("jetpackuse!!!!!!!!!!!!!");
        }

        if (JetpackTimer >= 0.5f && !_jetpackCooldownState)
        {
            _jetpackCooldownState = true;
            StartCoroutine(JetPackCooldown());

        }
        
       // if (_jetpackCooldownState)
        //    StartCoroutine(JetPackCooldown());
     //   else if (!_jetpackCooldownState && _jetpackUse)
       //     StartCoroutine(JetPackUseTime());
    }

    void UpForceReduction()
    {
        if (!isGrounded)
        {
            Timer += Time.deltaTime;

            if (Timer >= 0.1f)
            {
                Timer = 0f;
                if (UpForce > 80)
                    UpForce -= 10;
            }
        }
        



    }

    IEnumerator JetPackUseTime()
    {
      
        yield return new WaitForSeconds(1);
        _jetpackUse = false;

        _jetpackCooldownState = true;
    }

    IEnumerator JetPackCooldown()
    {
        _jetpackUse = false;
        JetpackTimer = 0f;
        yield return new WaitForSeconds(3);
        _jetpackCooldownState = false;
       


    }
    IEnumerator UpForceReset()
    {
        yield return new WaitForSeconds(3);
        UpForce = 150;
    }

    
    

}
