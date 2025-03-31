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
    [SerializeField] private Image entitySprite;

    public void OpenBattleWindow() => battleWindow.SetActive(true);
    public void HideBattleWindow() => battleWindow.SetActive(false);

    public void UpdateEntityUI(Enemy enemy)
    {
        entitySprite.sprite = enemy.enemyData.EnemySprite;
        entityHp.maxValue = enemy.enemyData.MaxHp;
        entityHp.value = enemy.GetStat("CurrentHP");
        entityHpText.text = $"{enemy.GetStat("CurrentHP")} / {enemy.enemyData.MaxHp}";
    }
    
    public void UpdatePlayerUI()
    {
        int cur = Player.Instance.GetStat("CurrentHP");
        int max = Player.Instance.GetStat("HP");
        playerHp.maxValue = max;
        playerHp.value = cur;
        playerHpText.text = $"{cur} / {max}";
    }
    
    public void Log(string message)
    {
        battleLogs.text += $"\n{message}";
    }
}
