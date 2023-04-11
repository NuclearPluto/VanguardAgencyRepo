using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Transform selectionAreaTransform;
    private Vector3 startPosition;
    private List<GameObject> selectedUnits;

    private void Awake() {
        selectedUnits = new List<GameObject>();
        selectionAreaTransform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.2f, 1.0f, 0.4f, 0.5f);
        selectionAreaTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionAreaTransform.gameObject.SetActive(true);
            startPosition = GetMouseWorldPosition();

            //Debug.Log("Number of rooms is " + gameObject.GetComponent<StageGeneration>().getListRooms().Count);
            foreach (Room room in gameObject.GetComponent<StageGeneration>().getListRooms()) {
                //Debug.Log("Current Room Pivot is " + room.getPivot());
                if (room.isPointInRoom(startPosition)){ 
                    Debug.Log("Point clicked is currently in a room");
                    Debug.Log("Room ID is " + room.getRoomID());
                }
            }
        }

        if (Input.GetMouseButton(0)) {
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

        if (Input.GetMouseButtonUp(0))
        {
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

        if (Input.GetMouseButtonDown(1)) {
            Debug.Log($"World position to move to is {GetMouseWorldPosition()}");
            foreach(GameObject selectedUnit in selectedUnits) {
                selectedUnit.GetComponent<PlayerBehavior>().MoveToPosition(GetMouseWorldPosition());
            }
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
