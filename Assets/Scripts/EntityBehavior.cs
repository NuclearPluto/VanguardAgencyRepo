using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehavior : MonoBehaviour
{
    [SerializeField] protected float health = 10f;
    [SerializeField] protected float movementSpeed = 5f;
    [SerializeField] protected float doorOpenSpeed = 0.5f; //time in seconds to open
    protected Dijekstras pathfinding;
    protected Room currentRoom;

    protected virtual void Start()
    {
        GameObject directorObject = GameObject.FindGameObjectWithTag("Director");

        if (directorObject != null)
        {
            pathfinding = directorObject.GetComponent<StageGeneration>().getPathfinding();
            currentRoom = pathfinding.GetCurrentRoom(transform.position);
            currentRoom.addEntity(this);
            GameEvents.current.EntityAdded(this);
        }
        else
        {
            Debug.LogError("Game object with tag 'Director' not found.");
        }
    }

    public virtual void MoveToPosition(Vector2 targetPosition)
    {
        StopAllCoroutines(); 
        List<Room> path = pathfinding.GetShortestPath(transform.position, targetPosition);
        foreach (Room room in path) {
            Debug.Log("The room pivot is " + room.getUnitPivot());
        }
        StartCoroutine(MoveToPositionCoroutine(path, targetPosition));
    }

    protected virtual IEnumerator MoveToPositionCoroutine(List<Room> path, Vector2 goToPosition)
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
                List<Door> connectedDoors = path[1].getConnectedDoor(path[0]);
                float distance0 = connectedDoors[1].getDistanceFrom(transform.position);
                float distance1 = connectedDoors[0].getDistanceFrom(transform.position);
                if (distance0 < distance1) {
                    doorToOpen = path[0].getConnectedDoor(path[1])[1];
                    targetPosition = connectedDoors[1].getPosition();
                }
                else {
                    doorToOpen = path[0].getConnectedDoor(path[1])[0];
                    targetPosition = connectedDoors[0].getPosition();
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

    protected virtual IEnumerator PassToDoorCoroutine(Door door) {
        yield return new WaitForSeconds(doorOpenSpeed);

        transform.position = door.getPosition();
        EnterRoom(door.getAttachedRoom());

        Debug.Log("passed through door and went to " + door.getPosition());
        
    }

    protected virtual void EnterRoom(Room room) {
        currentRoom.removeEntity(this);
        room.addEntity(this);
        currentRoom = room;
    }

    public virtual void TakeDamage(float damage) {
        health -= damage;
        if (health <= 0) {
            Die();
        }
    }

    protected virtual void Die() {
        GameEvents.current.EntityDied(this);
        Destroy(this);
    }
}
