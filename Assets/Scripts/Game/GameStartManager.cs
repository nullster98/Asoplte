using PlayerScript;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class GameStartManager : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField] private GameObject menu;
        [SerializeField] private GameObject gameMenu;
        [SerializeField] private GameObject godSelect;
        [SerializeField] private GameObject characterSelect;
        [SerializeField] private GameObject speciesSelect;
        [SerializeField] private GameObject traitSelect;
        [SerializeField] private TMP_Text faithPoint;
        [SerializeField] private GodUI godUI;
        
        public void Start()
        {
            menu.SetActive(true);
            StartGame();

            if (Player.Instance == null)
            {
                Debug.LogWarning("Player.Instance가 없어서 새로운 플레이어 객체를 생성합니다.");
                GameObject playerObj = new GameObject("Player");
                playerObj.AddComponent<Player>();
                DontDestroyOnLoad(playerObj); // 씬 넘겨도 유지되게
            }
        }

        public void Update()
        {
            faithPoint.text = "신앙 포인트 : " + Player.Instance.GetFatihString();
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
            godSelect.SetActive(true);
            
            godUI.InitializeUI();
        }

        private void StartGame()
        {
            gameMenu.SetActive(false);
            godSelect.SetActive(false);
            characterSelect.SetActive(false);
            speciesSelect.SetActive(false);
            traitSelect.SetActive(false);
        }

        public void ChangeSpeciesOn()
        {
            speciesSelect.SetActive(true);
        }
    }
}
