using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Core.UI
{
    public abstract class UIPopup : MonoBehaviour
    {
        [SerializeField] protected FadeProperties fadeProperties;
        public UnityEvent OnShow;
        public UnityEvent OnHide;
        public bool isActive;
        public TransitionPhase phase;
        protected Coroutine FadeCoroutine;

        [Serializable]
        public enum TransitionPhase
        {
            None,
            FadingIn,
            FadingOut
        }

        public virtual void Show()
        {
            Show_Internal();
        }
        protected virtual void Show_Internal()
        {
            phase = TransitionPhase.FadingIn;
            Fade(true, OnShowCallback);
        }
        public virtual void InstantShow()
        {
            SetVisibility(1f, true);
            OnShowCallback();
        }
        public virtual void Hide()
        {
            Hide_Internal();
        }
        protected virtual void Hide_Internal()
        {
            phase = TransitionPhase.FadingOut;
            Fade(false, OnHideCallback);    
        }
        
        public virtual void InstantHide()
        {
            SetVisibility(0f, false);
            OnHideCallback();
        }
        public virtual void Toggle()
        {
            switch (isActive)
            {
                case true:
                    Hide();
                    break;
                case false:
                    Show();
                    break;
            }
        }
        public virtual void ShowHide(bool show, bool instant = false)
        {
            switch (show)
            {
                case true:
                    if (instant)
                    {
                        InstantShow();
                    }
                    else
                    {
                        Show();
                    }
                    break;
                case false:
                    if (instant)
                    {
                        InstantHide();
                    }
                    else
                    {
                        Hide();
                    }

                    break;
            }
        }

        protected abstract void SetVisibility(float alpha, bool active);
        protected virtual void OnHideCallback()
        {
            isActive = false;
            OnHide?.Invoke();
        }
        
        protected virtual void OnShowCallback()
        {
            isActive = true;
            OnShow?.Invoke();
        }
        protected virtual void Fade(bool value, Action callback)
        {
            if (FadeCoroutine != null) StopCoroutine(FadeCoroutine);
            FadeCoroutine = StartCoroutine(FadeRoutine(value, callback));
        }

        protected virtual IEnumerator FadeRoutine(bool value, Action callback)
        {
            var initialValue = value ? 0f : 1f;
            var endValue = value ? 1f : 0f;
            SetVisibility(initialValue, !value);
            
            for (var i = 0f;
                 i < 1f;
                 i += (fadeProperties.unscaled ? Time.unscaledDeltaTime : Time.deltaTime) /
                      fadeProperties.fadeDuration)
            {
                var alpha = Mathf.Lerp(initialValue, endValue, i);
                SetVisibility(alpha, !value);
                yield return new WaitForEndOfFrame();
            }
            SetVisibility(endValue, value);
            callback?.Invoke();
            phase = TransitionPhase.None;
        }

        

    }
    [Serializable]
    public struct FadeProperties
    {
        public float fadeDuration;
        public bool unscaled;
    }
}