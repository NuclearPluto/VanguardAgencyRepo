using UnityEngine;
using System;
using System.Collections.Generic;

public class Door
{
    private Vector2 position;
    private Vector2Int unitPosition;
    private List<Door> connectedDoors;
    public Door (Vector2 position, Vector2Int unitPosition){
        this.position = position;
        this.unitPosition = unitPosition;
        connectedDoors = new List<Door>();
    }

    public Vector2Int unitDistance(Door door) {
        return door.unitPosition - this.unitPosition;
    }

    public Vector2Int absUnitDistance(Door door) {
        int xPos = Math.Abs(door.unitPosition.x - this.unitPosition.x);
        int yPos = Math.Abs(door.unitPosition.y - this.unitPosition.y);
        return new Vector2Int(xPos, yPos);
    }

    public Vector2 getPosition() {
        return position;
    }

    public Vector2Int getUnitPosition() {
        return unitPosition;
    }

    public void connectDoor(Door door) {
        this.connectedDoors.Add(door);
    }

    public void debugPrint() {
        Debug.Log("The connected doors at position " + unitPosition + " are:");
        foreach (Door door in connectedDoors) {
            door.debugPrintCurrent();
        }
    }

    public void debugPrintCurrent() {
        Debug.Log("Door position at " + this.unitPosition.x + ", " + this.unitPosition.y);
    }
}
