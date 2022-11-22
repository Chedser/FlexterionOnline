using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class ClientMoveMP : MonoBehaviour
{
    HealthMP health;
    private Rigidbody rb;

   public GameObject camera;
    public GameObject cameraClient;

    private bool enableMovement = true;

    [Header("Movement properties")]
    public float runSpeed = 20.0f;
    public float changeInStageSpeed = 10.0f; // Lerp from walk to run and backwards speed
    public float maximumPlayerSpeed = 150.0f;
    [HideInInspector] 
    public float vInput, hInput;

    [Header("Jump")]
    public float jumpForce = 500.0f;
    public float jumpCooldown = 1.0f;
    private bool jumpBlocked = false;

    float prevY;

    [SerializeField]
    AudioSource ahuennoSound;
    [SerializeField]
    AudioSource takeGrenadeSound;

    public bool isGrounded = false;
    public bool IsGrounded { get { return isGrounded; } }

    private Vector3 inputForce;
    private int i = 0;
    [HideInInspector]
    public Animator anim;

    public bool isJumping;
    public bool isRunning;

    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        anim = this.GetComponent<Animator>();

        health = GetComponent<HealthMP>();

        if (!PV.IsMine) {

            Destroy(camera);
            Destroy(cameraClient);
            Destroy(this);
            Destroy(rb);

        }
     
    }
    private void OnCollisionStay(Collision other)
    {

        if (!PV.IsMine) {
            return;
        }

        if (other.gameObject.tag.Equals("Ground"))
        {

            isGrounded = true;
            anim.SetBool("jumpClient", false);
            isJumping = false;

        }

    }

    private void OnCollisionExit(Collision other)
    {

        if (!PV.IsMine)
        {
            return;
        }

        if (other.gameObject.tag.Equals("Ground"))
        {

            isGrounded = false;
            isJumping = true;
            anim.SetBool("jumpClient", true);

        }

    }


    private void FixedUpdate()
    {

        // I recieved several messages that there are some bugs and I found out that the ground check is not working properly
        // so I made this one. It's faster and all it needs is the velocity of the rigidbody in two frames.
        // It works pretty well!
        /*  isGrounded = (Mathf.Abs(rb.velocity.y - prevY) < .1f) && (Physics.OverlapSphere(groundChecker.position, groundCheckerDist).Length > 1); // > 1 because it also counts the player
            prevY = rb.velocity.y; */


        //  debug.text = GameManager.enemiesKilledCount.ToString();

        if (!PV.IsMine )
        {
            return;
        }

        if (health.CurrentHealth <= 0) {

            return;

        }


        // Input
        vInput = Input.GetAxisRaw("Vertical");
        hInput = Input.GetAxisRaw("Horizontal");

        // Clamping speed
        rb.velocity = ClampMag(rb.velocity, maximumPlayerSpeed);

        if (!enableMovement)
            return;
        inputForce = (transform.forward * vInput + transform.right * hInput).normalized * runSpeed;

        if (isGrounded)
        {
            // Jump
            if (Input.GetButton("Jump") && !jumpBlocked)
            {

                rb.AddForce(-jumpForce * rb.mass * Vector3.down);
                jumpBlocked = true;
                Invoke("UnblockJump", jumpCooldown);
                //    anim.SetBool("runClient", false);
                isJumping = true;
                anim.SetBool("jumpClient", true);
            }
           
            // Ground controller
            rb.velocity = Vector3.Lerp(rb.velocity, inputForce, changeInStageSpeed * Time.fixedDeltaTime);

           

        }
        else
        {
            // Air control
            rb.velocity = ClampSqrMag(rb.velocity + inputForce * Time.fixedDeltaTime, rb.velocity.sqrMagnitude);

          //  anim.SetBool("runClient", false);
            anim.SetBool("jumpClient", true);
        }
      
        if (rb.velocity.magnitude > 0.1f && isGrounded)
        {

            anim.SetBool("runClient", true);
            isRunning = true;

        }
        else
        {
            //    anim.SetBool("jumpClient", false);
            isRunning = false;
            anim.SetBool("runClient", false);

        }


    }

    private static Vector3 ClampSqrMag(Vector3 vec, float sqrMag)
    {
        if (vec.sqrMagnitude > sqrMag)
            vec = vec.normalized * Mathf.Sqrt(sqrMag);
        return vec;
    }

    private static Vector3 ClampMag(Vector3 vec, float maxMag)
    {
        if (vec.sqrMagnitude > maxMag * maxMag)
            vec = vec.normalized * maxMag;
        return vec;
    }


    private void UnblockJump()
    {
        jumpBlocked = false;
    }


    // Enables jumping and player movement
    public void EnableMovement()
    {
        enableMovement = true;
    }

    // Disables jumping and player movement
    public void DisableMovement()
    {
        enableMovement = false;
    }

}
