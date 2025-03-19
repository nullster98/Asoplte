using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[System.Serializable]
public class ImageList
{
    public Sprite Img; // �̹��� ��������Ʈ
    public string Name; // �̸�
    public GodType GodName;
    public Sprite BackgroundImg; //��� �̹���
    [TextArea]
    public string information; // ����
}

     
public class GodUI : MonoBehaviour
{
    [SerializeField]
    private List<ImageList> ImgList = new List<ImageList>(); // �ν����Ϳ��� ������ �̹��� ����Ʈ

    private int currentIndex = 0;
    public Image mainImage; // ��� ���� �̹���
    public Image leftImage; // ���� �̹���
    public Image rightImage; // ������ �̹���
    public TMP_Text nameText; // �̸� �ؽ�Ʈ
    public TMP_Text infoText; // ���� �ؽ�Ʈ
    public Player player;

    [SerializeField] private GameObject GodSelect;
    [SerializeField] private Image GodBackground;
    [SerializeField] private GameObject CharacterSelect;
    [SerializeField] private GameObject SelectMenu;
    [SerializeField] private GameObject GameMenu;

    private void Start()
    {
        if (ImgList.Count > 0) // ����Ʈ�� ��� ���� ������ Ȯ��
        {
            UpdateUI();
        }
        else
        {
            Debug.LogWarning("ImgList is empty! Please add images in the inspector.");
        }
    }

    public void OnNextButtonPressed()
    {
        if (ImgList.Count == 0) return; // ����Ʈ�� ��� �ִ��� Ȯ��
        currentIndex = (currentIndex + 1) % ImgList.Count;
        UpdateUI();
    }

    public void OnPreviousButtonPressed()
    {
        if (ImgList.Count == 0) return; // ����Ʈ�� ��� �ִ��� Ȯ��
        currentIndex = (currentIndex - 1 + ImgList.Count) % ImgList.Count;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (ImgList.Count == 0) return; // ����Ʈ�� ��� �ִ��� Ȯ��

        ImageList currentImage = ImgList[currentIndex];

        mainImage.sprite = currentImage.Img;
        nameText.text = currentImage.Name;
        infoText.text = currentImage.information;
        GodBackground.sprite = currentImage.BackgroundImg;

        // ���ʰ� ������ �̹��� ����
        int leftIndex = (currentIndex - 1 + ImgList.Count) % ImgList.Count;
        int rightIndex = (currentIndex + 1) % ImgList.Count;

        leftImage.sprite = ImgList[leftIndex].Img;
        rightImage.sprite = ImgList[rightIndex].Img;

        // ���ʰ� ������ �̹����� ������� ��ȯ
        leftImage.color = Color.gray;
        rightImage.color = Color.gray;
    }

    public void GodSelectToCharacterSelect()//�žӼ��ÿ��� ������������ �Ѿ�� ��ư
    {
        GodSelect.SetActive(false);
        CharacterSelect.SetActive(true);
        player.God = ImgList[currentIndex].GodName;
    }

    public void PreviousButton()
    {
        GameMenu.SetActive(false);
        SelectMenu.SetActive(true);
    }
}