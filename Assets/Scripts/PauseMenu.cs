using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeButton, optionsButton, quitButton;

    void Start()
    {        
        resumeButton.GetComponent<Button>().onClick.AddListener(delegate { Time.timeScale = 1; gameObject.SetActive(false); });
        quitButton.GetComponent<Button>().onClick.AddListener(delegate { Application.Quit(); });
    }
}
