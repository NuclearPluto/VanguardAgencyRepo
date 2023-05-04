using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private bool isOptions = false;

    [SerializeField] public UnityEngine.Canvas optionsOverlay;

    public void StartGame() {
        SceneManager.LoadScene("FieldScene");
    }

    public void Options() {
        isOptions = !isOptions;
        if (isOptions) {
            optionsOverlay.gameObject.SetActive(true);
        }
        else {
            optionsOverlay.gameObject.GetComponent<OptionsManager>().HideAndDisable();
        }
    }

    public void Quit() {
        Application.Quit();
    }
}
