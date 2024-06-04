using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjTha
{
    public class Movement : MonoBehaviour
    {
        [SerializeField]
        private float movementSpeed;

        [SerializeField]
        private Rigidbody2D rigidbodyRef;

        private GameActions inputActions;

        private Vector2 inputValue;

        protected void Start()
        {
            inputActions.Standard.Navigation.performed += OnLocomotionPerformed;
            inputActions.Standard.Navigation.canceled += OnLocomotionCancelled;
        }

        protected void OnEnable()
        {
            inputActions ??= new();

            inputActions.Enable();
        }

        protected void OnDisable()
        {
            inputActions.Disable();
        }

        protected void FixedUpdate()
        {
            rigidbodyRef.velocity = inputValue * movementSpeed;
        }

        private void OnLocomotionCancelled(InputAction.CallbackContext context)
        {
            inputValue = Vector2.zero;
        }


        private void OnLocomotionPerformed(InputAction.CallbackContext context)
        {
            inputValue = context.ReadValue<Vector2>();
        }
    }
}