using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoControl : MonoBehaviour
{
    bool timeStopped = false;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene("14manScene");
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.LoadScene("1ManTestScene");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!timeStopped)
            {
                timeStopped = true;
                Time.timeScale = 0f;
            }
            else
            {
                timeStopped = false;
                Time.timeScale = 1f;
            }
            
        }
    }
}
