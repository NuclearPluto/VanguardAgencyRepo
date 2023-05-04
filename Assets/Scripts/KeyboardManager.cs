using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardManager : MonoBehaviour
{
    [SerializeField] public UnityEngine.Canvas pauseOverlay;
    [SerializeField] public UnityEngine.Canvas escapeOverlay;

    private UIManager uimanager;
    private InputAction pausePress;
    private InputAction escapePress;
    private bool isPaused = false;
    private bool isEscaped = false;
    // Start is called before the first frame update
    void Awake() {
        var inputActions = Resources.Load<InputActionAsset>("Keyboard Controls");
        pausePress = inputActions.FindAction("Pause");
        escapePress = inputActions.FindAction("Escape");
    }
    void Start() {
    }
    private void OnEnable()
    {
        //space bar
        pausePress.Enable();
        pausePress.performed += OnTogglePause;
        pausePress.performed += OnEnablePauseOverlay;

        escapePress.Enable();
        escapePress.performed += OnTogglePause;
        escapePress.performed += OnToggleEscape;
    }

    private void OnDisable()
    {
        pausePress.performed -= OnTogglePause;
        pausePress.Disable();

        escapePress.performed -= OnToggleEscape;
        escapePress.performed -= OnTogglePause;
        escapePress.Disable();
    }

    public void OnTogglePause(InputAction.CallbackContext context) {
        isPaused = !isPaused;
        //pauseOverlay.gameObject.SetActive(isPaused);
        if (isPaused)
            Time.timeScale = 0;
            else Time.timeScale = 1;
    }

    private void OnEnablePauseOverlay(InputAction.CallbackContext context) {
        pauseOverlay.gameObject.SetActive(isPaused);
    }

    public void OnToggleEscape(InputAction.CallbackContext context) {
        isEscaped = !isEscaped;

        if (isEscaped) {
            escapeOverlay.gameObject.SetActive(isEscaped);
        }
        else {
            escapeOverlay.gameObject.GetComponent<EscapeManager>().HideAndDisable();
        }
        
    }
}
