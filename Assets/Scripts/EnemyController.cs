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

    protected virtual void OnEnable()
    {
        GameEvents.current.onPlayerEnterRoom.AddListener(CheckWakeup);
    }

    protected virtual void OnDisable() {
        GameEvents.current.onPlayerEnterRoom.RemoveListener(CheckWakeup);
    }

    protected virtual void CheckWakeup() {
        Debug.Log("Current room is at " + currentRoom.getPivot() + " and fog is " + currentRoom.isFog());
        if (currentRoom.isFog()) {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        else {
            Debug.Log("yoooo");
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    protected override void EnterRoom(Room room) {
        base.EnterRoom(room);
        CheckWakeup();
    }
}