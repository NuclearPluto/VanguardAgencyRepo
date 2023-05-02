using System.Collections.Generic;
using UnityEngine;

namespace PlatformConnectivity {
public class Platform
{
    private const float epsilon = 1e-6f;

    private List<Vector2Int> openPositions;
    private List<Vector2Int> closedPositions;

    private Vector2 initialPosition;
    private float cellWidth;
    private LookupMap lookupMap;

    public Platform(Vector2 initialPosition, float cellWidth) {
        this.cellWidth = cellWidth;
        openPositions = new List<Vector2Int>();
        closedPositions = new List<Vector2Int>();
        openPositions.Add(new Vector2Int(0, 0));
        this.initialPosition = initialPosition;
    }

    public Vector2 getPosition() {
        int randomIndex = Random.Range(0, openPositions.Count);
        Vector2 position = (Vector2)openPositions[randomIndex] * cellWidth + initialPosition;
        return position;
    }

    public List<Vector2> getClosedPositionsInWorldSpace() {
        List<Vector2> returnList = new List<Vector2>();
        foreach(Vector2Int position in closedPositions) {
            returnList.Add(unitsToWorld(position));
        }
        return returnList;
    }

    //ONLY WORKS FOR X BY 1 ROOMS
    private void updateClosedPositions(Room currentRoom) {
        for (int x = 0; x < currentRoom.getUnitDimensions().x; x++) {
            closedPositions.Add(currentRoom.getUnitPivot() + new Vector2Int(x, 0));
            openPositions.Remove(currentRoom.getUnitPivot() + new Vector2Int(x, 0));
        }
    } 

    //ONLY WORKS FOR X BY 1 ROOMS
    private void updateOpenPositions(List<Room> listRooms, Room currentRoom, List<Vector2Int> tempPositions) {
        foreach (Vector2Int tempPosition in tempPositions) {
            if (openPositions.Contains(tempPosition) || closedPositions.Contains(tempPosition)) {
                int index = lookupMap.getIndexAt(unitsToWorld(tempPosition));
                if (closedPositions.Contains(tempPosition) && !listRooms[index].isRoomConnected(currentRoom) && !currentRoom.isRoomConnected(listRooms[index])) {
                    //if (!listRooms[index].isRoomConnected(currentRoom)) {
                        listRooms[index].connectRoom(currentRoom);
                    //}
                    //if (!currentRoom.isRoomConnected(listRooms[index])) {
                        currentRoom.connectRoom(listRooms[index]);
                    //}
                }
            }
            else {
                openPositions.Add(tempPosition);
            }
        }
    }

    private Vector2 unitsToWorld(Vector2Int vector) {
        float xPos = vector.x * cellWidth;
        float yPos = vector.y * cellWidth;
        return new Vector2(xPos, yPos);
    }

    public void updateConnectivity(List<Room> listRooms, Room currentRoom, LookupMap lookupMap) {
        //Debug.Log("ROOM ID TO UPDATE IS " + currentRoom.getRoomID());
        this.lookupMap = lookupMap;
        List<Vector2Int> tempPositions = getSurroundPositions(currentRoom);
        updateClosedPositions(currentRoom);
        updateOpenPositions(listRooms, currentRoom, tempPositions);
    }

    //ONLY WORKS FOR X BY 1 ROOMS
    private List<Vector2Int> getSurroundPositions(Room currentRoom) {
        //Debug.Log($"Creating {platformType} at {createPosition}");
        List<Vector2Int> tempPositions = new List<Vector2Int>();
        Vector2Int roomPivot = currentRoom.getUnitPivot();

        //left of pivot first
        tempPositions.Add(roomPivot + new Vector2Int(-1, 0));
        //top and bottom for length
        for (int x = 0; x < currentRoom.getUnitDimensions().x; x++) {
            tempPositions.Add(roomPivot + new Vector2Int(x, 1));
            tempPositions.Add(roomPivot + new Vector2Int(x, -1));
        }
        //right of pivot
        tempPositions.Add(roomPivot + new Vector2Int(currentRoom.getUnitDimensions().x, 0));
        return tempPositions;
    }
}
}