using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [Header("Horizontal Move")]
    //Andar
    [SerializeField] private float moveSpeed = 10f;
    public Vector2 direction;
    private bool _facingRight = true;

    [Header("Vertical Move")]
    public float jumpSpeed = 8f;
    public float jumpDelay = 0.25f;
    private float _jumpTimer;

    [Header("Physics")]
    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMult = 5f;

    [Header("Colisions")]
    public bool onGround = false;
    public float groundLength = 0.6f;
    public Vector3 colliderOffset;

    [Header("Components")]
    public Rigidbody2D playerRb;
    public Animator animator;
    public SpriteRenderer playerSprite;
    public GameObject feetPos;

    [SerializeField] private LayerMask _layerGround;


    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {                                                                              //Detecta o chão Por meio de 2 Raycast
        onGround = Physics2D.Raycast(feetPos.transform.position + colliderOffset, Vector2.down, groundLength, _layerGround) || Physics2D.Raycast(feetPos.transform.position - colliderOffset, Vector2.down, groundLength, _layerGround); 


        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            _jumpTimer = Time.time + jumpDelay;
        }

        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //Entrega +-1 pãra os eixos em que o jogador se movimentar
    }
    

    void FixedUpdate()
    {
        moveCharacter(direction.x);
        modifyPhysics();

        //Jump
        if (_jumpTimer > Time.time && onGround)
        {
            Jump();
        }
    }
    
    #region Move
        
    void moveCharacter(float horizontal) // Devido ao uso de AddForce no MoveCharacter(), o jogador acaba escorrregando como se fosse gelo.  
    {
        playerRb.AddForce(Vector2.right * horizontal * moveSpeed);// Addforce é utilizado para criar uma força que empurra o jogador. 

    //Animator
    animator.SetFloat("Horizontal", Mathf.Abs(playerRb.velocity.x));
    animator.SetFloat("Vertical", (playerRb.velocity.y));
    animator.SetBool("OnGround", onGround);


        if ((horizontal > 0 && !_facingRight) || (horizontal < 0 && _facingRight))
        {
            Flip();
        }
        if(Mathf.Abs(playerRb.velocity.x) > maxSpeed)
        {
            playerRb.velocity = new Vector2(Mathf.Sign(playerRb.velocity.x) * maxSpeed, playerRb.velocity.y);
        }

    }

    void Flip()
    {
        _facingRight = !_facingRight;
        if(_facingRight)
        {
            playerSprite.flipX = false;
        } else
        {
            playerSprite.flipX = true;
        }

    }
    #endregion

    #region Jump

    void Jump()
    {
        playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
        playerRb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        _jumpTimer = 0;
    }

    #endregion

    #region Physics
    void modifyPhysics() 
    {
        bool changingDirections = (direction.x > 0 && playerRb.velocity.x < 0) || (direction.x < 0 && playerRb.velocity.x > 0);


        //Gravity
        if(onGround)
        {
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections) // Melhora a movimentação tirando o efeito escorregadio do game. 
            {                                                        // Isso é possível por meio de um Drag(arrasto) que é implementado no personagem sempre que ele vira ou para.
                playerRb.drag = linearDrag;
            }
            else
            {
                playerRb.drag = 0f;
            }
            playerRb.gravityScale = 0;
        } else
        {
            playerRb.gravityScale = gravity;
            playerRb.drag = linearDrag * 0.15f;
            if(playerRb.velocity.y < 0)
            {
                playerRb.gravityScale = gravity * fallMult;

            }else if(playerRb.velocity.y > 0 && !Input.GetButton("Jump")) {
                playerRb.gravityScale = gravity * (fallMult / 2);
            }
        }
    }

    #endregion 

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(feetPos.transform.position + colliderOffset, feetPos.transform.position + colliderOffset  + Vector3.down * groundLength);
        Gizmos.DrawLine(feetPos.transform.position - colliderOffset, feetPos.transform.position - colliderOffset + Vector3.down * groundLength);

    }
}
