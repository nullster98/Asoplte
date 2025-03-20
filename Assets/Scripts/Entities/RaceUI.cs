using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
public class DerivedCharacter
{
    public string CharacterName;
    public Sprite CharacterImg;
    public Sprite OnImg;
    public Sprite OffImg;

    [TextArea]
    public string CharacterDescription;
    [TextArea]
    public string CharacterStat;

    [TextArea]
    public string UnlockHint;
    public float RequireFaith; // �ر� ���
    public bool IsUnlocked = false; // Ȱ��ȭ ����
}

[System.Serializable]
public class Tribe
{
    public Sprite RaceImg;
    public Sprite OffRaceImg;
    public string RaceName;
    //public Race race;
    public float RequireFaith; // �ر� ���

    // �Ļ� ĳ���� ����Ʈ �߰�
    public List<DerivedCharacter> DerivedCharacters;

    public bool IsUnlocked = false; // Ȱ��ȭ ����
}

public class RaceUI : MonoBehaviour
{
    [SerializeField] private Image MainImg; // ���õ� ������ ���� �̹���
    [SerializeField] private TMP_Text NameArea; // ���õ� ���� �̸�
    [SerializeField] private TMP_Text DescriptionArea; // ���� ����
    [SerializeField] private List<Tribe> raceList = new List<Tribe>(); // ���� ����Ʈ
    [SerializeField] private GameObject RaceCollection; // ���� ���� â
    [SerializeField] private Transform buttonContainer; // Scroll View�� Content
    [SerializeField] private Button raceButtonPrefab; // ��ư ������
    [SerializeField] private GameObject TratiSelect;
    [SerializeField] private GameObject GodSelect;
    [SerializeField] private Button unlockButton; // �ر� ��ư

    private int currentRaceIndex = 0; // ���õ� ������ �ε���
    private int currentCharacterIndex = 0; // ���õ� �Ļ� ĳ������ �ε���
    [SerializeField] private Sprite QuestionImg;
    [SerializeField] private Image LeftImg;
    [SerializeField] private Image RightImg;
    [SerializeField] private Image MidImg;
    [SerializeField] private Button LeftButton;
    [SerializeField] private Button RightButton;
    [SerializeField] private bool isFirst = false;

    private void Start()
    {
        CreateRaceButtons();
        HideDerivation();
    }

