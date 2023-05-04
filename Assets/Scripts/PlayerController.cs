using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityBehavior
{
    private int visionRange = 1;

    protected override void Start() {
        base.Start();
        attackRange = 3f;
        attackDamage = 1f;
        toggleVision();
    }

    // protected override IEnumerator PassToDoorCoroutine(Door door) {
    //     yield return StartCoroutine(base.PassToDoorCoroutine(door));
    // }

    protected override void EnterRoom(Room room) {
        toggleVision();
        base.EnterRoom(room);
        toggleVision();
        GameEvents.current.PlayerEnteredRoom();
    }

    public void toggleVision() {
        void helperFunc(Room room, List<Room> visited, int visionIntensity) {
            if (visionIntensity >= 0) {
                room.toggleFog();
                visionIntensity -= 1;
                visited.Add(room);

                foreach(Room roomNext in room.getConnectedRooms()) {
                    if (!visited.Contains(roomNext)) {
                        helperFunc(roomNext, visited, visionIntensity);
                    }                
                }
            }
        }

        List<Room> visited = new List<Room>();
        helperFunc(currentRoom, visited, visionRange);
    }

    public void changeVisionRange(int newVisionRange) {
        toggleVision();
        visionRange = newVisionRange;
        toggleVision();
        GameEvents.current.PlayerVisionChange();
    }

    protected override void Die() {
        base.Die();
    }

}