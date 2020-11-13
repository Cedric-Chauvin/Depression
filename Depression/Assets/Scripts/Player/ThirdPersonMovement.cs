using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public Transform camera;
    public PlayerFeet feet;
    public float speed;
    public float jumpSpeed;
    public bool keepJumpButtonPressed;
    public bool airControl;
    public float turnSmoothTime;

    private float turnSmoothVelocity;
    private Rigidbody rb;

    private float horizontal;
    private float vertical;
    private bool jump = false;
    private bool jumpDone = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (jumpDone)
        {
            if (keepJumpButtonPressed)
                jump = Input.GetButton("Jump");
            else
                jump = Input.GetButtonDown("Jump");

            if (jump)
                jumpDone = false;
        }
    }

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        if (horizontal != 0 || vertical != 0)
        {
            if (!feet.isGrounded)
                direction = new Vector3(direction.x, direction.y, direction.z);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward) * speed;

            if (feet.isGrounded || airControl)
                rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);

        }
        else if (feet.isGrounded)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }

        if (jump && feet.isGrounded && rb.velocity.y == 0f)
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            jump = false;
            StartCoroutine(WaitForNextJump());
        }
    }

    private IEnumerator WaitForNextJump()
    {
        while (feet.isGrounded)
            yield return null;
        while (!feet.isGrounded)
            yield return null;
        jumpDone = true;
    }
}
