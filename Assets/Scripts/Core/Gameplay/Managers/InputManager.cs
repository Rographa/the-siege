using System;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Core.Gameplay.Managers
{
    public class InputManager : MonoSingleton<InputManager>, InputActions.IGameplayActions
    {
        public static event Action<InputAction.CallbackContext> OnConfirmInput;
        public static event Action<InputAction.CallbackContext> OnCancelInput;
        public static event Action<InputAction.CallbackContext> OnBuildModeInput;
        public static event Action<InputAction.CallbackContext> OnMoveInput;
        public static event Action<InputAction.CallbackContext> OnPositionInput;
        public static event Action<InputAction.CallbackContext> OnNextInput;
        public static event Action<InputAction.CallbackContext> OnPreviousInput;
        public static event Action<InputAction.CallbackContext> OnNormalSpeedInput;
        public static event Action<InputAction.CallbackContext> OnFastSpeedInput;
        public static event Action<InputAction.CallbackContext> OnFastestSpeedInput;
        
        private InputActions _controls;
        protected override void Init()
        {
            base.Init();
            if (_controls == null)
            {
                _controls = new InputActions();
                _controls.Gameplay.SetCallbacks(this);
            }
            _controls.Enable();
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

        public void OnMove(InputAction.CallbackContext context)
        {
            OnMoveInput?.Invoke(context);
        }

        public void OnPosition(InputAction.CallbackContext context)
        {
            OnPositionInput?.Invoke(context);
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            OnNextInput?.Invoke(context);
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            OnPreviousInput?.Invoke(context);
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
