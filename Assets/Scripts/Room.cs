using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room : MonoBehaviour
{
    private const int MAX_ROOM_X_LENGTH = 3;
    private const int MAX_ROOM_Y_LENGTH = 2;

    public GameObject fogOfWarPrefab;

    private bool isFogToggle = true;
    private GameObject fogOfWarInstance;
    private int roomID;
    private Vector2 roomPivot;
    private List<Room> connectedRooms;
    private HashSet<Room> connectedRoomsHash;
    private float cellWidth;
    private Vector2 roomDimensions;
    private Vector2Int unitDimensions;
    private Door leftPosition;
    private Door rightPosition;
    private List<EntityBehavior> entitiesInRoom;
    // Start is called before the first frame update
    void Awake() {
        connectedRooms = new List<Room>();
        connectedRoomsHash = new HashSet<Room>();
        entitiesInRoom = new List<EntityBehavior>();
    }

    void Start() {
        fogOfWarInstance = Instantiate(fogOfWarPrefab, getCenter(), Quaternion.identity);
        fogOfWarInstance.transform.SetParent(transform);
        fogOfWarInstance.transform.localScale = new Vector3(roomDimensions.x, roomDimensions.y, 0);
        fogOfWarInstance.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0.5f);
    }

    public bool isFog() {
        return isFogToggle;
    }

    public void addEntity(EntityBehavior entity) {
        entitiesInRoom.Add(entity);
    }

    public void removeEntity(EntityBehavior entity) {
        entitiesInRoom.Remove(entity);
    }

    public List<EntityBehavior> getEntities() {
        return entitiesInRoom;
    }

    public void toggleFog() {
        isFogToggle = !isFogToggle;
        if (isFogToggle) {
            fogOfWarInstance.SetActive(true);
        } 
        else {
            fogOfWarInstance.SetActive(false);
        }
    }

    public void toggleNeighborFog() {
        foreach (Room room in connectedRooms) {
            room.toggleFog();
        }
    }

    public Vector2 getCenter() {
        Vector2 halfDimension = new Vector2(roomDimensions.x/2, 0);
        Vector2 returnCenter = halfDimension + leftPosition.getPosition();
        return returnCenter;
    }

    public void setID(int given) {
        roomID = given;
    }

    public void setPivot(Vector2 createPosition) {
        roomPivot = createPosition;
    }

    public void setCellWidth(float given) {
        cellWidth = given;
    }

    public void setRoomDimensions(Vector2Int given) {
        float length = given.x * cellWidth;
        float height = given.y * cellWidth;
        roomDimensions = new Vector2(length, height);
        unitDimensions = given;
    }

    public void setDoors() {
        Vector2 tempLeftPosition = new Vector2(roomPivot.x - cellWidth/2, roomPivot.y);
        Vector2 tempRightPosition = new Vector2(roomPivot.x - cellWidth/2 + roomDimensions.x, roomPivot.y);
        leftPosition = new Door(tempLeftPosition, worldToUnit(new Vector2(roomPivot.x, roomPivot.y)), "left", this);
        rightPosition = new Door(tempRightPosition, worldToUnit(new Vector2(roomPivot.x + roomDimensions.x, roomPivot.y)), "right", this);
    }

    public void connectRoom(Room room) {
        connectedRooms.Add(room);
        connectedRoomsHash.Add(room);

        Door tempLeftPosition = room.getLeftDoor();
        Door tempRightPosition = room.getRightDoor();
        //if connected room is centered
        if (leftPosition.unitDistance(tempLeftPosition).x == -rightPosition.unitDistance(tempRightPosition).x) {
            leftPosition.connectDoor(tempLeftPosition);
            rightPosition.connectDoor(tempRightPosition);
        } 
        //if connected room is shifted right
        else if (leftPosition.unitDistance(tempLeftPosition).x > 0 && rightPosition.unitDistance(tempRightPosition).x > 0) {
            rightPosition.connectDoor(tempLeftPosition);
        }
        else if (leftPosition.unitDistance(tempLeftPosition).x < 0 && rightPosition.unitDistance(tempRightPosition).x < 0) {
            leftPosition.connectDoor(tempRightPosition);
        }
        //closest door pair gets connected, if equidistant then it goes in direction of shift
        else { 
            //calculate which door is closest to any connected room doors
            int closestLeftDistance;
            int closestRightDistance;
            if (leftPosition.absUnitDistance(tempLeftPosition).x < leftPosition.absUnitDistance(tempRightPosition).x) {
                closestLeftDistance = leftPosition.absUnitDistance(tempLeftPosition).x;
            }
            else {
                closestLeftDistance = leftPosition.absUnitDistance(tempRightPosition).x;
            }
            if (rightPosition.absUnitDistance(tempRightPosition).x < rightPosition.absUnitDistance(tempLeftPosition).x) {
                closestRightDistance = rightPosition.absUnitDistance(tempRightPosition).x;
            }
            else {
                closestRightDistance = rightPosition.absUnitDistance(tempLeftPosition).x;
            }

            //if left door is closest
            if (closestLeftDistance < closestRightDistance) {
                //if equidistant
                if (leftPosition.absUnitDistance(tempLeftPosition).x == leftPosition.absUnitDistance(tempRightPosition).x) {
                    leftPosition.connectDoor(tempRightPosition);
                }
                else if (leftPosition.absUnitDistance(tempLeftPosition).x < leftPosition.absUnitDistance(tempRightPosition).x) {
                    leftPosition.connectDoor(tempLeftPosition);
                    //leftPosition.connectDoor(tempRightPosition);
                }
                else {
                    leftPosition.connectDoor(tempRightPosition);
                }
            }
            //if right door is closest
            else if (closestRightDistance < closestLeftDistance) {
                //if equidistant
                if (rightPosition.absUnitDistance(tempLeftPosition).x == rightPosition.absUnitDistance(tempRightPosition).x) {
                    rightPosition.connectDoor(tempLeftPosition);
                }
                else if (rightPosition.absUnitDistance(tempLeftPosition).x < rightPosition.absUnitDistance(tempRightPosition).x) {
                    rightPosition.connectDoor(tempLeftPosition);
                }
                else {
                    rightPosition.connectDoor(tempRightPosition);
                    //rightPosition.connectDoor(tempLeftPosition);
                }
            }
            else {
                Debug.Log("This should never be printed at " + roomPivot.x + ", " + roomPivot.y);
            }
        }
    }

    public Door getLeftDoor() {
        return leftPosition;
    }

    public Door getRightDoor() {
        return rightPosition;
    }

    public Vector2 getPivot() {
        return roomPivot;
    }

    private Vector2Int worldToUnit(Vector2 world) {
        int xPos = (int) System.Math.Round(world.x / cellWidth);
        int yPos = (int) System.Math.Round(world.y / cellWidth);
        return new Vector2Int(xPos, yPos);
    } 

    public Vector2 getRoomDimensions() {
        return roomDimensions;
    }

    public Vector2Int getUnitDimensions() {
        return unitDimensions;
    }

    public Vector2Int getUnitPivot() {
        int xPos = (int)System.Math.Round(roomPivot.x / cellWidth);
        int yPos = (int)System.Math.Round(roomPivot.y / cellWidth);
        return new Vector2Int(xPos, yPos);
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

    public bool isRoomConnected(Room room) {
        return connectedRoomsHash.Contains(room);
    }

    public float getCellWidth() {
        return cellWidth;
    }

    public float[] getWorldDimensions() {
        float[] returnDimensions = {
            roomPivot.x - (cellWidth/2),
            roomPivot.y - (cellWidth/2),
            roomPivot.x + roomDimensions.x,
            roomPivot.y + roomDimensions.y
        };
        return returnDimensions;
    }

    //TODO: CHANGE FOR WHEN ITS TIME TO REFACTOR FROM DIJEKSTRA'S TO DYNAMIC PROGRAMMING + A* SEARCH
    public float getEdgeWeight(Room room) {
        return 1;
    }

    //returns 2 rooms in list if both doors are connected
    public List<Door> getConnectedDoor(Room room) {
        List<Door> returnList = new List<Door>();
        if (leftPosition.getConnectedDoor(room) != null) {
            returnList.Add(leftPosition.getConnectedDoor(room));
        }
        if (rightPosition.getConnectedDoor(room) != null) {
            returnList.Add(rightPosition.getConnectedDoor(room));
        }

        return returnList; 
    }
}
