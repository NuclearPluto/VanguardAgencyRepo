using System.Collections.Generic;
using UnityEngine;

namespace PlatformConnectivity {
public class Platform
{
    List<Vector3> openPositions;
    public List<Vector3> closedPositions;
    public List<Room> listRooms;
    float cellWidth;

    public Platform(Vector3 initialPosition, float cellWidth) {
        openPositions = new List<Vector3>();
        closedPositions = new List<Vector3>();
        listRooms = new List<Room>();
        openPositions.Add(initialPosition);
        this.cellWidth = cellWidth;
    }

    public Vector3 getPosition() {
        int randomIndex = Random.Range(0, openPositions.Count);
        Vector3 position = openPositions[randomIndex];
        return position;
    }

    public void updateConnectivity(Room currentRoom, Vector3 createPosition, int platformType) {
        //Debug.Log("ROOM ID TO UPDATE IS " + currentRoom.getRoomID());
        List<Vector3> tempPositions = getTempPositions(currentRoom, createPosition, platformType);
        openPositions.Remove(createPosition);
        switch(platformType) {
            case 2:
                openPositions.Remove(createPosition + new Vector3(cellWidth, 0, 0));
                break;
            case 3:
                openPositions.Remove(createPosition + new Vector3(cellWidth*2, 0, 0));
                break;
        }
        //Debug.Log($"Platform type is {platformType}. Amount of temp positions in tempPositions is {tempPositions.Count}");

        for (int x = 0; x < tempPositions.Count; x++) {
            //Debug.Log($"tempPosition is {tempPositions[x]}");
            if (closedPositions.Contains(tempPositions[x]) || openPositions.Contains(tempPositions[x])) {
                if (closedPositions.Contains(tempPositions[x])) {
                    foreach (Room room in listRooms) {
                        foreach (Vector2 tempPosition in tempPositions) {
                            if (room.isPointInRoom(tempPosition) && !currentRoom.getConnectedRooms().Contains(room) && currentRoom.getPivot() != room.getPivot()) {
                                Debug.Log("Pivot " + currentRoom.getPivot() + " is not equal to " + room.getPivot());
                                currentRoom.connectRoom(room);
                                room.connectRoom(currentRoom);
                            }
                        }
                    }
                }
                continue;
            }
            else {
                //Debug.Log($"Adding {tempPositions[x]} to openPositions");
                openPositions.Add(tempPositions[x]);
            }
        }

        listRooms.Add(currentRoom);
        //Debug.Log("NEXT");
    }

    private List<Vector3> getTempPositions(Room currentRoom, Vector3 createPosition, int platformType) {
        //Debug.Log($"Creating {platformType} at {createPosition}");
        List<Vector3> tempPositions = new List<Vector3>();
        switch (platformType) {
            case 1:
                closedPositions.Add(createPosition);

                tempPositions.Add(createPosition + new Vector3(cellWidth, 0, 0));
                tempPositions.Add(createPosition + new Vector3(-cellWidth, 0, 0));
                tempPositions.Add(createPosition + new Vector3(0, cellWidth, 0));
                tempPositions.Add(createPosition + new Vector3(0, -cellWidth, 0));
                break;
            case 2:
                closedPositions.Add(createPosition);
                closedPositions.Add(createPosition + new Vector3(cellWidth, 0, 0));
                tempPositions.Add(createPosition + new Vector3(cellWidth*2, 0, 0));
                tempPositions.Add(createPosition + new Vector3(-cellWidth, 0, 0));
                tempPositions.Add(createPosition + new Vector3(0, cellWidth, 0));
                tempPositions.Add(createPosition + new Vector3(0, -cellWidth, 0));
                tempPositions.Add(createPosition + new Vector3(cellWidth, cellWidth, 0));
                tempPositions.Add(createPosition + new Vector3(cellWidth, -cellWidth, 0));
                break;
            case 3:
                closedPositions.Add(createPosition);
                closedPositions.Add(createPosition + new Vector3(cellWidth, 0, 0));
                closedPositions.Add(createPosition + new Vector3(cellWidth*2, 0, 0));
                tempPositions.Add(createPosition + new Vector3(cellWidth*3, 0, 0));
                tempPositions.Add(createPosition + new Vector3(-cellWidth, 0, 0));
                tempPositions.Add(createPosition + new Vector3(0, cellWidth, 0));
                tempPositions.Add(createPosition + new Vector3(0, -cellWidth, 0));
                tempPositions.Add(createPosition + new Vector3(cellWidth, cellWidth, 0));
                tempPositions.Add(createPosition + new Vector3(cellWidth, -cellWidth, 0));
                tempPositions.Add(createPosition + new Vector3(cellWidth*2, cellWidth, 0));
                tempPositions.Add(createPosition + new Vector3(cellWidth*2, -cellWidth, 0));
                break;
            default:
                Debug.Log("ERROR, UNKNOWN PLATFORM TYPE IN PLATFORMCONNECTIVITY - GET TEMP POSITIONS");
                break;
        }
        return tempPositions;
    }

    public bool isLeftClosed(Vector3 currentPosition) {
        if (closedPositions.Contains(currentPosition + new Vector3(-cellWidth, 0, 0))) {
            return true;
        }
        else return false;
    }

    public bool isRightClosed(Vector3 currentPosition) {
        if (closedPositions.Contains(currentPosition + new Vector3(cellWidth, 0, 0))) {
            return true;
        }
        else return false;
    }

    public List<Room> getListRooms() {
        return listRooms;
    }
}
}