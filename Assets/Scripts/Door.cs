using UnityEngine;
using System;
using System.Collections.Generic;

public class Door
{
    private Vector2 position;
    private Vector2Int unitPosition;
    private List<Door> connectedDoors;
    private string orientation;
    private Room attachedRoom;
    public Door (Vector2 position, Vector2Int unitPosition, string orientation, Room room){
        attachedRoom = room;
        this.orientation = orientation;
        if (orientation == "left") {
            this.position = new Vector2(position.x + 0.01f, position.y);
        }
        else {
            this.position = new Vector2(position.x - 0.01f, position.y);
        }
        this.unitPosition = unitPosition;
        connectedDoors = new List<Door>();
    }

    public Room getAttachedRoom() {
        return attachedRoom;
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

    public float getDistanceFrom(Vector2 point) {
        return Math.Abs(position.x - point.x);
    }

    public void debugPrint() {
        Debug.Log("The connected doors at position " + unitPosition + " are:");
        foreach (Door door in connectedDoors) {
            door.debugPrintCurrent();
        }
    }

    public void debugPrintCurrent() {
        Debug.Log("Door position at " + this.unitPosition.x + ", " + this.unitPosition.y);
        //Debug.Log("Door position at " + this.position.x + ", " + this.position.y);
    }

    public string getOrientation() {
        return orientation;
    }

    public bool isConnected(Room room) {
        if (connectedDoors.Contains(room.getLeftDoor()) || connectedDoors.Contains(room.getRightDoor())) {
            return true;
        } else return false;
    }

    public Door getConnectedDoor(Room room) {
        if (connectedDoors.Contains(room.getLeftDoor())) {
            return room.getLeftDoor();
        }
        else if (connectedDoors.Contains(room.getRightDoor())) {
            return room.getRightDoor();
        }
        else {
            return null;
            //Debug.LogError("The door at " + position + "is not connected to room at " + room.getPivot());
        }
    }

}
