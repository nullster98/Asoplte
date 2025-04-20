using System.Collections;
using Entities;
using Event;
using PlayerScript;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public class BattleManager : MonoBehaviour
    {
        private EntityObject currentEnemy;

        [SerializeField] private BattleUI ui;

        public void StartBattle(GameObject entityObj)
        {
            EventManager.Instance.NotifyUIOpened();
            Debug.Log($"[StartBattle] StartBattle 호출됨 / enemyObj: {entityObj}");

            if (entityObj == null)
            {
                Debug.LogError("전투 시작 실패: enemyObj가 null입니다.");
                return;
            }

            currentEnemy = entityObj.GetComponent<EntityObject>();
            if (currentEnemy == null)
            {
                Debug.LogError("Enemy 컴포넌트를 찾을 수 없습니다!");
                return;
            }

            ui.OpenBattleWindow();
            ui.UpdateEntityUI(currentEnemy);
            ui.UpdatePlayerUI();

            ui.Log($"{currentEnemy.enemyData.Name} 출현!");

            //Destroy(entityObj);
        }

        public void PlayerAttack()
        {
            int playerDmg = Player.Instance.GetStat("Atk");
            currentEnemy.TakeDamage(playerDmg);
            StartCoroutine(HandleEnemyHitThenContinue(playerDmg));
        }

        private IEnumerator HandleEnemyHitThenContinue(int damage)
        {
            yield return StartCoroutine(currentEnemy.FlashOnHit(Color.red, 0.1f));

            ui.Log($"플레이어가 {currentEnemy.enemyData.Name}에게 {damage}의 데미지를 입혔다!");
            ui.UpdateEntityUI(currentEnemy);

            if (currentEnemy.GetStat("CurrentHP") <= 0)
            {
                ui.Log($"️{currentEnemy.enemyData.Name}이(가) 쓰러졌습니다!");
                EndBattle();
                yield break;
            }

            StartCoroutine(EnemyCounterAttack());
        }

        public void EndBattle()
        {
            ui.ClearLog();
            ui.HideBattleWindow();
            Destroy(EventManager.Instance.currentSpawnedEnemy);
            EventManager.Instance.currentSpawnedEnemy = null;
            EventManager.Instance.NotifyUIClosed();
            EventManager.Instance.RequestHandleEventEnd();
        }

        private IEnumerator EnemyCounterAttack()
        {
            yield return new WaitForSeconds(0.5f);

            int enemyDmg = currentEnemy.GetStat("Atk");
            Player.Instance.TakeDamage(enemyDmg);
            ui.Log($"{currentEnemy.enemyData.Name}이(가) 플레이어에게 {enemyDmg}의 반격을 가했다!");
            ui.UpdatePlayerUI();

            if (Player.Instance.CurrentHP <= 0)
            {
                ui.Log("💀 플레이어가 쓰러졌습니다...");
                EndBattle();
            }
        }
    }

}
