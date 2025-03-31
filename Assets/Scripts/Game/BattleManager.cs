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
        private Enemy currentEnemy;

        [SerializeField] private BattleUI ui;

        public void StartBattle(GameObject enemyObj)
        {
            if (enemyObj == null)
            {
                Debug.LogError("전투 시작 실패: enemyObj가 null입니다.");
                return;
            }
            
            currentEnemy = enemyObj.GetComponent<Enemy>();
            if (currentEnemy == null)
            {
                Debug.LogError("Enemy 컴포넌트를 찾을 수 없습니다!");
                return;
            }

            ui.OpenBattleWindow();
            ui.UpdateEntityUI(currentEnemy);
            ui.UpdatePlayerUI();

            ui.Log($"{currentEnemy.enemyData.Name} 출현!");

            Destroy(enemyObj);
        }

        public void PlayerAttack()
        {
            int dmg = Player.Instance.GetStat("Atk");
            currentEnemy.TakeDamage(dmg);

            ui.Log($"플레이어가 {dmg}의 데미지를 줌");
            ui.UpdateEntityUI(currentEnemy);

            if (currentEnemy.GetStat("CurrentHp") <= 0)
            {
                ui.Log($"{currentEnemy.enemyData.Name}을 처치했습니다!");
                EndBattle();
            }
            else
            {
                //StartCoroutine(EnemyCounterAttack());
            }
        }

        public void EndBattle()
        {
            ui.HideBattleWindow();
            EventManager.Instance.currentSpawnedEnemy = null;
            EventManager.Instance.eventHandler.HandleEventEnd();

        }
        
    }
}
