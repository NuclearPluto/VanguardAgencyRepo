using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityBehavior
{

    protected override void Start() {
        base.Start();
    }

    // protected override IEnumerator PassToDoorCoroutine(Door door) {
    //     yield return StartCoroutine(base.PassToDoorCoroutine(door));
    // }

    protected override void Die() {
        base.Die();
    }
}