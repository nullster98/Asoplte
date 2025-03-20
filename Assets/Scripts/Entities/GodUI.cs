using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

 
public class GodUI : MonoBehaviour
{
    [SerializeField] private GodDatabase godDatabase;
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
        if (godDatabase == null || godDatabase.GodList.Count == 0)
        {
            Debug.LogWarning("GodDatabase�� ��� �ֽ��ϴ�! �ν����Ϳ��� �����ϼ���.");
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
        if (godDatabase.GodList.Count == 0) return; // ����Ʈ�� ��� �ִ��� Ȯ��

        GodData currentImage = godDatabase.GetGodByIndex(currentIndex);

        mainImage.sprite = currentImage.GetGodImage();
        nameText.text = currentImage.name;
        infoText.text = currentImage.GetDescription();
        GodBackground.sprite = currentImage.GetBackgroundImage();

        // ���ʰ� ������ �̹��� ����
        int leftIndex = (currentIndex - 1 + godDatabase.GodList.Count) % godDatabase.GodList.Count;
        int rightIndex = (currentIndex + 1) % godDatabase.GodList.Count;

        leftImage.sprite = godDatabase.GetGodByIndex(leftIndex).GetGodImage();
        rightImage.sprite = godDatabase.GetGodByIndex(rightIndex).GetGodImage();

        // ���ʰ� ������ �̹����� ������� ��ȯ
        leftImage.color = Color.gray;
        rightImage.color = Color.gray;
    }

    public void GodSelectToCharacterSelect()//�žӼ��ÿ��� ������������ �Ѿ�� ��ư
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