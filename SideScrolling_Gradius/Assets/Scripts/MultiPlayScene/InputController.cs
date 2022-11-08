using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    // Get Component In Children
    private MovementController movementController;
    private WeaponController weaponController;
    private Animator animator;

    private Vector2 direction = Vector2.zero;

    [HideInInspector] public float hInput;
    [HideInInspector] public float vInput;
    [HideInInspector] public bool fireInput = false;
    [HideInInspector] public bool inputChange = false;

    public KeyCode fireKeyCode = KeyCode.None;

    private void Start()
    {
        movementController = GetComponentInChildren<MovementController>();
        weaponController = GetComponentInChildren<WeaponController>();
        animator = GetComponentInChildren<Animator>();

        //fire keycode setting
        fireKeyCode = KeyCode.Space;
    }

    private void Update()
    {
        InputControl();
    }

    private void InputControl()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var fire = Input.GetKeyDown(fireKeyCode);

        //Preventing you from leaving the screen in the event of a collision with "Border"
        if ((movementController.GetCheckBorder() == 2 && horizontal == 1) ||
            (movementController.GetCheckBorder() == -2 && horizontal == -1))
        {
            horizontal = 0;
        }
        if ((movementController.GetCheckBorder() == 1 && vertical == 1) ||
            (movementController.GetCheckBorder() == -1 && vertical == -1))
        {
            vertical = 0;
        }

        inputChange = (horizontal != hInput || vertical != vInput || fire != fireInput);

        hInput = horizontal;
        vInput = vertical;
        fireInput = fire;

        movementController.SetDirectionMovement(hInput, vInput);
        AnimationControl(); //animation play

        if (fireInput)
        {
            weaponController.AttackFire();
        }
    }

    private void AnimationControl()
    {
        direction = new Vector2(hInput, vInput);

        animator.SetInteger("isVertical", (int)direction.y);
    }
}

