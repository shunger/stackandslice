using UnityEngine;

namespace SliceAndStack.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreen : MonoBehaviour
    {
        protected CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            // DOTween: _canvasGroup.DOFade(1f, 0.3f);
        }

        public virtual void Hide()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
            // DOTween: _canvasGroup.DOFade(0f, 0.2f).OnComplete(() => gameObject.SetActive(false));
        }

        public bool IsVisible => gameObject.activeInHierarchy && _canvasGroup.alpha > 0f;
    }
}
