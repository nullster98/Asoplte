using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Event;
using Game;
using PlayerScript;
using TMPro;
using Trait;
using UI;
using UnityEngine;
using UnityEngine.UI;

public enum EffectChangeType // 효과 타입 분류: 특성 추가/제거, 스탯 증감
{
   GainTrait,
   RemoveTrait,
   StatIncrease,
   StatDecrease
}

public class EffectResultUI : MonoBehaviour
{
   [SerializeField] private GameObject eventResultContainer;
   [SerializeField] private Image resultImage;
   [SerializeField] private TMP_Text resultText;

   private Action onConfirmAction;
   private EffectChangeType changeType;
   private object currentResult;

   // 외부에서 호출 → 결과 UI 구성
    public void SetupResultUI(EffectChangeType changeType, string id, int amount = 0)
    {
        EventManager.Instance.NotifyUIOpened();

        // 기본 UI
        this.changeType = changeType;
        resultText.text = "";
        onConfirmAction = null;

        switch (changeType)
        {
            case EffectChangeType.GainTrait:
                if (id.StartsWith("?{")) // 무작위 추가
                {
                    var match = Regex.Match(id, @"\?\{(\w+)\}");
                    string category = match.Success ? match.Groups[1].Value : "Any";

                    var pool = DatabaseManager.Instance.traitList
                        .Where(t => !Player.Instance.HasTrait(t.traitID))
                        .ToList();

                    if (category == "Positive")
                        pool = pool.Where(t => t.PnN == TraitPnN.Positive).ToList();
                    else if (category == "Negative")
                        pool = pool.Where(t => t.PnN == TraitPnN.Negative).ToList();

                    var selected = pool.GetRandom();
                    if (selected != null)
                    {
                        resultImage.sprite = selected.traitImage;
                        resultText.text = $"<b>특성 획득 - {selected.traitName}</b>\n{selected.summary}";
                        onConfirmAction = () => Player.Instance.AddTrait(selected.traitID);
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning("[EffectResultUI] 조건에 맞는 특성이 없어 무작위 선택 실패");
                    }
                }
                else // 고정 특성
                {
                    var trait = DatabaseManager.Instance.GetTraitData(id);
                    if (trait != null)
                    {
                        resultImage.sprite = trait.traitImage;
                        resultText.text = $"<b>특성 획득 - {trait.traitName}</b>\n{trait.summary}";
                        onConfirmAction = () => Player.Instance.AddTrait(trait.traitID);
                    }
                }
                break;

            case EffectChangeType.RemoveTrait:
                if (id.StartsWith("?{")) // 무작위 제거
                {
                    var match = Regex.Match(id, @"\?\{(\w+)\}");
                    string category = match.Success ? match.Groups[1].Value : "Any";

                    var owned = Player.Instance.selectedTraitIDs.ToList();

                    if (category == "Positive")
                        owned = owned.Where(tid => DatabaseManager.Instance.GetTraitData(tid)?.PnN == TraitPnN.Positive).ToList();
                    else if (category == "Negative")
                        owned = owned.Where(tid => DatabaseManager.Instance.GetTraitData(tid)?.PnN == TraitPnN.Negative).ToList();

                    var toRemove = owned.GetRandom();
                    if (!string.IsNullOrEmpty(toRemove))
                    {
                        var trait = DatabaseManager.Instance.GetTraitData(toRemove);
                        resultImage.sprite = trait.traitImage;
                        resultText.text = $"<b>특성 제거 - {trait.traitName}</b>\n{trait.summary}";
                        onConfirmAction = () => Player.Instance.RemoveTrait(toRemove);
                    }
                }
                else // 고정 제거
                {
                    var trait = DatabaseManager.Instance.GetTraitData(id);
                    if (trait != null)
                    {
                        resultImage.sprite = trait.traitImage;
                        resultText.text = $"<b>특성 제거 - {trait.traitName}</b>\n{trait.summary}";
                        onConfirmAction = () => Player.Instance.RemoveTrait(id);
                    }
                }
                break;

            case EffectChangeType.StatIncrease:
                SetupStatUI(id, amount, isIncrease: true);
                onConfirmAction = () => Player.Instance.ChangeStat(id, amount);
                break;

            case EffectChangeType.StatDecrease:
                SetupStatUI(id, amount, isIncrease: false);
                onConfirmAction = () => Player.Instance.ChangeStat(id, amount);
                break;
        }

        eventResultContainer.SetActive(true);
    }

    private void SetupStatUI(string stat, int amount, bool isIncrease) // 스탯 변화 UI 설정
    {
        string suffix = isIncrease ? "_Up" : "_Down";
        string spritePath = $"StatUI/Stats/Stat{suffix}";

        resultImage.sprite = Resources.Load<Sprite>(spritePath);

        string sign = isIncrease ? "+" : "-";
        resultText.text = $"<b>스탯 {(isIncrease ? "증가" : "감소")} - {stat}</b>\n{sign}{Mathf.Abs(amount)}";
    }

    public void CheckBtn() // 확인 버튼 클릭 → 효과 실제 적용
    {
        onConfirmAction?.Invoke(); // 실제 적용
        UIManager.Instance.UpdateHpUI();
        UIManager.Instance?.UpdatePlayerUI();
        EventManager.Instance.NotifyUIClosed();
        EventManager.Instance.RequestHandleEventEnd();
        eventResultContainer.SetActive(false);
    }
   
}
