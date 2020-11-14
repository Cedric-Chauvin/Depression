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
    [Range(1, 5)]
    public int maxJump = 1;
    public bool keepJumpButtonPressed;
    public bool airControl;
    public float turnSmoothTime;

    private float turnSmoothVelocity;
    private Rigidbody rb;

    private float horizontal;
    private float vertical;
    private bool jump = false;
    private bool jumpDone = true;
    private int nbJumpDone = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (maxJump > 1)
            keepJumpButtonPressed = false;
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

        if (jumpDone && feet.isGrounded)
            nbJumpDone = 0;
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
            {
                if (feet.isGrounded && !airControl)
                    rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);
                else
                    rb.MovePosition(transform.position + moveDir * Time.fixedDeltaTime);
            }
        }
        else if (feet.isGrounded)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }

        if (jump && feet.isGrounded && rb.velocity.y == 0f && nbJumpDone == 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            jump = false;
            nbJumpDone++;
            Debug.Log("First Jump");
            if (maxJump == 1)
                StartCoroutine(WaitForNextJump());
            else
                StartCoroutine(WaitForJump());
        }
        else if (jump && nbJumpDone + 1 < maxJump && nbJumpDone != 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            jump = false;
            nbJumpDone++;
            StartCoroutine(WaitForJump());
            Debug.Log("Jump");
        }
        else if (jump && nbJumpDone + 1 == maxJump && nbJumpDone != 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            jump = false;
            nbJumpDone++;
            Debug.Log("Last Jump");
            StartCoroutine(WaitForNextJump());
        }
    }

    private IEnumerator WaitForNextJump()
    {
        float failTimer = 0.5f;
        float startTime = Time.time;

        while (feet.isGrounded && Time.time < startTime + failTimer)
            yield return null;
        while (!feet.isGrounded)
            yield return null;
        jumpDone = true;
        nbJumpDone = 0;
    }

    private IEnumerator WaitForJump()
    {
        float failTimer = 0.5f;
        float startTime = Time.time;

        while (feet.isGrounded && Time.time < startTime + failTimer)
            yield return null;
        jumpDone = true;
    }
}
