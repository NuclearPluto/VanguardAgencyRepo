using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 3f;
    Dijekstras pathfinding;

    void Start()
    {
        GameObject directorObject = GameObject.FindGameObjectWithTag("Director");

        if (directorObject != null)
        {
            Debug.Log(directorObject.GetComponent<StageGeneration>().getListRooms().Count);
            pathfinding = directorObject.GetComponent<StageGeneration>().getPathfinding();
        }
        else
        {
            Debug.LogError("Game object with tag 'Director' not found.");
        }
    }

    public void MoveToPosition(Vector2 targetPosition)
    {
        StopAllCoroutines(); 
        List<Room> testing = pathfinding.GetShortestPath(transform.position, targetPosition);
        foreach (Room room in testing) {
            Debug.Log("The room pivot is " + room.getUnitPivot());
        }
        StartCoroutine(MoveToPositionCoroutine(targetPosition));
    }

    private IEnumerator MoveToPositionCoroutine(Vector2 targetPosition)
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }

        // Snap to the target position when close enough
        transform.position = targetPosition;
    }
}
