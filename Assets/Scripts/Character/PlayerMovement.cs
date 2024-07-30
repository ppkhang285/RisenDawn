using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float defaultSpeed = 5f;
    private float moveSpeed;

    private Vector2 movement;
    private Vector2 lastMovement;
    private Rigidbody2D rb;
    private Animator animator;

    // Variable for Dash
    [SerializeField] private float dashSpeed;
    [SerializeField] private float _dashDuration;
    [SerializeField] private int _dashIframe;
    [SerializeField] private float _dashCooldown;

    private bool isDasing;
    private float dashDuration;
    private float dashCooldown;
    private int dashIframe;
    private bool havingIframe;
    


    void Start()
    {
        Setup();
    }

    private void Setup()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Setup for Dash
        isDasing = false;
        dashIframe = 0;
        dashDuration = 0;
        dashCooldown = 0;

        moveSpeed = defaultSpeed;
        lastMovement = Vector2.down;
        
    }

    // Update is called once per frame
    void Update()
    {

        HandleMovement();
        CalculateDashIframe();
        HandleDash();

    }
    private void FixedUpdate()
    {
        CalculateDashTime();
        Vector2 characterMovement = movement;
        if (isDasing) characterMovement = lastMovement;
        rb.MovePosition(rb.position + characterMovement * moveSpeed * Time.fixedDeltaTime);
        
        
    }

    private void HandleMovement()
    {
        // Get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");


        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (movement.sqrMagnitude > 0)
        {
            animator.SetFloat("LastHorizontal", movement.x);
            animator.SetFloat("LastVertical", movement.y);
            lastMovement = movement;
        }
    }


    private void CalculateDashIframe()
    {
        if (isDasing && havingIframe)
        {
            dashIframe ++;
        }

        if (dashIframe >  _dashIframe) 
        {
            dashIframe = 0;
            havingIframe = false;
        }


    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDasing && dashCooldown <=0)
        {
            Debug.Log("Dash");
            isDasing = true;
            havingIframe = true;
            moveSpeed = dashSpeed;
        }

        animator.SetBool("IsDashing", isDasing);
    }
    private void CalculateDashTime()
    {
        if (isDasing)
        {
            if (dashDuration <= 0)
            {
                isDasing = false;
                dashDuration = _dashDuration;
                moveSpeed = defaultSpeed;
                dashCooldown = _dashCooldown;

            }

            dashDuration -= Time.fixedDeltaTime;
            return;
        }
        else
        {
            if (dashCooldown > 0) dashCooldown -= Time.fixedDeltaTime;
        }

        
    }

   
    

    
}
