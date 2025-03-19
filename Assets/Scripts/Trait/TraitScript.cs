using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using UnityEngine.SceneManagement;

public enum TraitPnN
{
    Positive,
    Negative
}

public enum TraitAnP
{
    Active,
    Passive,
    Both
}

[System.Serializable]
public class Trait // Ư�� Ŭ����
{
    public string TraitName; //Ư�� �̸�
    public TraitPnN PnN; //���� �� ���� ����
    public TraitAnP AnP; //�нú� �� ��Ƽ�� ����
    public Sprite TraitImg; //Ư�� �̹���
    [TextArea]
    public string TraitDescription; //Ư�� ����
    public int Cost; // Ư�� �ڽ�Ʈ

}

public class TraitScript : MonoBehaviour
{
    [Header("ObjectControll")]
    [SerializeField] GameObject CharacterSelect;
    [SerializeField] GameObject PostivePannel;
    [SerializeField] GameObject NegativePannel;
    [SerializeField] Button PositiveBtn;
    [SerializeField] Button NegativeBtn;

    [Header("TraitControll")]
    [SerializeField] List<Trait> TraitsList = new List<Trait>();
    [SerializeField] private Transform PbuttonContainer; // Scroll View�� Content
    [SerializeField] private Transform NbuttonContainer; // Scroll View�� Content
    [SerializeField] Button ButtonPrefab; //��ư ������
    [SerializeField] TMP_Text DescriptionArea; // Ư�� ����â
    [SerializeField] TMP_Text CostArea; //�ڽ�Ʈ ǥ��â

    [Header("SelectControll")]
    [SerializeField] Button SelectButtonPrefab;
    [SerializeField] Transform SelectContainer;
    [SerializeField] int TotalCost;
    [SerializeField] int MaxCost;
    private List<Button> selectButtonList = new List<Button>(); // ���õ� ��ư���� ������ ����Ʈ
    //private int nextSelectIndex = 0; // ���õ� ��ư�� ���� �� �ڸ� �ε���
    private Sprite originalSelectedSprite;
    private List<Trait> selectedTraits = new List<Trait>(); // ���õ� Ư������ �����ϴ� ����Ʈ

    // Start is called before the first frame update
    void Start()
    {

        originalSelectedSprite = SelectButtonPrefab.image.sprite;
        // �ʱ� ���� - ���� �� �� ��ư ��� �⺻ ��������
        //SetButtonColors(PostiveBtn, Color.black, Color.white); // Positive ��ư�� ��� ���, ������ ���ڷ� ����
        //SetButtonColors(NegavieBtn, Color.white, Color.black); // Negative ��ư�� ��� ���, ������ ���ڷ� ����

        TraitsList = TraitsList.OrderBy(t => t.Cost).ToList();
        CreateTraitButtons();
        CreateSelectButtons();


    }

    // Update is called once per frame
    void Update()
    {
        CostArea.text = "Cost : " + TotalCost.ToString() + " / " + Player.Instance.MaxCost;
    }

    public void PostiveBtnAction() //Positive ��ư�� ������ ��
    {
        // Positive ��ư ����: ���� ���, �� ����
        //SetButtonColors(PostiveBtn, Color.black, Color.white);

        // Negative ��ư ����: �� ���, ���� ����
        //SetButtonColors(NegavieBtn, Color.white, Color.black);

        PostivePannel.SetActive(true);
        NegativePannel.SetActive(false);
        PositiveBtn.transform.SetAsLastSibling();
        PostivePannel.transform.SetAsLastSibling();

    }

    public void NegativeBtnAction() //Negative ��ư�� ������ ��
    {
        // Negative ��ư ����: ���� ���, �� ����
        //SetButtonColors(NegavieBtn, Color.black, Color.white);

        // Positive ��ư ����: �� ���, ���� ����
        //SetButtonColors(PostiveBtn, Color.white, Color.black);

        PostivePannel.SetActive(false);
        NegativePannel.SetActive(true);
        NegativeBtn.transform.SetAsLastSibling();
        NegativePannel.transform.SetAsLastSibling();
    }



    /*private void SetButtonColors(Button button, Color backgroundColor, Color textColor)
    {
        // ��ư�� ��� �̹��� ���� ����
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = backgroundColor;
        }
        else
        {
            Debug.LogWarning("Button�� Image ������Ʈ�� �����ϴ�.");
        }

        // ��ư�� �ڽ� ������Ʈ�� �ִ� TMP_Text ������Ʈ�� ���� ����
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.color = textColor;
        }
        else
        {
            Debug.LogWarning("Button�� TMP_Text ������Ʈ�� �����ϴ�.");
        }
    }*/


