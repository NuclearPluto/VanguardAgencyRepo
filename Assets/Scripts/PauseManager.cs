using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] public UnityEngine.UI.Image pauseOverlay;
    private InputAction pausePress;
    private bool isPaused = false;
    // Start is called before the first frame update
    void Awake() {
        var inputActions = Resources.Load<InputActionAsset>("Keyboard Controls");
        pausePress = inputActions.FindAction("Pause");
    }
    private void OnEnable()
    {
        //space bar
        pausePress.Enable();
        pausePress.performed += OnTogglePause;
    }

    private void OnDisable()
    {
        pausePress.performed -= OnTogglePause;
        pausePress.Disable();
    }

    private void OnTogglePause(InputAction.CallbackContext context) {
        isPaused = !isPaused;
        pauseOverlay.gameObject.SetActive(isPaused);
        if (isPaused)
            Time.timeScale = 0;
            else Time.timeScale = 1;
    }
}
