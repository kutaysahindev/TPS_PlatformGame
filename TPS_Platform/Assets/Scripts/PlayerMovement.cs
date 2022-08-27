using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // Stamina 0 - Increase 5 saniye sonra
    // Stamina  2 saniye

    public CharacterController controller;
    public Transform cam;

    private float moveSpeed;
    [SerializeField] private StaminaBar staminaBar;
    [SerializeField] private float dValue = 5f;

    [SerializeField] private float maxStamina=20f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpHeight = 3f;

    public Transform groundCheck;
    public LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private float gravity = -9.81f;

    private PlayerAnimationController animationController;

    Vector3 velocity;
    bool isGrounded;

    [SerializeField] float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    private float stamina;
    private Animator anim;

    public bool isRunning;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        anim = GetComponentInChildren<Animator>();
        animationController = new PlayerAnimationController(); 
        animationController.SetAnim(anim);
        stamina = maxStamina;
        staminaBar.SetMaxValue(maxStamina);
        isRunning = false;
    }

    void Update()
    {
        VerticalPlayerMove();
        HorizontalPlayerMove();
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        staminaBar.SetBarVisual(stamina);
        //Debug.Log(stamina);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
        //Debug.Log(isRunning);
        
    }

    
   private void DecreaseEnergy()
   {
       stamina -= dValue * Time.deltaTime;
   }
   private void IncreaseEnergy()
   {
       stamina += dValue * Time.deltaTime;
   }

    private float time;

    private void HorizontalPlayerMove()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        isRunning = false;



        if (direction.magnitude >= 0.1f)
        {
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            if (!Input.GetKey(KeyCode.LeftShift) || stamina<=0f)
            {
                //Debug.Log("Walking");
                Walk();
                IncreaseEnergy();
            }
            else if (Input.GetKey(KeyCode.LeftShift) && stamina > 0f)
            {
                if (stamina <= 3)
                {
                    StartCoroutine(Wait());
                }
                Run();
                DecreaseEnergy();
                //Debug.Log("Running")
                                       
            }
            //Mathf.Pow
            moveDir *= moveSpeed;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);

        }
        else if (direction == Vector3.zero)
        {
            Idle();
            IncreaseEnergy();
        }
    }

    private void VerticalPlayerMove()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -4f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

    }

    private void Idle()
    {
        animationController.SetMovementBlendTree(0f);
    }

    private void Walk()
    {
        moveSpeed = walkSpeed;
        animationController.SetMovementBlendTree(0.5f);
    }

    private void Run()
    {
        isRunning = true;
        moveSpeed = runSpeed;
        animationController.SetMovementBlendTree(1f);
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void Attack()
    {

        animationController.PlayAttackAnim();
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3);
    }
}
