using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[System.Serializable]
public class ImageList
{
    public Sprite Img; // 이미지 스프라이트
    public string Name; // 이름
    public GodType GodName;
    public Sprite BackgroundImg; //배경 이미지
    [TextArea]
    public string information; // 정보
}

     
public class GodUI : MonoBehaviour
{
    [SerializeField]
    private List<ImageList> ImgList = new List<ImageList>(); // 인스펙터에서 설정할 이미지 리스트

    private int currentIndex = 0;
    public Image mainImage; // 가운데 메인 이미지
    public Image leftImage; // 왼쪽 이미지
    public Image rightImage; // 오른쪽 이미지
    public TMP_Text nameText; // 이름 텍스트
    public TMP_Text infoText; // 정보 텍스트
    public Player player;

    [SerializeField] private GameObject GodSelect;
    [SerializeField] private Image GodBackground;
    [SerializeField] private GameObject CharacterSelect;
    [SerializeField] private GameObject SelectMenu;
    [SerializeField] private GameObject GameMenu;

    private void Start()
    {
        if (ImgList.Count > 0) // 리스트가 비어 있지 않은지 확인
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
        if (ImgList.Count == 0) return; // 리스트가 비어 있는지 확인
        currentIndex = (currentIndex + 1) % ImgList.Count;
        UpdateUI();
    }

    public void OnPreviousButtonPressed()
    {
        if (ImgList.Count == 0) return; // 리스트가 비어 있는지 확인
        currentIndex = (currentIndex - 1 + ImgList.Count) % ImgList.Count;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (ImgList.Count == 0) return; // 리스트가 비어 있는지 확인

        ImageList currentImage = ImgList[currentIndex];

        mainImage.sprite = currentImage.Img;
        nameText.text = currentImage.Name;
        infoText.text = currentImage.information;
        GodBackground.sprite = currentImage.BackgroundImg;

        // 왼쪽과 오른쪽 이미지 갱신
        int leftIndex = (currentIndex - 1 + ImgList.Count) % ImgList.Count;
        int rightIndex = (currentIndex + 1) % ImgList.Count;

        leftImage.sprite = ImgList[leftIndex].Img;
        rightImage.sprite = ImgList[rightIndex].Img;

        // 왼쪽과 오른쪽 이미지를 흑백으로 변환
        leftImage.color = Color.gray;
        rightImage.color = Color.gray;
    }

    public void GodSelectToCharacterSelect()//신앙선택에서 종족선택으로 넘어가는 버튼
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