using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityBehavior
{

    protected override void Start() {
        base.Start();
        CheckWakeup();
    }

    // protected override IEnumerator PassToDoorCoroutine(Door door) {
    //     yield return StartCoroutine(base.PassToDoorCoroutine(door));
    // }

    protected override void Die() {
        base.Die();
    }

    protected virtual void OnEnable()
    {
        GameEvents.current.onPlayerVisionChange.AddListener(CheckWakeup);
        GameEvents.current.onPlayerEnterRoom.AddListener(CheckWakeup);
    }

    protected virtual void OnDisable() {
        GameEvents.current.onPlayerEnterRoom.RemoveListener(CheckWakeup);
        GameEvents.current.onPlayerVisionChange.RemoveListener(CheckWakeup);
    }

    protected virtual void CheckWakeup() {
        Debug.Log("Current room is at " + currentRoom.getPivot() + " and fog is " + currentRoom.isFog());
        if (currentRoom.isFog()) {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        else {
            Debug.Log("yoooo");
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    public bool IsInFog() {
        return currentRoom.isFog();
    }

    protected override void EnterRoom(Room room) {
        base.EnterRoom(room);
        CheckWakeup();
    }
}