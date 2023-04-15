using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Transform selectionAreaTransform;
    private Vector3 startPosition;
    private List<GameObject> selectedUnits;
    private InputAction startSelection;
    private InputAction updateSelection;
    private InputAction endSelection;
    private InputAction moveToPosition;
    private bool isUpdatingSelection = false;

    private void Awake()
    {
        selectedUnits = new List<GameObject>();
        selectionAreaTransform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.2f, 1.0f, 0.4f, 0.5f);
        selectionAreaTransform.gameObject.SetActive(false);

        var inputActions = Resources.Load<InputActionAsset>("Mouse Controls");
        startSelection = inputActions.FindAction("StartSelection");
        //Debug.Log(startSelection);
        updateSelection = inputActions.FindAction("UpdateSelection");
        //Debug.Log(updateSelection);
        endSelection = inputActions.FindAction("EndSelection");
        //Debug.Log(endSelection);
        moveToPosition = inputActions.FindAction("MoveToPosition");
        //Debug.Log(moveToPosition);
    }

    private void OnEnable()
    {
        startSelection.Enable();
        startSelection.performed += OnStartSelection;
        startSelection.canceled += OnEndSelection;

        updateSelection.Enable();
        updateSelection.performed += OnUpdateSelection;

        endSelection.Enable();

        moveToPosition.Enable();
        moveToPosition.performed += OnMoveToPosition;
    }

    private void OnDisable()
    {
        startSelection.performed -= OnStartSelection;
        startSelection.canceled -= OnEndSelection;
        startSelection.Disable();

        updateSelection.performed -= OnUpdateSelection;
        updateSelection.Disable();

        endSelection.Disable();

        moveToPosition.performed -= OnMoveToPosition;
        moveToPosition.Disable();
    }

    private void Update() {
        if (isUpdatingSelection) {
            OnUpdateSelection();
        }
    }

    private void OnStartSelection(InputAction.CallbackContext context)
    {
        Debug.Log("START SELECTION BEGINS");
        isUpdatingSelection = true;
        selectionAreaTransform.gameObject.SetActive(true);
        startPosition = GetMouseWorldPosition();
        //Debug.Log("Number of rooms is " + gameObject.GetComponent<StageGeneration>().getListRooms().Count);
        foreach (Room room in gameObject.GetComponent<StageGeneration>().getListRooms()) {
            //Debug.Log("Current Room Pivot is " + room.getPivot());
            if (room.isPointInRoom(startPosition)){ 
                //Debug.Log("Point clicked is currently in a room");
                //Debug.Log("Room ID is " + room.getRoomID());
                foreach (Room connectedRoom in room.getConnectedRooms()) {
                connectedRoom.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                Debug.Log("Connected Room Pivot is " + connectedRoom.getPivot());
                }
            }
        }
    }

    private void OnUpdateSelection(InputAction.CallbackContext context = default)
    {
        Debug.Log("UPDATE SELECTION BEGINS");
        Vector3 currentMousePosition = GetMouseWorldPosition();
        Vector3 lowerLeft = new Vector3 (
            Mathf.Min(startPosition.x, currentMousePosition.x), 
            Mathf.Min(startPosition.y, currentMousePosition.y)
        );
        Vector3 upperRight = new Vector3 (
            Mathf.Max(startPosition.x, currentMousePosition.x), 
            Mathf.Max(startPosition.y, currentMousePosition.y)
        );
        selectionAreaTransform.position = lowerLeft;
        selectionAreaTransform.localScale = upperRight - lowerLeft;
    }

    private void OnCancelStartSelection(InputAction.CallbackContext context) {
        
    }

    private void OnEndSelection(InputAction.CallbackContext context)
    {
        Debug.Log("END SELECTION BEGINS");
        isUpdatingSelection = false;
        if (context.phase == InputActionPhase.Canceled) {
            selectionAreaTransform.gameObject.SetActive(false);
            List<GameObject> unitsToRemove = new List<GameObject>();

            foreach (GameObject selectedUnit in selectedUnits) {
                selectedUnit.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                unitsToRemove.Add(selectedUnit);
            }

            Collider2D[] colliders = Physics2D.OverlapAreaAll(startPosition, GetMouseWorldPosition());
            List<Collider2D> collider2DList = new List<Collider2D>(colliders);

            foreach (GameObject unitToRemove in unitsToRemove)
            {
                selectedUnits.Remove(unitToRemove);
            }

            foreach (Collider2D collider2D in colliders)
            {
                if (!collider2D.CompareTag("Player"))
                {
                    collider2DList.Remove(collider2D);
                }
                else {
                    selectedUnits.Add(collider2D.gameObject);
                    GameObject spriteObject = collider2D.gameObject.transform.GetChild(0).gameObject;
                    spriteObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.8f, 1.0f, 1.0f);
                }
            }
        }   
    }

    private void OnMoveToPosition(InputAction.CallbackContext context)
    {
        Debug.Log($"World position to move to is {GetMouseWorldPosition()}");
        foreach(GameObject selectedUnit in selectedUnits) {
            selectedUnit.GetComponent<PlayerBehavior>().MoveToPosition(GetMouseWorldPosition());
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found.");
            return Vector3.zero;
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f;
        return worldPosition;
    }

    public bool hasSelectedUnits() {
        if (selectedUnits.Count == 0) {
            return false;
        }
        else return true;
    }


}
