using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

 
public class GodUI : MonoBehaviour
{
    [SerializeField] private GodDatabase godDatabase;
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
        if (godDatabase == null || godDatabase.GodList.Count == 0)
        {
            Debug.LogWarning("GodDatabase가 비어 있습니다! 인스펙터에서 설정하세요.");
            return;
        }

        UpdateUI();
    }

    public void OnNextButtonPressed()
    {
        if (godDatabase.GodList.Count == 0) return;
        currentIndex = (currentIndex + 1) % godDatabase.GodList.Count;
        UpdateUI();
    }

    public void OnPreviousButtonPressed()
    {
        if (godDatabase.GodList.Count == 0) return;
        currentIndex = (currentIndex - 1 + godDatabase.GodList.Count) % godDatabase.GodList.Count;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (godDatabase.GodList.Count == 0) return; // 리스트가 비어 있는지 확인

        GodData currentImage = godDatabase.GetGodByIndex(currentIndex);

        mainImage.sprite = currentImage.GetGodImage();
        nameText.text = currentImage.name;
        infoText.text = currentImage.GetDescription();
        GodBackground.sprite = currentImage.GetBackgroundImage();

        // 왼쪽과 오른쪽 이미지 갱신
        int leftIndex = (currentIndex - 1 + godDatabase.GodList.Count) % godDatabase.GodList.Count;
        int rightIndex = (currentIndex + 1) % godDatabase.GodList.Count;

        leftImage.sprite = godDatabase.GetGodByIndex(leftIndex).GetGodImage();
        rightImage.sprite = godDatabase.GetGodByIndex(rightIndex).GetGodImage();

        // 왼쪽과 오른쪽 이미지를 흑백으로 변환
        leftImage.color = Color.gray;
        rightImage.color = Color.gray;
    }

    public void GodSelectToCharacterSelect()//신앙선택에서 종족선택으로 넘어가는 버튼
    {
        GodSelect.SetActive(false);
        CharacterSelect.SetActive(true);
        player.SelectedGod = godDatabase.GetGodByIndex(currentIndex);
    }

    public void PreviousButton()
    {
        GameMenu.SetActive(false);
        SelectMenu.SetActive(true);
    }
}