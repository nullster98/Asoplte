using UnityEngine;
using UnityEngine.UI;

public class ScrollViewManager : MonoBehaviour
{
    public GameObject buttonPrefab;  // 버튼 프리팹 (이미지만 있는 버튼)
    public Transform content;        // Scroll View의 Content
    public Sprite[] buttonImages;    // 버튼에 할당할 이미지 배열

    void Start()
    {
        // 이미지 배열 크기만큼 버튼을 동적으로 생성
        for (int i = 0; i < buttonImages.Length; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, content); // Content의 자식으로 버튼 생성
            Image buttonImage = newButton.GetComponent<Image>();       // 버튼의 Image 컴포넌트 참조
            buttonImage.sprite = buttonImages[i];                      // 이미지 배열에서 이미지를 할당
        }
    }
}