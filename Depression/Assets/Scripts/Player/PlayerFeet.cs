using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeet : MonoBehaviour
{
    [HideInInspector]
    public bool isGrounded = true;

    private int cnt;

    private void OnTriggerEnter(Collider other)
    {
        cnt++;
    }

    private void OnTriggerExit(Collider other)
    {
        cnt--;
    }

    private void Update()
    {
        if (cnt > 0)
            isGrounded = true;
        else
            isGrounded = false;
    }
}
