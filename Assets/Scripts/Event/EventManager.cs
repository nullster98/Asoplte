using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [Header("Main Component")]
    [SerializeField] TMP_Text EventText;
    [SerializeField] Image EventSprite;
    [SerializeField] Transform ButtonContainer;
    [SerializeField] GameObject ButtonPrefab;
    [SerializeField] ScrollRect eventScrollView;

    [Header("Event Data")]
    public EventDatabase eventDatabase;
    private EventHandler eventHandler;

    /*IEnumerator WaitForPlayer()
    {
        //  Player.Instance�� null�̸� ������ ������ ���
        while (Player.Instance == null)
        {
            Debug.LogWarning("Player �ν��Ͻ��� ���� �������� ����. ��� ��...");
            yield return null; // �� ������ ��� �� �ٽ� Ȯ��
        }

    }*/

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (eventDatabase == null)
            {
                Debug.LogError("EventDatabase�� ������� �ʾҽ��ϴ�!");
                return;
            }
            ResetEventDatabase();
            Debug.Log("�̺�Ʈ ���� ����");
            EventCreator.GenerateEvents(eventDatabase);

            eventHandler = new EventHandler(eventDatabase); //eventHandler �ʱ�ȭ
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        if (eventHandler == null)
        {
            Debug.LogError("EventHandler�� �ʱ�ȭ���� �ʾҽ��ϴ�!");
            return;
        }

        eventHandler.StartEvent("�����̺�Ʈ");
    }

    public void Update()
    {

    }



    public void UpdateEventUI(string eventDescription, List<EventChoice> choices, Sprite eventSprite)
    {
        EventText.text = eventDescription;
        EventSprite.sprite = eventSprite;

        // 2. ���� ��ư ���� (������ ������� ����)
        List<GameObject> buttonsToDestroy = new List<GameObject>();
        foreach (Transform child in ButtonContainer)
        {
            buttonsToDestroy.Add(child.gameObject);
        }
        foreach (GameObject button in buttonsToDestroy)
        {
            Destroy(button);
        }

        // 3. ���ο� ��ư ���� (������ ������� �ε����� ����)
        for (int i = 0; i < choices.Count; i++)
        {
            EventChoice choice = choices[i];

            GameObject newButton = Instantiate(ButtonPrefab, ButtonContainer);
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                buttonText.text = choice.ChoiceName;
            }
            else
            {
                Debug.LogError("ButtonPrefab�� TMP_Text�� �������� �ʽ��ϴ�!");
            }

            Button buttonComponent = newButton.GetComponent<Button>();

            if (buttonComponent != null)
            {
                int choiceIndex = i; // �ε����� �����ϰ� ����
                buttonComponent.onClick.AddListener(() => eventHandler.OnChoiceSelected(choiceIndex));
            }
            else
            {
                Debug.LogError("ButtonPrefab�� Button ������Ʈ�� �������� �ʽ��ϴ�!");
            }
        }

        // 4. ��ũ�� ���� ��ġ�� �ֻ������ �̵�
        if (eventScrollView != null)
        {
            eventScrollView.verticalNormalizedPosition = 1f;
        }
        else
        {
            Debug.LogWarning("eventScrollView�� �������� �ʾҽ��ϴ�!");
        }
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        eventHandler.OnChoiceSelected(choiceIndex); // �÷��̾ ������ �̺�Ʈ ����
    }

    private void ResetEventDatabase()
    {
        if (eventDatabase != null)
        {
            eventDatabase.ResetDatabase();
            Debug.Log("���� ���� �� EventDatabase �ʱ�ȭ �Ϸ�!");
        }
        else
        {
            Debug.LogError("EventDatabase�� ã�� �� �����ϴ�!");
        }
    }


}
