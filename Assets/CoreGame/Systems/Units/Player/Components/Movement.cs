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

        public Vector2 GetInputValue { get => inputValue; }

        protected void Start()
        {
            inputActions.Standard.Navigation.performed += OnLocomotionPerformed;
            inputActions.Standard.Navigation.canceled += OnLocomotionCancelled;
        }

        //!TODO REMOVE THIS HAX
        protected void Update()
        {
            if (GameManager.PauseElementMovement && inputValue != Vector2.zero)
            {
                inputValue = Vector2.zero;
                rigidbodyRef.velocity = Vector2.zero;
            }
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

        public void CustomFixedTick()
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