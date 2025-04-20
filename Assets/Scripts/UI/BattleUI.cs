using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using PlayerScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("BattleObject")]
    [SerializeField] private GameObject battleWindow;
    [SerializeField] private TMP_Text battleLogs;
    [SerializeField] private GameObject battleBtns;
    [SerializeField] private GameObject skillPage;
   
    [Header("Player")]
    [SerializeField] private TMP_Text playerHpText;
    [SerializeField] private Slider playerHp;
    //[SerializeField] private TMP_Text playerMpText;
    //[SerializeField] private Slider playerMp;
    
    [Header("Entity")]
    [SerializeField] private TMP_Text entityHpText;
    [SerializeField] private Slider entityHp;

    // 전투 UI 열기
    public void OpenBattleWindow() => battleWindow.SetActive(true);
    // 전투 UI 닫기
    public void HideBattleWindow() => battleWindow.SetActive(false);

    public void UpdateEntityUI(EntityObject enemy) // 적 HP UI 업데이트
    {
        entityHp.maxValue = enemy.enemyData.MaxHp;
        entityHp.value = enemy.GetStat("CurrentHP");
        entityHpText.text = $"{enemy.GetStat("CurrentHP")} / {enemy.enemyData.MaxHp}";
    }
    
    public void UpdatePlayerUI() // 플레이어 HP UI 업데이트
    {
        int cur = Player.Instance.GetStat("CurrentHP");
        int max = Player.Instance.GetStat("HP");
        playerHp.maxValue = max;
        playerHp.value = cur;
        playerHpText.text = $"{cur} / {max}";
    }
    
    public void Log(string message) // 로그 메시지 추가
    {
        battleLogs.text += $"\n{message}";
    }
    
    public void ClearLog() // 로그 초기화
    {
        battleLogs.text = "";
    }
}
