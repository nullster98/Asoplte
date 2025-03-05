using UnityEngine;
using UnityEngine.UI;

public class ScrollViewManager : MonoBehaviour
{
    public GameObject buttonPrefab;  // ��ư ������ (�̹����� �ִ� ��ư)
    public Transform content;        // Scroll View�� Content
    public Sprite[] buttonImages;    // ��ư�� �Ҵ��� �̹��� �迭

    void Start()
    {
        // �̹��� �迭 ũ�⸸ŭ ��ư�� �������� ����
        for (int i = 0; i < buttonImages.Length; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, content); // Content�� �ڽ����� ��ư ����
            Image buttonImage = newButton.GetComponent<Image>();       // ��ư�� Image ������Ʈ ����
            buttonImage.sprite = buttonImages[i];                      // �̹��� �迭���� �̹����� �Ҵ�
        }
    }
}