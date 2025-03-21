using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public void Load() //시작씬 -> 메인씬
    {
        SceneManager.LoadScene("MainScene");
    }
}
