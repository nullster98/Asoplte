using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public void Load() //½ÃÀÛ¾À -> ¸ŞÀÎ¾À
    {
        SceneManager.LoadScene("MainScene");
    }
}
