using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private float movementSpeed = 5f;
    private float doorOpenSpeed = 0.5f; //time in seconds to open
    private Dijekstras pathfinding;

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
        List<Room> path = pathfinding.GetShortestPath(transform.position, targetPosition);
        foreach (Room room in path) {
            Debug.Log("The room pivot is " + room.getUnitPivot());
        }
        StartCoroutine(MoveToPositionCoroutine(path, targetPosition));
    }

    private IEnumerator MoveToPositionCoroutine(List<Room> path, Vector2 goToPosition)
    {
        if (path.Count == 0) {}
        else if (path.Count == 1) {
            Vector2 targetPosition = goToPosition;
            while (Vector2.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else {
            Vector2 targetPosition = new Vector2(0, 0);
            Door doorToOpen = null;
            if (path[0].getConnectedDoor(path[1]).Count == 1) {
                doorToOpen = path[0].getConnectedDoor(path[1])[0];
                //Debug.Log("The door to teleport to is a " + doorToOpen.getOrientation());
                targetPosition = path[1].getConnectedDoor(path[0])[0].getPosition();
                //Debug.Log("The door to go to is a " + path[0].getConnectedDoor(path[1])[0].getOrientation());
                //Debug.Log("The connected door position from " + targetPosition + " needs to go to " + doorToOpen.getPosition());
            } 
            else if (path[0].getConnectedDoor(path[1]).Count == 2) {
                List<Door> connectedDoors = path[0].getConnectedDoor(path[1]);
                float distance0 = connectedDoors[0].getDistanceFrom(transform.position);
                float distance1 = connectedDoors[1].getDistanceFrom(transform.position);
                if (distance0 < distance1) {
                    doorToOpen = path[1].getConnectedDoor(path[0])[0];
                    targetPosition = connectedDoors[0].getPosition();
                }
                else {
                    doorToOpen = path[1].getConnectedDoor(path[0])[1];
                    targetPosition = connectedDoors[1].getPosition();
                }
            }
            while (Vector2.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
                yield return null;
            }
            // Snap to the target position when close enough
            transform.position = targetPosition;

            yield return StartCoroutine(PassToDoorCoroutine(doorToOpen));
            path.RemoveAt(0);
            yield return StartCoroutine(MoveToPositionCoroutine(path, goToPosition));
        }
    }

    private IEnumerator PassToDoorCoroutine(Door door) {
        yield return new WaitForSeconds(doorOpenSpeed);


        transform.position = door.getPosition();
        Debug.Log("passed through door and went to " + door.getPosition());
    }
}
