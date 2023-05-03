using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemy : EnemyController
{

    protected override void Start() {
        base.Start();
        movementSpeed = 0f;
        Debug.Log("Current fog is " + currentRoom.isFog() + " at room " + currentRoom.getPivot());
    }

    public override void MoveToPosition(Vector2 targetPosition) {

    }

    // protected override IEnumerator PassToDoorCoroutine(Door door) {
    //     yield return StartCoroutine(base.PassToDoorCoroutine(door));
    // }
}