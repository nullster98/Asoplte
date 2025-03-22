using UnityEngine;

namespace UI
{
    public class ButtonContainerTransform : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            RectTransform rect = GetComponent<RectTransform>();
            RectTransform parentRect = transform.parent.GetComponent<RectTransform>();

            // X 위치를 부모(Content)의 중앙으로 강제 이동
            float parentWidth = parentRect.rect.width;
            rect.anchoredPosition = new Vector2(parentWidth / 2, rect.anchoredPosition.y);

            // Width가 0이 되는 문제 방지
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentWidth);
        }
        
    }
}
