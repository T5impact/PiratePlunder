using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;

    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Start()
    {
        Screen.SetResolution(2160 / 3, 2560 / 3, false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMainPanel();
        }
    }
}
