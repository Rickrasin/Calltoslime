using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move Settings")]
    //Andar
    [SerializeField] private float moveSpeed = 3f;
    private float moveH;
    //Pular
    [SerializeField] private bool isJumping;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float ghostJump;
    public float counterJumpDefault = 0.4f;
    public float counterJump = 0.4f;


    public float jumpStartTime;
    private float jumpTime;



    [Header("Colision Settings")]

    //Atributos do isGround
    

    

    [Header("Objects Settings")]
    [SerializeField] private LayerMask layerGround;
    public Transform feetPosition;
    SpriteRenderer playerSprite;
    Rigidbody2D playerRb;
    public Collider2D boxCollider2D;

    Animator playerAnimation;


    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponentInChildren<SpriteRenderer>();
        playerAnimation = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        move();
        Jump();
        ghostJumpCheck();
    }

    void FixedUpdate()
    {
        // Movimentação do Player
        playerRb.velocity = new Vector2(moveH * moveSpeed, playerRb.velocity.y);


        //Pulo do Player
        if (isJumping)
        {
            if (counterJump > 0)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce);
            }
            else
            {
                isJumping = false;

            }
        }
    }

    void move()
    {
        //Movimentação
        moveH = Input.GetAxisRaw("Horizontal");

        if(moveH != 0)
        {
            moveSpeed += 15f * Time.deltaTime;

            if(moveSpeed >= 3.0f)
            {
                moveSpeed = 3.0f;
            }
        } else
        {
            moveSpeed = 0;
        }


        //Flip
        if (moveH > 0)
        {
            playerSprite.flipX = true;
        }
        else if (moveH < 0)
        {
            playerSprite.flipX = false;
        }


        //Animations
        if (isGround()) //Animações no chão
        {

            playerAnimation.SetBool("Jumping", false);


            if (moveH != 0)
            {
                playerAnimation.SetBool("Walking", true);
            }
            else
            {
                playerAnimation.SetBool("Walking", false);
            }

        }
        else //Animações no ar
        {

            playerAnimation.SetBool("Walking", false);


            if (playerRb.velocity.y != 0 || isGround()! || isJumping)
            {
                playerAnimation.SetBool("Jumping", true);

            }
            else
            {
                playerAnimation.SetBool("Jumping", false);

            }


        }

    }
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && ghostJump > 0)
        {
            isJumping = true;
            jumpTime = jumpStartTime;
            playerRb.velocity = Vector2.up * jumpForce;
        }
        if (Input.GetButton("Jump"))
        {
            if (jumpTime > 0)
            {
                playerRb.velocity = Vector2.up * jumpForce;
                jumpTime -= Time.deltaTime;

            }
            else
            {
                isJumping = false;
            }
        }
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }


    }

    void ghostJumpCheck()
    {
        if(isGround())
        {
            ghostJump = 0.08f;
        } else
        {
            ghostJump -= Time.deltaTime;
            if(ghostJump <= 0)
            {
                ghostJump = 0;
            }
        }
    }
    private bool isGround()
    {
       RaycastHit2D isGround = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0, Vector2.down, 0.1f, layerGround);
        return isGround.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
    }
}
