using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private float playerSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    public bool flipX;

    private float gravity = -9.87f;

    [SerializeField] private bool jumpAllowed;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool grounded;

    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float fallSpeed;

    [SerializeField] private Animator bHAnimator;
    [SerializeField] private string runState;

    [SerializeField] private Rigidbody2D bulletPrefab;
    [SerializeField] private Transform bulletFirePoint;
    [SerializeField] private Transform bulletFirePointLeft;
    [SerializeField] private float bulletSpeed;

    Vector3 originalFirePointPos;

    //START SINGLETON

    public static PlayerMovement instance;
    public static PlayerMovement Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<PlayerMovement>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject();
                    singleton.AddComponent<PlayerMovement>();
                    singleton.name = "(Singleton) PlayerMovement";
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bHAnimator = GetComponent<Animator>();
        playerSpeed = 4;
        jumpAllowed = true;
        originalFirePointPos = bulletFirePoint.position;
        instance = this;
    }
    private void Update()
    {

        RunStates();

        //Sprite Direction Flip
        if (flipX)  { spriteRenderer.flipX = true;  }
        else if (!flipX) { spriteRenderer.flipX = false; }


        //Run Input

        if (Input.GetKey(KeyCode.LeftShift))  { playerSpeed = runSpeed; }
        else if (!Input.GetKey(KeyCode.LeftShift)) {  playerSpeed = walkSpeed; }

        //Jump Input and Prerequisites
        if (Input.GetKeyDown(KeyCode.Space) && jumpAllowed) { Jump(); }

        if (isJumping)  { jumpAllowed = false; }
        else if (grounded) { jumpAllowed = true; }
        else { jumpAllowed = false; }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        grounded = true;
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        grounded = false;
    }

    //STATE MACHINE
    public void RunStates()
    {
        if (Input.GetAxisRaw("Horizontal") == 0) { Idle(); runState = "isIdle"; }
        else if (Input.GetAxisRaw("Horizontal") > 0) { WalkRight(); runState = "isWalking"; }
        else if (Input.GetAxisRaw("Horizontal") < 0) { WalkLeft(); runState = "isWalking"; }

        if (Input.GetAxisRaw("Jump")>0) { Jump();  runState = "isJumping"; }

        if (Input.GetKey(KeyCode.W)) { MoveForward(); gameObject.transform.Translate(Vector3.forward * playerSpeed * Time.deltaTime); runState = "posZAxis"; };
        if (Input.GetKey(KeyCode.S)) { MoveBackward(); gameObject.transform.Translate(Vector3.back * playerSpeed * Time.deltaTime); runState = "negZAxis"; };

        if (Input.GetMouseButtonDown(0) && Input.GetAxisRaw("Horizontal") > 0) { FireBullet(); runState = "firedBullet"; } // Bullet Fire Animation Faces Wrong Direction.
        else if (Input.GetMouseButtonDown(0) && Input.GetAxisRaw("Horizontal") < 0) { FireBulletLeft(); runState = "firedbullet"; }
        else if (Input.GetMouseButtonDown(0) && Input.GetAxisRaw("Horizontal") == 0) { flipX = true; FireBullet(); runState = "firedBullet"; }
        else { StopFiredBullet(); }
    }
    //ANIMATION STATES

    public void FireBullet()
    {
        ResetStates();
        
        bHAnimator.SetBool("firedBullet", true);
        flipX = false;
        Rigidbody2D bulletClone = Instantiate(bulletPrefab, bulletFirePoint, false);
        bulletClone.AddForce(Vector2.right * bulletSpeed);
    }
    public void FireBulletLeft()
    {
        ResetStates();

        bHAnimator.SetBool("firedBullet", true);
        flipX = true;
        Rigidbody2D bulletClone = Instantiate(bulletPrefab, bulletFirePointLeft, false);
        bulletClone.AddForce(Vector2.left * bulletSpeed);

    }
    public void StopFiredBullet()
    {
        bHAnimator.SetBool("firedBullet", false);
    }

    //Z Axis Movement
    public void MoveForward()
    {
        ResetStates();
        bHAnimator.SetBool("movingForward", true);
        runState = "posZAxis";
        //StopMovingForward();
    }
    public void StopMovingForward()
    {
        
        bHAnimator.SetBool("movingForward", false);

    }
    public void MoveBackward()
    {
        ResetStates();
        bHAnimator.SetBool("movingBackward", true);
        runState = "negZAxis";
        //StopMovingBackward();
    }
    public void StopMovingBackward()
    {
        bHAnimator.SetBool("movingBackward", false); ;
    }

    //Jumping Animation State
    public void Jump()
    {
        ResetStates();
        
        isJumping = true;
        runState = "isJumping";
        playerRB.transform.position += Vector3.up * jumpHeight * jumpSpeed * -gravity * Time.deltaTime;
        bHAnimator.SetBool("isJumping", true);
        Debug.Log("Jumped.");
        //StopJump();

    }
    public void StopJump()
    {
        isJumping = false;
        //playerRB.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        jumpAllowed = false;
        bHAnimator.SetBool("isJumping", false);
        Debug.Log("Player Grounded.");
        runState = "None";
    }
 
    //Idle Animation State
    public void Idle()
    {
        ResetStates();
        runState = "isIdle";
        bHAnimator.SetBool("isIdle", true);
    }
    public void StopIdle()
    {
        bHAnimator.SetBool("isWalking", false);
        runState = "None";
    }
    //Walking Animation State
    public void WalkRight()
    {
        ResetStates();
        playerRB.transform.Translate(Vector3.right * playerSpeed * Time.deltaTime);
        runState = "isWalking";
        flipX = true;
        bHAnimator.SetBool("isWalking", true);
    }
    public void WalkLeft()
    {
        ResetStates();
        playerRB.transform.Translate(Vector3.left * playerSpeed * Time.deltaTime);
        runState = "isWalking";
        flipX = false;
        bHAnimator.SetBool("isWalking", true);
    }
    public void StopWalk()
    {
        runState = "None";
        bHAnimator.SetBool("isWalking", false);
    }
    void ResetStates()
    {
        StopIdle();
        StopWalk();
        StopJump();
        StopMovingBackward();
        StopMovingForward();
        runState = "None";
    }
}
