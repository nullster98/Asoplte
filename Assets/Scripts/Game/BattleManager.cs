using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    private EnemyData enemy;

    [SerializeField] GameObject BattleWindow;
    [SerializeField] Slider MonsterHP;
    [SerializeField] TMP_Text MonsterHP_Text;
    [SerializeField] TMP_Text PlayerHP_Text;
    [SerializeField] Slider PlayerHP;
    [SerializeField] Image MonsterSprite;
    [SerializeField] TMP_Text BattleLogs;

    [SerializeField] GameObject BattleBtns;
    [SerializeField] GameObject SkillPage;

    private void Awake()
    {
        if(DatabaseManager.Instance.enemyDatabase == null)
        {
            Debug.LogError("EnemyDatabse missing by.BattleManager");
            return;
        }

        EnemyCreator.InitializeEnemies(EventManager.Instance.floor);
        BattleWindow.SetActive(false);
    }

    public void StartBattle(EventChoice selectedChoice)
    { 
        BattleWindow.SetActive(true);      

        int? enemyId = selectedChoice.FixedID;

        if(enemyId == 0 )
        {
            enemy = EnemyCreator.StartBattle(EventManager.Instance.floor);
        }

        else
            enemy = DatabaseManager.Instance.enemyDatabase.GetEnemyByID(enemyId);

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
        PlayerHP.value = Player.Instance.GetStat("CurrentHP");

        MonsterHP_Text.text = $"{enemy.CurrentHP} / {enemy.MaxHP}";
        PlayerHP_Text.text = $"{(float)Player.Instance.GetStat("CurrentHP")} / {(float)Player.Instance.GetStat("HP")}";
    }

    IEnumerator EnemyCounterAttack()
    {
        yield return new WaitForSeconds(0.5f);

        float enemyDamage = enemy.Attack;

        // 플레이어 체력 감소
        Player.Instance.ChangeStat("CurrentHP", -enemyDamage);
        BattleLogs.text += $"\n{enemy.Name}은 {enemyDamage}의 데미지를 주었다!";

        UpdateBattleUI();

        if (Player.Instance.GetStat("CurrentHP") <= 0)
        {
            BattleLogs.text += "\n플레이어가 패배하였습니다!";
            EndBattle();
        }
    }

    public void EndBattle()
    {
        if(enemy.CurrentHP <= 0)
        BattleWindow.SetActive(false);
    }

    public void Damage()
    {
        float playerAtk = Player.Instance.GetStat("Atk");
        float playerHP = Player.Instance.GetStat("CurrentHP");

        // 플레이어가 적을 공격
        enemy.CurrentHP -= playerAtk;
        BattleLogs.text += $"\n{Player.Instance.name}은 {playerAtk}만큼의 데미지를 {enemy.Name}에게 주었다!";

        UpdateBattleUI();

        // 적이 살아있다면 0.5초 후 반격
        if (enemy.CurrentHP > 0)
        {
            StartCoroutine(EnemyCounterAttack());
        }
        else
        {
            EndBattle();
        }
    }

    public void AtkBtn()
    {
        Damage();
        UpdateBattleUI();
    }

    public void SkillBtn()
    {
        SkillPage.SetActive(true);
        BattleBtns.SetActive(false);
    }

    public void BacktoBattleBtns()
    {
        SkillPage.SetActive(false);
        BattleBtns.SetActive(true);
    }
    
    public void ItemBtn()
    {

    }   
    
    public void RunBtn()
    {

    }

    private void Update()
    {
        
    }

}
