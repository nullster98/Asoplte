using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject GodSelect;
    [SerializeField] private GameObject CharacterSelect;
    [SerializeField] private GameObject SpeciesSelect;
    [SerializeField] private GameObject TraitSelect;
    [SerializeField] private TMP_Text FaithPoint;

    public void Start()
    {
        menu.SetActive(true);
        StartGame();

        if (Player.Instance == null)
        {
            Debug.LogWarning("Player.Instance가 없어서 새로운 플레이어 객체를 생성합니다.");
            GameObject playerObj = new GameObject("Player");
            Player player = playerObj.AddComponent<Player>();
        }
    }

    public void Update()
    {
        FaithPoint.text = "신앙 포인트 : " + Player.Instance.GetFatihString();
    }

    /*public void Load() //시작씬 -> 메인씬
    {
        SceneManager.LoadScene("MainScene");
    }
    */


    public void GameStart()//메인 씬에서 게임 시작을 누를시
    {
        menu.SetActive(false);

        gameMenu.SetActive(true);
        GodSelect.SetActive(true);
    }

    public void StartGame()
    {
        gameMenu.SetActive(false);
        GodSelect.SetActive(false);
        CharacterSelect.SetActive(false);
        SpeciesSelect.SetActive(false);
        TraitSelect.SetActive(false);
    }

    public void ChangeSpeciesOn()
    {
        SpeciesSelect.SetActive(true);
    }
}
