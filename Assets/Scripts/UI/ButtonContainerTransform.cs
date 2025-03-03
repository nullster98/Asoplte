using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonContainerTransform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        RectTransform parentRect = transform.parent.GetComponent<RectTransform>();

        // X ��ġ�� �θ�(Content)�� �߾����� ���� �̵�
        float parentWidth = parentRect.rect.width;
        rect.anchoredPosition = new Vector2(parentWidth / 2, rect.anchoredPosition.y);

        // Width�� 0�� �Ǵ� ���� ����
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentWidth);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
