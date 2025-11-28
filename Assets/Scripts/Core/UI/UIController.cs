using UnityEngine;
using Utilities;

namespace Core.UI
{
    public class UIController : UIPopup
    {
        [GetComponent] protected CanvasGroup CanvasGroup;

        private void Start()
        {
            ComponentInjector.InjectComponents(this);
            ShowHide(isActive, true);
        }
        
        protected override void SetVisibility(float alpha, bool active)
        {
            if (CanvasGroup == null)
            {
                ComponentInjector.InjectComponents(this);
            }
            
            CanvasGroup.alpha = alpha;
            CanvasGroup.interactable = active;
            CanvasGroup.blocksRaycasts = active;
        }
    }
}