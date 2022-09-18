using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    public GameObject gameManager;
    public Button continueButton;
    public Button restartButton;
    public Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        continueButton.enabled = false;
        restartButton.enabled = false;
        quitButton.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetComponent<GameManager>().progress.Count ==1)
        {
            continueButton.enabled = true;
        }
        else
        {
            continueButton.enabled = false;
            restartButton.enabled = true;
            quitButton.enabled = true;
        }
    }
}
