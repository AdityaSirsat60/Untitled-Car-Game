using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex == 1)
        {
            Menu();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
