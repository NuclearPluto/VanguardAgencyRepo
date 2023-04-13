using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room : MonoBehaviour
{
    private const int MAX_ROOM_X_LENGTH = 3;
    private const int MAX_ROOM_Y_LENGTH = 2;

    private int roomID;
    private Vector2 roomPivot;
    private List<Room> connectedRooms;
    private float cellWidth;
    // 1x1 = 1, 1x2 = 2, 1x3 = 3, 2x1 = 4, 2x2 = 5, 2x3 = 6
    private int roomType;
    // Start is called before the first frame update
    void Awake() {
        connectedRooms = new List<Room>();
        roomID = Random.Range(0, 10000);
    }

    public void setPivot(Vector2 createPosition) {
        roomPivot = createPosition;
    }

    public void setCellWidth(float given) {
        cellWidth = given;
    }

    public void setRoomType(int given) {
        roomType = given;
    }

    public void connectRoom(Room room) {
        connectedRooms.Add(room);
    }

    public Vector2 getPivot() {
        return roomPivot;
    }

    public bool isConnectedPivot(Vector2 givenPivot) {
        if (givenPivot.x + cellWidth == roomPivot.x && givenPivot.y == roomPivot.y) {
            return true;
        }
        if (givenPivot.x - cellWidth == roomPivot.x && givenPivot.y == roomPivot.y) {
            return true;
        }
        if (givenPivot.y + cellWidth == roomPivot.y && givenPivot.x == roomPivot.x) {
            return true;
        }
        if (givenPivot.y - cellWidth == roomPivot.y && givenPivot.x == roomPivot.x) {
            return true;
        }
        return false;
    }

    public Vector2 getRoomDimensions() {
        Vector2 returnVector = new Vector2(0, 0);
        if (roomType > 0 && roomType < MAX_ROOM_X_LENGTH + 1) {
            returnVector.x = cellWidth * roomType;
            returnVector.y = cellWidth;
        }
        else Debug.Log("THIS SHOULD NOT BE SEEN");

        //Debug.Log("ROOM DIMENSIONS ARE " + returnVector);
        return returnVector;
    }

    public bool isPointInRoom(Vector2 givenVector) {
        Vector2 localPivot = roomPivot;
        localPivot.x -= (cellWidth/2);
        localPivot.y -= (cellWidth/2);

        if (givenVector.x > localPivot.x && givenVector.x < (localPivot.x + getRoomDimensions().x))
            if (givenVector.y > localPivot.y && givenVector.y < (localPivot.y + getRoomDimensions().y))
                return true;
        
        return false;
    }

    public int getRoomID() {
        return roomID;
    }

    public List<Room> getConnectedRooms() {
        return connectedRooms;
    }

    public float getCellWidth() {
        return cellWidth;
    }

    public float[] getWorldDimensions() {
        float[] returnDimensions = {
            roomPivot.x - (cellWidth/2),
            roomPivot.y - (cellWidth/2),
            roomPivot.x + (cellWidth/2 + cellWidth * getDimensionsFromPlatformType().x),
            roomPivot.y + (cellWidth/2 + cellWidth * getDimensionsFromPlatformType().y)
        };
        return returnDimensions;
    }

    private Vector2 getDimensionsFromPlatformType() {
        switch (roomType) {
            case 1: 
                return new Vector2(1,1);
            case 2: 
                return new Vector2(2,1);
            case 3: 
                return new Vector2(3,1);
            default:
                Debug.Log("UNDEFINED PLATFORM TYPE");
                return new Vector2(-1, -1);
        }
    }
}
