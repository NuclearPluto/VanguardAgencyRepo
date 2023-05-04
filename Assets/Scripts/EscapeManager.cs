using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;


public class EscapeManager : UIManager
{
    KeyboardManager keyboardManager;
    void Start() {
        GameObject directorObject = GameObject.FindGameObjectWithTag("Director");
        keyboardManager = directorObject.GetComponent<KeyboardManager>();
    }

    public void ResumeButton()
    {
        var context = new InputAction.CallbackContext();

        Debug.Log("click detected.");
        keyboardManager.OnToggleEscape(context);
        keyboardManager.OnTogglePause(context);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
