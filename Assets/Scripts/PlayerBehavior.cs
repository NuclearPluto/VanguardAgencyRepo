using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 3f;

    void Start()
    {
        
    }

    public void MoveToPosition(Vector2 targetPosition)
    {
        StopAllCoroutines(); 
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
