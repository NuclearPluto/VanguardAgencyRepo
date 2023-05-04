using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehavior : MonoBehaviour
{
    protected float health = 10f;
    protected float movementSpeed = 5f;
    protected float doorOpenSpeed = 0.5f; //time in seconds to open
    protected float attackRange = 0.1f;
    protected float attackSpeed = 1.0f;
    protected float attackDamage = 2.0f;
    protected Dijekstras pathfinding;
    protected Room currentRoom;
    protected bool animationInProgress = false;
    protected bool currentlyWaiting = false;

    protected Coroutine movementCoroutine;
    protected Coroutine currentCoroutine;

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
        IEnumerator waitForAnimation(Vector2 targetPosition) {
            if (!currentlyWaiting) {
                currentlyWaiting = true;
            }
            else {
                yield break;
            }
            while (animationInProgress) {
                //spinlock baby
                yield return null;
            }
            currentlyWaiting = false;
            MoveToPosition(targetPosition);
        }
        if (animationInProgress) {
            Debug.Log("animation is in progress, so wait....");
            StartCoroutine(waitForAnimation(targetPosition));
            Debug.Log("now go");
            return;
        }
        StopAllCoroutines(); 
        List<Room> path = pathfinding.GetShortestPath(transform.position, targetPosition);
        foreach (Room room in path) {
            Debug.Log("The room pivot is " + room.getUnitPivot());
        }
        movementCoroutine = StartCoroutine(MoveToPositionCoroutine(path, targetPosition));     
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

            movementCoroutine = StartCoroutine(PassToDoorCoroutine(doorToOpen));
            yield return movementCoroutine;
            path.RemoveAt(0);
            movementCoroutine = StartCoroutine(MoveToPositionCoroutine(path, goToPosition));
            yield return movementCoroutine;
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
        StartCoroutine(ShakeAnimation(transform));
        if (health <= 0) {
            Die();
        }
    }

    public void AttackEntity(EntityBehavior entity) {
        currentCoroutine = StartCoroutine(CheckAttackRangeAndAttack(entity));
    }

   protected virtual IEnumerator CheckAttackRangeAndAttack(EntityBehavior entity)
    {
        IEnumerator helperFunc(Vector2 targetPosition) // Change Coroutine to IEnumerator
        {
            List<Room> path = pathfinding.GetShortestPath(transform.position, targetPosition);
            return MoveToPositionCoroutine(path, targetPosition); // Remove StartCoroutine
        }
        if (entity.gameObject == null) {
            Debug.Log("it's gone");
        }
        while (!IsEntityDestroyed(entity))
        {
            //Debug.Log("Distance is " + Vector2.Distance(transform.position, entity.transform.position) + " and attack range is " + attackRange);
            if (Vector2.Distance(transform.position, entity.transform.position) <= attackRange)
            {
                if (movementCoroutine != null) {
                    StopCoroutine(movementCoroutine);
                    movementCoroutine = null;
                }

                yield return new WaitForSeconds(attackSpeed);
                if (IsEntityDestroyed(entity)) {
                    continue;
                }
                
                if (Vector2.Distance(transform.position, entity.transform.position) <= attackRange)
                {
                    Debug.Log("attack");
                    yield return StartCoroutine(AttackAnimation(entity));
                    entity.TakeDamage(attackDamage);
                }
                else
                {
                    movementCoroutine = StartCoroutine(helperFunc(entity.transform.position));
                    while (movementCoroutine != null && Vector2.Distance(transform.position, entity.transform.position) > attackRange) {
                        yield return null;
                    }
                }
            }
            else
            {
                if (IsEntityDestroyed(entity)) {
                    continue;
                }
                movementCoroutine = StartCoroutine(helperFunc(entity.transform.position));
                while (movementCoroutine != null && Vector2.Distance(transform.position, entity.transform.position) > attackRange) {
                    yield return null;
                }
            }
        }
    }
    protected bool IsEntityDestroyed(EntityBehavior entity)
    {
        try
        {
            return entity == null || entity.gameObject == null;
        }
        catch (MissingReferenceException)
        {
            return true;
        }
    }

    protected virtual IEnumerator AttackAnimation(EntityBehavior entity)
    {
        return AttackAnimationCoroutine(entity);
    }

    protected virtual IEnumerator AttackAnimationCoroutine(EntityBehavior entity)
    {
        animationInProgress = true;

        Vector3 originalPosition = transform.position;

        float smallDistance = 0.5f;
        float largerDistance = 1.0f;
        float moveDuration = 0.1f;
        float waitTime = 0.1f;

        Vector3 direction = (entity.transform.position - transform.position).normalized;
        Vector3 moveAwayTarget = transform.position - direction * smallDistance;
        yield return StartCoroutine(animationMove(transform, moveAwayTarget, moveDuration));

        yield return new WaitForSeconds(waitTime);


        Vector3 moveTowardsTarget = transform.position + direction * (smallDistance + largerDistance);
        yield return StartCoroutine(animationMove(transform, moveTowardsTarget, moveDuration));
        yield return StartCoroutine(animationMove(transform, originalPosition, moveDuration));

        animationInProgress = false;
        yield break;
    }

    IEnumerator animationMove(Transform t, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = t.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            t.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        t.position = targetPosition;
    }


    private IEnumerator ShakeAnimation(Transform target, float duration = 0.1f, float magnitude = 0.1f)
    {
        animationInProgress = true;

        Vector3 originalPosition = target.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            target.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.position = originalPosition;

        animationInProgress = false;
    }

    protected virtual void Die() {
        GameEvents.current.EntityDied(this);
        Destroy(gameObject);
    }
}
