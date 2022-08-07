using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    // Start is called before the first frame update
    // Change Scene to Main
    private void Awake()
    {
        SceneManager.LoadScene(0);
    }
  
}
