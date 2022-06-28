using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAdvanced : MonoBehaviour
{
    [Header("Horizontal Move")]
    //Andar
    [SerializeField] private float defautMoveSpeed;
    [SerializeField] private Vector2 moveInput;
    private float decceleration;
    private float acceleration;
    private float moveSpeed;


    private bool _facingRight;
   

    public float defaultDecceleration;
    public float defaultAcceleration;

  
    public float velPower;


    [Header("Jump")]
    public float jumpForce;
    [Range(0, 1)] public float jumpCutMultiplier;
    [Space(10)]
    [Range(0, 0.5f)] public float jumpBufferTime;
    [Space(10)]
    [Range(0, 0.5f)] public float coyoteTime;
    private bool _isJumping;
    private float _lastOnGroundTime;
    private float _lastPressedJumpTime;


    //Gravity
    public float fallGravityMultiplier;
    private float _gravityScale = 1.1f;




    [Header("Physics")]
    public float airAcceleleration;
    public float airDecceleration;
    public float airMoveSpeed;
    public float velocityH;

    //Friction
    public float frictionAmount;

    [Header("Colisions")]
    //Ground
    [SerializeField] private bool onGround = false;
    public float groundLength = 0.6f;
    public Vector3 groundColliderOffset;

    //Wall
    [SerializeField] private bool onWall = false;
    public float wallLength = 0.6f;
    public Vector3 wallColliderOffset;



    [Header("Components")]
    public Rigidbody2D playerRb;
    public Animator animator;
    public SpriteRenderer playerSprite;
    public GameObject feetPos;
    public GameObject bodyPos;

    [SerializeField] private LayerMask _layerGround;


    // Start is called before the first frame update
    private void Awake()
    {
        SetGravityScale();
    }

    // Update is called once per frame
    void Update()
    {                                                                              //Detecta o chão Por meio de 2 Raycast
        onGround = Physics2D.Raycast(feetPos.transform.position + groundColliderOffset, Vector2.down, groundLength, _layerGround) || Physics2D.Raycast(feetPos.transform.position - groundColliderOffset, Vector2.down, groundLength, _layerGround);
        onWall = Physics2D.Raycast(bodyPos.transform.position + wallColliderOffset, Vector2.left, wallLength, _layerGround) || Physics2D.Raycast(bodyPos.transform.position - wallColliderOffset, Vector2.right, wallLength, _layerGround);
        
        //MoveInput
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Flip();
        JumpGravity();

        #region Jump
        //Jump

        if (Input.GetButtonDown("Jump"))
        {
            _lastPressedJumpTime = jumpBufferTime;
        }

        if(Input.GetButtonUp("Jump"))
        {
            JumpCut();
        }
        #endregion

        #region JumpRegion
        //Jump checks
        if (_isJumping && playerRb.velocity.y < 0) { 
            _isJumping = false;
        }

        if (_lastPressedJumpTime > 0)
        {
            if(_lastOnGroundTime > 0)
            {
                _isJumping = true;
                Jump();
            }
        }
        
        //ground Check

        if (onGround) { 
            _lastOnGroundTime = coyoteTime;
        }
        //Timers
        _lastOnGroundTime -= Time.deltaTime;
        _lastPressedJumpTime -= Time.deltaTime;
        #endregion


        //Flip


        //Pulo Antigo
        /*
        if (onGround && Input.GetButtonDown("Jump"))
        {
            playerRb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

        }
        */


    }


    void FixedUpdate()
    {
        //Physics
        ChangeVelocity();
        AirAcceleration();
        Friction();

        //Move
        MoveCharacter();
        //Jump

        //Anim
        //Flip();

    }

    #region Move

    void MoveCharacter() // Devido ao uso de AddForce no MoveCharacter(), o jogador acaba escorrregando como se fosse gelo.  
    {

        
        float targetSpeed = moveInput.x * moveSpeed;

        float speedDif = targetSpeed - playerRb.velocity.x;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        
        playerRb.AddForce(movement * Vector2.right);

        //flip
        
        

        //Animator
        animator.SetFloat("Horizontal", Mathf.Abs(playerRb.velocity.x));
        animator.SetFloat("Vertical", (playerRb.velocity.y));
        animator.SetBool("OnGround", onGround);
    }

    void Flip()
    {
        if(moveInput.x > 0)
        {
            playerSprite.flipX = false;
        } else if (moveInput.x < 0)
        {
            playerSprite.flipX = true;
        }

    }
    #endregion

    #region Jump

    private void Jump()
    {
        //ensures we can't call a jump multiple times from one press
        _lastPressedJumpTime = 0;
        _lastOnGroundTime = 0;

        playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce);
    }

    private void JumpCut()
    {
        //applies force downward when the jump button is released. Allowing the player to control jump height
        playerRb.AddForce(Vector2.down * playerRb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
    }
    #endregion


    #region gravity
    public void JumpGravity()
    {
        if(playerRb.velocity.y < 0)
        {
            playerRb.gravityScale = _gravityScale * fallGravityMultiplier;
        }
        else
        {
            playerRb.gravityScale = _gravityScale;
        }
    }

    public void SetGravityScale()
    {
        playerRb.gravityScale = _gravityScale;
    }

    #endregion

    #region Physics
    void ModifyPhysics() 
    {


       
    }

    private void Friction()
    {
        if(onGround && Mathf.Abs(moveInput.x) < 00.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(playerRb.velocity.x), Mathf.Abs(frictionAmount));

            amount *= Mathf.Sign(playerRb.velocity.x);

            playerRb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        } 
    }
    
    private void ChangeVelocity()
    {
        if(moveInput.x == 0 && onGround)
        {
            velocityH = 0;
        }
        else
        {
            velocityH = playerRb.velocity.x;
        }
    }

    private void AirAcceleration()
    {
        acceleration = 0;
        decceleration = 0;
        moveSpeed = 0;
        if(onGround)
        {
            acceleration = defaultAcceleration;
            decceleration = defaultDecceleration;
            moveSpeed = defautMoveSpeed;
        } else
        {
            acceleration = airAcceleleration;
            decceleration = airDecceleration;
            moveSpeed = airMoveSpeed;
        }
    }

    #endregion 

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        //Ground

        //Gizmos.DrawLine(feetPos.transform.position + groundColliderOffset, feetPos.transform.position + groundColliderOffset  + Vector3.down * groundLength);
       //Gizmos.DrawLine(feetPos.transform.position - groundColliderOffset, feetPos.transform.position - groundColliderOffset + Vector3.down * groundLength);

        Gizmos.color = Color.blue;

        //Wall
       //Gizmos.DrawLine(bodyPos.transform.position + wallColliderOffset, bodyPos.transform.position + wallColliderOffset + Vector3.left * wallLength);
        //Gizmos.DrawLine(bodyPos.transform.position - wallColliderOffset, bodyPos.transform.position - wallColliderOffset + Vector3.right * wallLength);


    }
}
