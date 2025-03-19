using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

#region �ž� ����Ʈ
public enum GodType
{
    Fire,
    Water,
    Light,
    Dark,
    None
}
#endregion

#region ���� ����Ʈ
public enum Race
{
    Human,
    Orc,
    Elf,
    Golem,
    None
}
#endregion

public class Player : MonoBehaviour
{
    //�̱���
    public static Player _instance;
    public static Player Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Player>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("Player");
                    _instance = obj.AddComponent<Player>();
                }
            }
            return _instance;
        }
    }

    //�÷��̾� �⺻ ����
    private Dictionary<string, float> stats = new Dictionary<string, float>()
    {
    { "Atk", 0 },          // ���ݷ�
    { "Def", 0 },          // ����
    { "HP", 0 },          // �ִ� ü��
    { "MP", 0 },          // �ִ� ����
    { "CurrentHP", 0 },   // ���� ü��
    { "CurrentMP", 0 },   // ���� ����
    { "MentalStat", 0 },  // ���ŷ�
    { "CurrentMentalStat", 0 }, // ���� ���ŷ�
    { "FaithStat", 0 },   // �žӽ�
    { "Gold", 1000f }        // ���

    };

    private Equipment[] equippedItem = new Equipment[6];
    private EquipUI equipUI;


    public int MaxCost { get; set; } // �ڽ�Ʈ
    public Sprite PlayerImg; //�÷��̾� �̹���

    public List<Trait> selectedTraits = new List<Trait>();

    private float FaithPoint { get; set; } = 500f;//�ž�����Ʈ
    private GodType _god = GodType.None;
    private Race _race = Race.None;

    public GodType God
    {
        get { return _god; }
        set
        {
            _god = value;
            ApplyGodBonuses(); // �� ���ÿ� ���� ���ʽ� ����
        }
    }

    public Race Race
    {
        get { return _race; }
        set
        {
            _race = value;
            ApplyRaceBonuses(); // ���� ���ÿ� ���� ���ʽ� ����
        }
    }

    private void Awake()
    {
        // �̱��� ���� ����
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("�÷��̾� ������ ����");

        }
        else
        {
            Debug.Log("�����Ͱ� �̹� ������");
            Destroy(gameObject);
        }

    }

    public void SetEquipUI(EquipUI uI)
    {
        equipUI = uI;
    }


    // Ư������ �Ѳ����� ����
    public void ApplySelectedTraits(List<Trait> traits)
    {
        foreach (var trait in traits)
        {
            ApplyTraitEffect(trait);
            selectedTraits.Add(trait); // ���õ� Ư�� ����Ʈ�� �߰�
        }
    }

    // Ư���� ȿ���� �����ϴ� �޼���
    private void ApplyTraitEffect(Trait trait)
    {
        switch (trait.TraitName)
        {
            case "��Ÿ�� ����":
                ChangeStat("Atk", GetStat("MP") * 0.1f);
                break;

            case "������ȯ":
                float excessHP = GetStat("CurrentHP") - GetStat("HP");
                if (excessHP > 0)
                {
                    ChangeStat("MP", excessHP);
                    ChangeStat("CurrentHP", -excessHP);
                }
                break;

            case "��ο� �þ�":
                // ���� ü���� ������ �ʵ��� ���� (UI ���� ������Ʈ �ʿ�)
                break;

            case "������ ����":
                // ������ ����ϴ� ��ų�� ü���� �Ҹ��ϰ� �� (��ų �ߵ� ���� �ݿ� �ʿ�)
                break;
        }
    }

    // ���õ� Ư���� ȿ���� �����ϴ� �޼��嵵 �ʿ��� ��� ���� ����


    void Start()
    {
        StartStat();
        MaxCost = 15;
    }

    void Update()
    {

    }
    #region FatihPoint Get,Set
    public string GetFatihString()
    {
        return FaithPoint.ToString("F0");
    }

    public float GetFaithPoint()
    {
        return FaithPoint;
    }


    public void SetFaithPoint(float value)
    {
        FaithPoint = value;
    }

    public bool SpendFaith(float amount)
    {
        if (FaithPoint >= amount)
        {
            FaithPoint -= amount;
            return true;
        }

        return false;
    }

    public void AddFatihPoint(float amount)
    {
        FaithPoint += amount;
    }
    #endregion

    private void ApplyGodBonuses() //�ž� ���ÿ� ���� �߰�ȿ��
    {
        switch (_god)
        {
            case GodType.Fire:
                ChangeStat("Atk", 10);
                ChangeStat("HP", 20);
                ChangeStat("CurrentHP", 20);
                break;
            case GodType.Water:
                ChangeStat("MP", 15);
                ChangeStat("CurrentMP", 15);
                ChangeStat("MentalStat", 10);
                ChangeStat("CurrnetMentalStat", 10);
                break;
            case GodType.Light:
                ChangeStat("HP", 30);
                ChangeStat("CurrentHP", 30);
                ChangeStat("FaithStat", 10);
                break;
        }
    }

    private void ApplyRaceBonuses() //���� ���ÿ� ���� �߰�ȿ��
    {
        switch (_race)
        {
            case Race.Human:
                ChangeStat("HP", 10);
                ChangeStat("MentalStat", 5);
                ChangeStat("CurrentHP", 10);
                ChangeStat("CurrnetMentalStat", 5);
                break;
            case Race.Elf:
                ChangeStat("MP", 20);
                ChangeStat("CurrnetMP", 20);
                ChangeStat("Atk", 5);
                break;
            case Race.Orc:
                ChangeStat("Atk", 15);
                ChangeStat("HP", 10);
                ChangeStat("CurrentHP", 10);
                break;
        }
    }

    void StartStat() //�ʱ��÷��̾� ����
    {
        ChangeStat("Atk", 10);
        ChangeStat("Def", 10);
        ChangeStat("HP", 100);
        ChangeStat("MP", 100);
        ChangeStat("CurrentHP", GetStat("HP"));
        ChangeStat("CurrentMP", GetStat("MP"));
        ChangeStat("MentalStat", 100);
        ChangeStat("FaithStat", 100);
        God = GodType.None;
    }


    public void ChangeStat(string statName, float value)
    {
        if (stats.ContainsKey(statName))
        {
            stats[statName] += value;
            Debug.Log($"{statName}��(��) {value} ��ŭ �����. ���� ��: {stats[statName]}");
        }
        else
        {
            Debug.LogWarning($"{statName} ������ �������� �ʽ��ϴ�!");
        }
    }

    public float GetStat(string statName)
    {
        return stats.ContainsKey(statName) ? stats[statName] : 0;
    }

    //  Ư�� ������ ������ ��� �������� �Լ�
    public Equipment GetEquippedItem(EquipmentType slot)
    {
        return equippedItem[(int)slot]; // �迭 �ε����� ���� ����
    }

    public bool EquipItem(Equipment equipItem)
    {

        int slotIndex = (int)equipItem.EquipmentType;

        RemoveEquip(equipItem);

        //��� ����
        equippedItem[slotIndex] = equipItem;
        Debug.Log($"{equipItem.ItemName}�� {equipItem.EquipmentType}ĭ�� �����߽��ϴ�.");

        ChangeStat("Atk", equipItem.AttackPoint);
        ChangeStat("Def", equipItem.DefensePoint);

        equipUI?.UpdateEquipmentUI();

        return true;
    }

    public void RemoveEquip(Equipment equipitem)
    {
        int slotIndex = ( int)equipitem.EquipmentType;
        // ��� ����
        if (equippedItem[slotIndex] != null)
        {
            Debug.Log($"���� ��� {equippedItem[slotIndex].ItemName}�� �����մϴ�.");

            ChangeStat("Atk", -equippedItem[slotIndex].AttackPoint);
            ChangeStat("Def", -equippedItem[slotIndex].DefensePoint);

            equippedItem[slotIndex] = null;
        }

        equipUI?.UpdateEquipmentUI();
    }

    public void ApplyItemEffects(Equipment equipItem)
    {
        foreach (var effect in equipItem.Effects)
        {
            effect.ApplyEffect(this); // �÷��̾�� ȿ�� ����
        }
    }

    /*public void RemoveItemEffects(Equipment equipItem)
    {
        foreach (var effect in equipItem.Effects)
        {
            effect.RemoveEffect(this); // �÷��̾�� ����� ȿ�� ����
        }
    }*/


}
