using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemy : EnemyController
{

    protected override void Start() {
        base.Start();
        movementSpeed = 0f;
    }

    // protected override IEnumerator PassToDoorCoroutine(Door door) {
    //     yield return StartCoroutine(base.PassToDoorCoroutine(door));
    // }
}