    // Ư�� ��ư ���� �� �ʱ�ȭ
    private void CreateTraitButtons()
    {
        // ����Ʈ�� ��� �ִ��� üũ
        if (TraitsList == null || TraitsList.Count == 0)
        {
            Debug.LogWarning("Race list is empty!");
            return;
        }

        for (int i = 0; i < TraitsList.Count; i++)
        {
            // Positive�� Negative�� ���� �ٸ� Container�� �߰�
            Transform targetContainer = TraitsList[i].PnN == TraitPnN.Positive ? PbuttonContainer : NbuttonContainer;

            // ��ư �������� Content�� �߰�
            Button button = Instantiate(ButtonPrefab, targetContainer);

            // ��ư�� �̹��� ������Ʈ�� Ư�� �̹��� ����
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.sprite = TraitsList[i].TraitImg;

            // ��ư Ŭ�� �̺�Ʈ ���� (���� ������ ���� �� �߰�)
            int index = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => PushTrait(TraitsList[index]));
        }

    }

    // Select ��ư�� �����ϰ� ���� �̹����� ������ �� �ֵ��� ����
    private void CreateSelectButtons()
    {
        for (int i = 0; i < 10; i++)
        {
            // Select ��ư�� ����
            Button button = Instantiate(SelectButtonPrefab, SelectContainer);
            selectButtonList.Add(button); // ����Ʈ�� �߰�

            // Ŭ�� �� ���� �̹����� �����ϴ� ��� �߰�
            int index = i; // �ε����� ����
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => RemoveTrait(index)); // ���� ���
        }
    }

    // Ŭ���� SelectButton�� ���� �̹����� �����ϴ� �޼���
    private void RestoreOriginalImage(Button selectButton)
    {
        selectButton.image.sprite = originalSelectedSprite; // ���� �̹����� ����
    }

    private void RemoveTrait(int index)
    {
        if (index < selectedTraits.Count)
        {
            Trait trait = selectedTraits[index];

            int traitCost = selectedTraits[index].Cost;
            // ���� ���
            selectedTraits.RemoveAt(index);
            if (trait.PnN == TraitPnN.Positive)
            {
                TotalCost -= traitCost;
            }
            else if (trait.PnN == TraitPnN.Negative)
            {
                TotalCost += traitCost;
            }

            // ��ư�� ���� �̹����� ����
            ReorganizeSelectButtons();
        }
    }

    // ���õ� Trait���� �տ������� �ٽ� ����
    private void ReorganizeSelectButtons()
    {
        // ��� SelectButton�� �̹����� ���� �̹����� ����
        foreach (var button in selectButtonList)
        {
            button.image.sprite = originalSelectedSprite;
        }

        // ���õ� Ư������ �ٽ� �տ������� ���ʴ�� ��ġ
        for (int i = 0; i < selectedTraits.Count; i++)
        {
            Button selectButton = selectButtonList[i];
            selectButton.image.sprite = selectedTraits[i].TraitImg;
        }
    }

    private void PushTrait(Trait selectedTrait)
    {
        // Ư�� ���� ������Ʈ
        DescriptionArea.text = $"{selectedTrait.TraitName}\n[Cost: {selectedTrait.Cost}]\n\n{selectedTrait.TraitDescription}";
        //�ߺ� ���� Ȯ��
        for (int i = 0; i < selectButtonList.Count; i++)
        {
            if (selectedTraits.Contains(selectedTrait))
            {
                Debug.Log("�ߺ�����");
                return;
            }
        }
        // ���� ������ �� �ڸ��� �ִ��� Ȯ��
        if (selectedTraits.Count < selectButtonList.Count)
        {
            if (Player.Instance.MaxCost < TotalCost + selectedTrait.Cost)
            {
                Debug.Log("�ڽ�Ʈ ����");
                return;
            }
            // Ư���� ���� ����Ʈ�� �߰� (������ ���߿�)
            selectedTraits.Add(selectedTrait);

            // Ư�� ����� TotalCost�� �߰�
            if (selectedTrait.PnN == TraitPnN.Positive)
            {
                TotalCost += selectedTrait.Cost;
            }

            else if (selectedTrait.PnN == TraitPnN.Negative)
            {
                TotalCost -= selectedTrait.Cost;
            }
            // ���õ� Ư���� �̹����� �ϴ� ��ư�� �ݿ�
            for (int i = 0; i < selectedTraits.Count; i++)
            {
                Button selectButton = selectButtonList[i];
                selectButton.image.sprite = selectedTraits[i].TraitImg; // ���õ� Ư�� �̹��� ����
            }

            // �ϴ� ��ư�� ���õ� Ư���� �̹����� ������Ʈ ��, �������� ���� ���·� ����
            ReorganizeSelectButtons();
        }
        else
        {
            Debug.Log("�� �̻� ������ �� �ִ� ��ư�� �����ϴ�.");
        }


    }

    public void OnNextButtonPressed()
    {
        // ���õ� Ư������ �÷��̾�� ����
        Player.Instance.ApplySelectedTraits(selectedTraits);


        // ���� ȭ������ ��ȯ (��: Ư�� ������ ���� �� ĳ���� ���� ȭ��)
       SceneManager.LoadScene("GameScene");
    }

    public void PreviousButtion()
    {
        gameObject.SetActive(false);
        CharacterSelect.SetActive(true);
    }

}


