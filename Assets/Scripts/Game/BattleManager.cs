using System.Collections;
using Entities;
using Event;
using PlayerScript;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public class BattleManager : MonoBehaviour
    {
        private EnemyData enemy;

        [SerializeField] GameObject battleWindow;
        [SerializeField] private Slider monsterHp;
        [SerializeField] private TMP_Text monsterHpText;
        [SerializeField] private TMP_Text playerHpText;
        [SerializeField] private Slider playerHp;
        [SerializeField] private Image monsterSprite;
        [SerializeField] private TMP_Text battleLogs;

        [SerializeField] private GameObject battleBtns;
        [SerializeField] private GameObject skillPage;

        private void Awake()
        {
            if(DatabaseManager.Instance.enemyDatabase == null)
            {
                Debug.LogError("EnemyDatabse missing by.BattleManager");
                return;
            }

            EnemyCreator.InitializeEnemies(EventManager.Instance.Floor);
            battleWindow.SetActive(false);
        }

        public void StartBattle(EventChoice selectedChoice)
        { 
            battleWindow.SetActive(true);      

            int? enemyId = selectedChoice.FixedID;

            enemy = enemyId == 0 ? EnemyCreator.StartBattle(EventManager.Instance.Floor) : DatabaseManager.Instance.enemyDatabase.GetEnemyByID(enemyId);

            if(enemy == null )
            {
                Debug.LogError($"적 정보를 찾을수 없습니다");
                return;
            }

            InitializeBattleUI();
            battleLogs.text = $"{enemy.Name} 출현!";
        }

        private void InitializeBattleUI()
        {
            monsterSprite.sprite = enemy.EnemySprite;
            monsterHp.maxValue = enemy.MaxHp;
            monsterHp.value = enemy.CurrentHp;
            monsterHpText.text = $"{enemy.CurrentHp} / {enemy.MaxHp}";

            playerHp.maxValue = Player.Instance.GetStat("HP");
            playerHp.value = Player.Instance.GetStat("HP");
            playerHpText.text = $"{(float)Player.Instance.GetStat("CurrentHP")} / {(float)Player.Instance.GetStat("HP")}";
        }

        private void UpdateBattleUI()
        {
            monsterHp.value = enemy.CurrentHp;
            playerHp.value = Player.Instance.GetStat("CurrentHP");

            monsterHpText.text = $"{enemy.CurrentHp} / {enemy.MaxHp}";
            playerHpText.text =
                $"{(float)Player.Instance.GetStat("CurrentHP")} / {(float)Player.Instance.GetStat("HP")}";
        }

        // ReSharper disable Unity.PerformanceAnalysis
        IEnumerator EnemyCounterAttack()
        {
            yield return new WaitForSeconds(0.5f);

            int enemyDamage = enemy.Attack;

            // 플레이어 체력 감소
            Player.Instance.ChangeStat("CurrentHP", -enemyDamage);
            battleLogs.text += $"\n{enemy.Name}은 {enemyDamage}의 데미지를 주었다!";

            UpdateBattleUI();

            if (Player.Instance.GetStat("CurrentHP") <= 0)
            {
                battleLogs.text += "\n플레이어가 패배하였습니다!";
                EndBattle();
            }
        }

        private void EndBattle()
        {
            if(enemy.CurrentHp <= 0)
                battleWindow.SetActive(false);
        }

        private void Damage()
        {
            int playerAtk = Player.Instance.GetStat("Atk");
           
            // 플레이어가 적을 공격
            enemy.TakeDamage(playerAtk);
            battleLogs.text += $"\n{Player.Instance.name}은 {playerAtk}만큼의 데미지를 {enemy.Name}에게 주었다!";

            UpdateBattleUI();

            // 적이 살아있다면 0.5초 후 반격
            if (enemy.CurrentHp > 0)
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
            skillPage.SetActive(true);
            battleBtns.SetActive(false);
        }

        public void BacktoBattleBtns()
        {
            skillPage.SetActive(false);
            battleBtns.SetActive(true);
        }
    
        public void ItemBtn()
        {

        }   
    
        public void RunBtn()
        {

        }
        

    }
}
