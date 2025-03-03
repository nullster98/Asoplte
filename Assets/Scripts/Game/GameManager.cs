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
            Debug.LogWarning("Player.Instance�� ��� ���ο� �÷��̾� ��ü�� �����մϴ�.");
            GameObject playerObj = new GameObject("Player");
            Player player = playerObj.AddComponent<Player>();
        }
    }

    public void Update()
    {
        FaithPoint.text = "�ž� ����Ʈ : " + Player.Instance.GetFatihString();
    }

    /*public void Load() //���۾� -> ���ξ�
    {
        SceneManager.LoadScene("MainScene");
    }
    */


    public void GameStart()//���� ������ ���� ������ ������
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
