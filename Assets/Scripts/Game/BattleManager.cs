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
        private EntityObject currentEnemy; // 현재 전투 중인 적 오브젝트

        [SerializeField] private BattleUI ui;

        public void StartBattle(GameObject entityObj) // 전투 시작 처리
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

        public void PlayerAttack() // 플레이어가 공격을 선택했을 때
        {
            int playerDmg = Player.Instance.GetStat("Atk");
            currentEnemy.TakeDamage(playerDmg);
            StartCoroutine(HandleEnemyHitThenContinue(playerDmg));
        }

        
        // 적이 피격되고 상태 갱신 후 반격 여부 판단
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

        public void EndBattle() // 전투 종료 처리
        {
            ui.ClearLog(); //로그 지우고
            ui.HideBattleWindow(); // UI 닫고
            Destroy(EventManager.Instance.currentSpawnedEnemy); // 오브젝트 제거하고
            EventManager.Instance.currentSpawnedEnemy = null; //초기화 시켜주고
            EventManager.Instance.NotifyUIClosed(); // 닫힘 알림 넣어주고
            EventManager.Instance.RequestHandleEventEnd(); // 다음 이벤트로
        }

        private IEnumerator EnemyCounterAttack() // 적의 반격 처리
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
