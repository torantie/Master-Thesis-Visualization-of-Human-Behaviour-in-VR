using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimationController : MonoBehaviour
{
    public Animator animator;

    public string anim_var_grip = "Flex";
    public string anim_var_pinch = "Pinch";
    public InputActionReference gripTrigger;
    public InputActionReference pinchTrigger;
         

    // Start is called before the first frame update
    void Start()
    {
        gripTrigger.action.performed += onGripPerformed;
        gripTrigger.action.started += onGripStarted;
        gripTrigger.action.canceled += onGripCancel;

        pinchTrigger.action.performed += onPinchPerformed;
        pinchTrigger.action.started += onPinchStarted;
        pinchTrigger.action.canceled += onPinchCancel;
    }


    // ### Grip
    private void onGripStarted(InputAction.CallbackContext obj)
    {
        //Debug.Log("1 - Grip - Started");
    }

    private void onGripPerformed(InputAction.CallbackContext obj)
    {
        //Debug.Log("2 - Grip - Performed");

        // btn down (full)
        animator.SetFloat(anim_var_grip, 1.0f);
    }

    private void onGripCancel(InputAction.CallbackContext obj)
    {
        //Debug.Log("3 - Grip - Cancel");

        // btn up
        animator.SetFloat(anim_var_grip, 0.0f);
    }


    // ### Pinch
    private void onPinchStarted(InputAction.CallbackContext obj)
    {
        //Debug.Log("1 - Pinch - Started");
    }

    private void onPinchPerformed(InputAction.CallbackContext obj)
    {
        //Debug.Log("2 - Pinch - Performed");

        // btn down (full)
        animator.SetFloat(anim_var_pinch, 1.0f);
    }

    private void onPinchCancel(InputAction.CallbackContext obj)
    {
        //Debug.Log("3 - Pinch - Cancel");

        // btn up
        animator.SetFloat(anim_var_pinch, 0.0f);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
