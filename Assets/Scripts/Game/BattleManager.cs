using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    private EnemyData enemy;

    [SerializeField] EnemyDatabase enemyDatabase;
    [SerializeField] GameObject BattleWindow;
    [SerializeField] Slider MonsterHP;
    [SerializeField] TMP_Text MonsterHP_Text;
    [SerializeField] TMP_Text PlayerHP_Text;
    [SerializeField] Slider PlayerHP;
    [SerializeField] Image MonsterSprite;
    [SerializeField] TMP_Text BattleLogs;

    private void Awake()
    {
        if(enemyDatabase == null)
        {
            Debug.LogError("EnemyDatabse missing by.BattleManager");
            return;
        }

        EnemyCreator.InitializeEnemies(enemyDatabase);
        BattleWindow.SetActive(false);
    }

    public void StartBattle(EventChoice selectedChoice)
    { 
        BattleWindow.SetActive(true);      

        int enemyId = selectedChoice.FixedID;

        if(enemyId == 0 )
        {
            enemy = enemyDatabase.GetRandomEnemy();
        }

        else
            enemy = enemyDatabase.GetEnemyByID(enemyId);

        if(enemy == null )
        {
            Debug.LogError($"적 정보를 찾을수 없습니다");
            return;
        }

        InitializeBattleUI();
        BattleLogs.text = $"{enemy.Name} 출현!";
    }

    public void InitializeBattleUI()
    {
        MonsterSprite.sprite = enemy.EnemySprite;
        MonsterHP.maxValue = enemy.MaxHP;
        MonsterHP.value = enemy.CurrentHP;
        MonsterHP_Text.text = $"{enemy.CurrentHP} / {enemy.MaxHP}";

        PlayerHP.maxValue = Player.Instance.GetStat("HP");
        PlayerHP.value = Player.Instance.GetStat("HP");
        PlayerHP_Text.text = $"{(float)Player.Instance.GetStat("CurrentHP")} / {(float)Player.Instance.GetStat("HP")}";
    }

    public void UpdateBattleUI()
    {
        MonsterHP.value = enemy.CurrentHP;
        PlayerHP.value = Player.Instance.GetStat("HP");
    }

    IEnumerator WaitForHalfSecond()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void EndBattle()
    {
        BattleWindow.SetActive(false);
    }

    public void Damage()
    {
        float playerHP = Player.Instance.GetStat("CurrnetHP");
        float playerAtk = Player.Instance.GetStat("Atk");
        enemy.CurrentHP -= playerHP;
        WaitForHalfSecond();
        playerHP -= enemy.Attack;

    }

    public void AtkBtn()
    {
        Damage();
        UpdateBattleUI();
    }

    public void SkillBtn()
    {

    }
    
    public void ItemBtn()
    {

    }   
    
    public void RunBtn()
    {

    }

}
