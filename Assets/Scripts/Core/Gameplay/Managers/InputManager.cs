using System;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Core.Gameplay.Managers
{
    public class InputManager : MonoBehaviour, InputActions.IGameplayActions
    {
        public static event Action<InputAction.CallbackContext> OnConfirmInput;
        public static event Action<InputAction.CallbackContext> OnCancelInput;
        public static event Action<InputAction.CallbackContext> OnBuildModeInput;
        public static event Action<InputAction.CallbackContext> OnPositionInput;
        public static event Action<InputAction.CallbackContext> OnPauseInput;
        public static event Action<InputAction.CallbackContext> OnNormalSpeedInput;
        public static event Action<InputAction.CallbackContext> OnFastSpeedInput;
        public static event Action<InputAction.CallbackContext> OnFastestSpeedInput;
        
        private InputActions _controls;
        private void Awake()
        {
            if (_controls == null)
            {
                _controls = new InputActions();
                _controls.Gameplay.SetCallbacks(this);
            }
            _controls.Enable();
        }

        private void OnDestroy()
        {
            _controls.Disable();
        }

        public void OnConfirm(InputAction.CallbackContext context)
        {
            OnConfirmInput?.Invoke(context);
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            OnCancelInput?.Invoke(context);
        }

        public void OnBuildMode(InputAction.CallbackContext context)
        {
           OnBuildModeInput?.Invoke(context); 
        }

        public void OnPosition(InputAction.CallbackContext context)
        {
            OnPositionInput?.Invoke(context);
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            OnPauseInput?.Invoke(context);
        }

        public void OnNormalSpeed(InputAction.CallbackContext context)
        {
            OnNormalSpeedInput?.Invoke(context);
        }

        public void OnFastSpeed(InputAction.CallbackContext context)
        {
            OnFastSpeedInput?.Invoke(context);
        }

        public void OnFastestSpeed(InputAction.CallbackContext context)
        {
            OnFastestSpeedInput?.Invoke(context);
        }
    }
}
