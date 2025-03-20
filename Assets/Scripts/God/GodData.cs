using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GodEffect
{
    public abstract void ApplyEffect(Player player);
}

public class GodData : MonoBehaviour
{
    public string GodName;
    public string FileName;
    public float GodID;
    public bool IsUnlocked = false;
    public int UnlockCost;
    public Sprite GetBackgroundImage()
    {
        return LoadBackgroundSprite(FileName);
    }

    private Sprite LoadBackgroundSprite(string imageName)
    {
        Sprite backgroundSprite = Resources.Load<Sprite>($"God/Backgrounds/{imageName}_BG");
        return backgroundSprite != null ? backgroundSprite : Resources.Load<Sprite>("God/Backgrounds/default_BG");
    }
    public Sprite GetGodImage()
    {
        return LoadGodSprite(FileName);
    }

    private Sprite LoadGodSprite(string imageName)
    {
        Sprite godSprite = Resources.Load<Sprite>($"Images/Gods/{imageName}");
        return godSprite != null ? godSprite : Resources.Load<Sprite>("Images/default");
    }

    public string GetDescription()
    {
        TextAsset description = Resources.Load<TextAsset>($"Descriptions/{FileName}");
        return description != null ? description.text : "설명 없음";
    }

    public Dictionary<string, float> GodStats = new Dictionary<string, float>
    {
        {"Atk", 0},
        {"Def", 0},
        {"HP", 0 },
        {"MP", 0 },
        {"MentalStat", 0 }
    };

    public GodEffect SpecialEffect;
}

public class LibertyGodEffect :GodEffect
{
    public override void ApplyEffect(Player player)
    {
        Player.Instance.MaxCost += 10;
    }
}
