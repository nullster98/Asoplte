using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class StartManager : MonoBehaviour
    {
        public void Load() //시작씬 -> 메인씬
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
