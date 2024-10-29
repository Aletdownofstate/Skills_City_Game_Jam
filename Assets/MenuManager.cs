using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button playButton, optionsButton, quitButton;

    void Start()
    {
        Application.targetFrameRate = 60;
        playButton.GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene(1); });
        quitButton.GetComponent<Button>().onClick.AddListener(delegate { Application.Quit(); });
    }
}
