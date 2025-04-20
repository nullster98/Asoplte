using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EndingType
{
    Death,
    BadEnding,
    NormalEnding,
    GoodEnding,
}

public class EndingUI : MonoBehaviour
{
    [SerializeField] private GameObject endingScene;
    [SerializeField] private Image endingImage;
    [SerializeField] private Sprite deathImage;
    [SerializeField] private Sprite badEndingImage;
    [SerializeField] private Sprite normalEndingImage;
    [SerializeField] private Sprite goodEndingImage;
    [SerializeField] private TMP_Text endingText;

    private void Start()
    {
        endingScene.SetActive(false);
    }

    public void ShowEnding(EndingType type)
    {
        endingScene.SetActive(true);

        switch (type)
        {
            case EndingType.Death:
                endingImage.sprite = deathImage;
                endingText.text = "사망 엔딩";
                break;
            case EndingType.BadEnding:
                endingImage.sprite = badEndingImage;
                endingText.text = "베드 엔딩";
                break;
            case EndingType.NormalEnding:
                endingImage.sprite = normalEndingImage;
                endingText.text = "일반 엔딩";
                break;
            case EndingType.GoodEnding:
                endingImage.sprite = goodEndingImage;
                endingText.text = "굿 엔딩";
                break;
        }
    }

    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
