using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Elements.Systems
{
    public class InputSystem : MonoBehaviour
    {
        public event Action<Vector2> OnClickDownEvent;
        public event Action<Vector2> OnClickUpEvent;

        [SerializeField] private InputAction clickAction;
        [SerializeField] private InputAction screenPositionAction;
        
        private void OnEnable()
        {
            if (clickAction != null)
            {
                clickAction.started += OnClickStarted;
                clickAction.canceled += OnClickCanceled;
                clickAction.Enable();
                screenPositionAction.Enable();
            }
        }

        private void OnDisable()
        {
            if (clickAction != null)
            {
                clickAction.started -= OnClickStarted;
                clickAction.canceled -= OnClickCanceled;
                clickAction.Disable();
                screenPositionAction.Disable();
            }
        }

        private void OnClickStarted(InputAction.CallbackContext context)
        {
            OnClickDownEvent?.Invoke(screenPositionAction.ReadValue<Vector2>());
        }

        private void OnClickCanceled(InputAction.CallbackContext context)
        {
            OnClickUpEvent?.Invoke(screenPositionAction.ReadValue<Vector2>());
        }
    }
}