using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlatformConnectivity;

//TODO: USE SOME SORT OF DESIGN PATTERN RATHER THAN WHATEVER THIS UGLY POS IS
public class StageGeneration : MonoBehaviour
{
    public GameObject Room;
    public GameObject platformCell;
    public GameObject platformCellHallway;
    public GameObject platformCellLeft;
    public GameObject platformCellRight;
    public GameObject DebugCircle;
    //stageSize - 1 = small - 2 = medium - 3 = large
    public int stageSize = 3;
    //public bool debugToggle = false;

    private List<Room> listRooms;
    private Platform platformControl;
    private Vector3 createPosition;
    private float cellWidth;
    
    void Awake() {
        createPosition = transform.position;
        cellWidth = platformCell.GetComponent<SpriteRenderer>().bounds.size.x;
        platformControl = new Platform(createPosition, cellWidth);
    }

    void Start()
    {
        //Debug.Log($"DIOSNFIOWSEJFIOEWJ Current Cell Width is {cellWidth}");
        createStage(stageSize);
        listRooms = platformControl.getListRooms();
        LookupMap lookupMap = new LookupMap(listRooms, platformControl.getMinMax());
        //TODO: INITIALIZE QUADTREE
        debugClosed();
    }

    public void createStage(int type) {
        List<int> platformsToCreate = new List<int>();
        switch (type) {
            case 1: 
                platformsToCreate.AddRange(new int[] {1, 1, 2, 2});
                break;
            case 2: 
                platformsToCreate.AddRange(new int[] {1, 1, 2, 2, 2, 2, 3, 3});
                break;
            case 3:
                platformsToCreate.AddRange(new int[] {1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3});
                break;
        }

        while (platformsToCreate.Count > 0) {
            int randomNum = Random.Range(0, platformsToCreate.Count);
            int platformToCreate = platformsToCreate[randomNum];
            platformsToCreate.RemoveAt(randomNum);
            Room current = createPlatform(platformToCreate);
            //current.setPivot(createPosition);
            //current.setCellWidth(cellWidth);
            //current.setRoomType(platformToCreate);
        }
    }

    public Room createPlatform(int type) {
        bool dontUpdateConnectivity = false;
        Room returnRoom = null;
        switch (type) {
            case 1:
                createPosition = platformControl.getPosition();
                returnRoom = create1x1();
                break;
            case 2:
                createPosition = platformControl.getPosition();
                //BUG IS HERE
                if (platformControl.isRightClosed(createPosition) || platformControl.isLeftClosed(createPosition)) {
                    if (platformControl.isLeftClosed(createPosition)) {
                        returnRoom = createPlatform(type);
                        dontUpdateConnectivity = true;
                    }
                    else {
                        createPosition += new Vector3(-cellWidth, 0, 0);
                        returnRoom = create1x2();
                    }
                }
                else returnRoom = create1x2();
                break;
            case 3:
                createPosition = platformControl.getPosition();
                if (platformControl.isRightClosed(createPosition) || platformControl.isLeftClosed(createPosition)) {
                    if (platformControl.isLeftClosed(createPosition) || platformControl.isLeftClosed(createPosition + new Vector3(-cellWidth, 0, 0))) {
                        returnRoom = createPlatform(type);
                        dontUpdateConnectivity = true;
                    }
                    else {
                        createPosition += new Vector3(-cellWidth*2, 0, 0);
                        returnRoom = create1x3();
                    }
                }
                else if (platformControl.isRightClosed(createPosition + new Vector3(cellWidth, 0, 0)) || platformControl.isLeftClosed(createPosition + new Vector3(cellWidth, 0, 0))) {
                    returnRoom = createPlatform(type);
                    dontUpdateConnectivity = true;
                }
                else returnRoom = create1x3();
                break;
        }
        if (!dontUpdateConnectivity) {
            returnRoom.setPivot(createPosition);
            returnRoom.setCellWidth(cellWidth);
            returnRoom.setRoomType(type);
            platformControl.updateConnectivity(returnRoom, createPosition, type);
        } else dontUpdateConnectivity = false;

        return returnRoom;
    }

    public Room create1x1()
    {
        GameObject roomInstance = Instantiate(Room, createPosition, Quaternion.identity);
        GameObject platformCellInstance = Instantiate(platformCell, createPosition, Quaternion.identity);

        platformCellInstance.transform.SetParent(roomInstance.transform, true);

        return roomInstance.GetComponent<Room>();
    }

    public Room create1x2()
    {
        GameObject roomInstance = Instantiate(Room, createPosition, Quaternion.identity);
        GameObject platformCellLeftInstance = Instantiate(platformCellLeft, createPosition, Quaternion.identity);
        GameObject platformCellRightInstance = Instantiate(platformCellRight, createPosition + new Vector3(cellWidth, 0, 0), Quaternion.identity);

        platformCellLeftInstance.transform.SetParent(roomInstance.transform, true);
        platformCellRightInstance.transform.SetParent(roomInstance.transform, true);

        return roomInstance.GetComponent<Room>();
    }

    public Room create1x3()
    {
        GameObject roomInstance = Instantiate(Room, createPosition, Quaternion.identity);
        GameObject platformCellLeftInstance = Instantiate(platformCellLeft, createPosition, Quaternion.identity);
        GameObject platformCellHallwayInstance = Instantiate(platformCellHallway, createPosition + new Vector3(cellWidth, 0, 0), Quaternion.identity);
        GameObject platformCellRightInstance = Instantiate(platformCellRight, createPosition + new Vector3(cellWidth * 2, 0, 0), Quaternion.identity);

        platformCellLeftInstance.transform.SetParent(roomInstance.transform, true);
        platformCellRightInstance.transform.SetParent(roomInstance.transform, true);
        platformCellHallwayInstance.transform.SetParent(roomInstance.transform, true);

        return roomInstance.GetComponent<Room>();
    }


    private void pointerController(int movement) {
        switch (movement) {
            case 1: 
                //move drawing pointer up
                createPosition.y += cellWidth;
                break;
            case 2: 
                //move drawing pointer right
                createPosition.x += cellWidth;
                break;
            case 3: 
                //move drawing pointer down
                createPosition.y -= cellWidth;
                break;
            case 4: 
                //move drawing pointer left
                createPosition.x -= cellWidth;
                break;
        }
    }

    public void debugClosed() {
        foreach(Vector3 position in platformControl.closedPositions) {
            Instantiate(DebugCircle, position, Quaternion.identity);
        }
    }

    public List<Room> getListRooms() {
        return listRooms;
    }
}