    // ���� ��ư ���� �� �ʱ�ȭ
    private void CreateRaceButtons()
    {
        if (raceList == null || raceList.Count == 0)
        {
            Debug.LogWarning("Race list is empty!");
            return;
        }

        for (int i = 0; i < raceList.Count; i++)
        {
            Button button = Instantiate(raceButtonPrefab, buttonContainer);
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.sprite = raceList[i].RaceImg;
            int index = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectRace(index));
        }
    }

    // ���� ���� �޼���
    private void SelectRace(int raceIndex)
    {
        currentRaceIndex = raceIndex;
        currentCharacterIndex = 0; // �׻� ù ��° �Ļ� ĳ���͸� ������

        Tribe selectedRace = raceList[currentRaceIndex];
        MainImg.sprite = selectedRace.RaceImg;
        NameArea.text = selectedRace.RaceName;
        //Player.Instance.Race = selectedRace.race;
        RaceCollection.SetActive(false);

        EnsureMinimumCharacters(selectedRace);

        // ù ��° �Ļ� ĳ���� UI ������Ʈ
        if (selectedRace.DerivedCharacters.Count > 0)
        {
            SelectDerivedCharacter(0);
        }
    }

    private void EnsureMinimumCharacters(Tribe tribe)
    {
        // �Ļ� ĳ���Ͱ� 3�� �̸��̸� �ڵ����� �߰�
        while (tribe.DerivedCharacters.Count < 3)
        {
            DerivedCharacter placeholderCharacter = new DerivedCharacter
            {
                CharacterName = "Unknown",
                CharacterImg = QuestionImg,
                OnImg = QuestionImg,
                OffImg = QuestionImg,
                RequireFaith = 0f, // �⺻�� ����
                CharacterDescription = "???",
                CharacterStat = "???",
                UnlockHint = "�غ��� �Դϴ�."
            };

            tribe.DerivedCharacters.Add(placeholderCharacter);
        }
    }

    private void SelectDerivedCharacter(int characterIndex)
    {
        currentCharacterIndex = characterIndex;

        Tribe currentTribe = raceList[currentRaceIndex];
        if (currentTribe.DerivedCharacters.Count == 0)
            return;

        DerivedCharacter selectedCharacter = currentTribe.DerivedCharacters[currentCharacterIndex];
        // ĳ���Ͱ� �رݵǾ����� ���ο� ���� �ٸ� ������ ǥ��

        bool isUnknown = selectedCharacter.CharacterName == "Unknown";

        if(isUnknown)
        {
            MainImg.sprite = QuestionImg;
            NameArea.text = "???";
            DescriptionArea.text = "�غ����� ĳ�����Դϴ�.";
            ConfigureButton(false, 0f, true);
        }

        else if (selectedCharacter.IsUnlocked)
        {
            MainImg.sprite = selectedCharacter.CharacterImg;
            NameArea.text = selectedCharacter.CharacterName;
            DescriptionArea.text = $"{selectedCharacter.CharacterName} <sprite=0>\n\n{selectedCharacter.CharacterDescription}\n\n{selectedCharacter.CharacterStat}";

            // ��ư�� '���� �ܰ�'�� ����
            ConfigureButton(false);
        }
        else
        {
            MainImg.sprite = selectedCharacter.OffImg; // �رݵ��� ���� ��� OffImg ���
            NameArea.text = "???";
            DescriptionArea.text = $"{selectedCharacter.UnlockHint}\n\n�ر� ���: {selectedCharacter.RequireFaith} �ž� ��ȭ";

            // ��ư�� '�ر��ϱ�'�� ����
            ConfigureButton(true, selectedCharacter.RequireFaith);
        }

        UpdateUI();
    }

    private void ConfigureButton(bool isUnlock, float cost = 0f, bool isUnavailable = false)
    {
        if (isUnavailable)
        {
            //���� �Ұ� ��ư���� ����
            unlockButton.GetComponentInChildren<TMP_Text>().text = "���úҰ�";
            unlockButton.onClick.RemoveAllListeners();
            unlockButton.interactable = false;
        }

        else if (isUnlock)
        {
            // �ر��ϱ� ��ư ����
            unlockButton.GetComponentInChildren<TMP_Text>().text = "�ر��ϱ�";
            unlockButton.onClick.RemoveAllListeners();
            unlockButton.onClick.AddListener(UnlockCharacter);

            // �ž� ����Ʈ�� ������� Ȯ���Ͽ� ��ư Ȱ��ȭ/��Ȱ��ȭ
            unlockButton.interactable = Player.Instance.GetFaithPoint() >= cost;
        }
        else
        {
            // ���� �ܰ� ��ư ����
            unlockButton.GetComponentInChildren<TMP_Text>().text = "�����ܰ�";
            unlockButton.onClick.RemoveAllListeners();
            unlockButton.onClick.AddListener(ChcToTrait);
            unlockButton.interactable = true; // �׻� Ȱ��ȭ
        }

        unlockButton.gameObject.SetActive(true);
    }
    public void UnlockCharacter()
    {
        Tribe currentTribe = raceList[currentRaceIndex];
        DerivedCharacter selectedCharacter = currentTribe.DerivedCharacters[currentCharacterIndex];

        if (!selectedCharacter.IsUnlocked && Player.Instance.SpendFaith(selectedCharacter.RequireFaith))
        {
            selectedCharacter.IsUnlocked = true;
            Debug.Log($"{selectedCharacter.CharacterName}��(��) �رݵǾ����ϴ�!");

            // UI ������Ʈ
            SelectDerivedCharacter(currentCharacterIndex);
        }
        else
        {
            Debug.Log("�ž� ��ȭ�� �����մϴ�.");
            return;
        }
    }

    private void UpdateUI()
    {
        Tribe currentTribe = raceList[currentRaceIndex];
        if (currentTribe.DerivedCharacters.Count == 0)
            return;

        int count = currentTribe.DerivedCharacters.Count;
        int leftIndex = (currentCharacterIndex - 1 + count) % count;
        int rightIndex = (currentCharacterIndex + 1) % count;

        isFirst = true;
        HideDerivation();

        MidImg.sprite = currentTribe.DerivedCharacters[currentCharacterIndex].CharacterImg;
        LeftImg.sprite = currentTribe.DerivedCharacters[leftIndex].OffImg;
        RightImg.sprite = currentTribe.DerivedCharacters[rightIndex].OffImg;
    }

    public void ShowNextCharacter()
    {
        Tribe currentTribe = raceList[currentRaceIndex];
        if (currentTribe.DerivedCharacters.Count > 0)
        {
            currentCharacterIndex = (currentCharacterIndex + 1) % currentTribe.DerivedCharacters.Count;
            SelectDerivedCharacter(currentCharacterIndex);
        }
    }

    public void ShowPreviousCharacter()
    {
        Tribe currentTribe = raceList[currentRaceIndex];
        if (currentTribe.DerivedCharacters.Count > 0)
        {
            currentCharacterIndex = (currentCharacterIndex - 1 + currentTribe.DerivedCharacters.Count) % currentTribe.DerivedCharacters.Count;
            SelectDerivedCharacter(currentCharacterIndex);
        }
    }

    public void HideDerivation()
    {
        

        if (!isFirst)
        {
            LeftImg.gameObject.SetActive(false);
            RightImg.gameObject.SetActive(false);
            MidImg.gameObject.SetActive(false);
            LeftButton.interactable = false;
            RightButton.interactable = false;
        }

        else
        {
            LeftImg.gameObject.SetActive(true);
            RightImg.gameObject.SetActive(true);
            MidImg.gameObject.SetActive(true);
            LeftButton.interactable = true;
            RightButton.interactable = true;
        }
    }

    #region �̵���ư
    public void MainToSelect()
    {
        RaceCollection.SetActive(true);
    }

    public void ChcToTrait()
    {
        Player.Instance.PlayerImg = MainImg.sprite;
        Debug.Log($"PlayerImg �����: {Player.Instance.PlayerImg}"); // ����� �ڵ� �߰�

        //FindObjectOfType<PlayerUIManager>().UpdatePlayerUI(Player.Instance); // UI ������Ʈ
        this.gameObject.SetActive(false);
        TratiSelect.SetActive(true);
    }

    public void PreviousButton()
    {
        gameObject.SetActive(false);
        GodSelect.SetActive(true);
        isFirst = false;
        HideDerivation();
    }
    #endregion
}