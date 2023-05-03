using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlatformConnectivity;

//TODO: USE SOME SORT OF DESIGN PATTERN RATHER THAN WHATEVER THIS UGLY POS IS
public class StageGeneration : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject platformCell;
    public GameObject platformCellHallway;
    public GameObject platformCellLeft;
    public GameObject platformCellRight;
    public GameObject door;
    public GameObject DebugCircle;
    public GameObject PlayerPrefab;
    //stageSize - 1 = small - 2 = medium - 3 = large
    public int stageSize = 3;
    //public bool debugToggle = false;

    private List<Room> listRooms;
    private Platform platformControl;
    private LookupMap lookupMap;
    private Dijekstras pathfinding;
    private Vector3 createPosition;
    private float cellWidth;
    private int numRooms = 0;
    
    void Awake() {
        listRooms = new List<Room>();
        createPosition = transform.position;
        cellWidth = platformCell.GetComponent<SpriteRenderer>().bounds.size.x;
        platformControl = new Platform(createPosition, cellWidth);
    }

    void Start()
    {
        lookupMap = new LookupMap(cellWidth);
        createStage(stageSize);
        pathfinding = new Dijekstras(lookupMap, listRooms);
        createPlayer();
        debugClosed();
    }

    public void createPlayer() {
        Instantiate(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
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
            Room currentRoom = createPlatform(platformToCreate);
            
            listRooms.Add(currentRoom);
            lookupMap.insertRoom(currentRoom);
            platformControl.updateConnectivity(listRooms, currentRoom, lookupMap);
        }
    }

    private Vector2Int getDimensionsFromPlatformType(int type) {
        switch(type) {
            case 1:
                return new Vector2Int(1, 1);
            case 2:
                return new Vector2Int(2, 1);
            case 3:
                return new Vector2Int(3, 1);
            case 4:
                return new Vector2Int(1, 2);
            case 5:
                return new Vector2Int(2, 2);
            case 6:
                return new Vector2Int(3, 2);
            default:
                Debug.Log("ERROR: INVALID PLATFORM TYPE IN STAGEGENERATION.CS.getDimensionsFromPlatformType!");
                return new Vector2Int(-1, -1);
        }
    }

    private Room createPlatform(int type) {
        createPosition = platformControl.getPosition();
        Vector2Int roomDimensions = getDimensionsFromPlatformType(type);
        if (lookupMap.isValidPosition(createPosition, roomDimensions)) {
            GameObject roomInstance = Instantiate(roomPrefab, createPosition, Quaternion.identity);
            Room currentRoom = roomInstance.GetComponent<Room>();

            currentRoom.setID(numRooms);
            currentRoom.setPivot(createPosition);
            currentRoom.setCellWidth(cellWidth);
            currentRoom.setRoomDimensions(roomDimensions);
            currentRoom.setDoors();
            numRooms++;

            instantiatePlatform(roomInstance, currentRoom, type);
            return currentRoom;
        }
        else return createPlatform(type);
    }

    private void instantiatePlatform(GameObject roomInstance, Room currentRoom, int type) {
        List <GameObject> createdCells = new List<GameObject>();
        switch (type) {
            case 1:
                HandleType1(createPosition, createdCells);
                break;
            case 2:
                HandleType2(createPosition, createdCells);
                break;
            case 3:
                HandleType3(createPosition, createdCells);
                break;
            default:
                Debug.Log("ERROR! PLATFORM TYPE NOT DEFINED YET.");
                break;
        }
        
        foreach (GameObject cell in createdCells) {
            cell.transform.SetParent(roomInstance.transform, true);
        }
    }

    public void debugClosed() {
        foreach(Vector2 position in platformControl.getClosedPositionsInWorldSpace()) {
            Instantiate(DebugCircle, position, Quaternion.identity);
        }
    }

    public List<Room> getListRooms() {
        return listRooms;
    }

    public LookupMap getLookupMap() {
        return lookupMap;
    }

    public Dijekstras getPathfinding() {
        return pathfinding;
    }

    private void HandleType1(Vector3 createPosition, List<GameObject> createdCells) {
        GameObject platformCellInstance = Instantiate(platformCell, createPosition, Quaternion.identity);
        createdCells.Add(platformCellInstance);
    }

    private void HandleType2(Vector3 createPosition, List<GameObject> createdCells) {
        GameObject platformCellLeftInstance = Instantiate(platformCellLeft, createPosition, Quaternion.identity);
        GameObject platformCellRightInstance = Instantiate(platformCellRight, createPosition + new Vector3(cellWidth, 0, 0), Quaternion.identity);
        createdCells.Add(platformCellLeftInstance);
        createdCells.Add(platformCellRightInstance);
    }

    private void HandleType3(Vector3 createPosition, List<GameObject> createdCells) {
        GameObject platformCellLeftInstance = Instantiate(platformCellLeft, createPosition, Quaternion.identity);
        GameObject platformCellHallwayInstance = Instantiate(platformCellHallway, createPosition + new Vector3(cellWidth, 0, 0), Quaternion.identity);
        GameObject platformCellRightInstance = Instantiate(platformCellRight, createPosition + new Vector3(cellWidth * 2, 0, 0), Quaternion.identity);
        createdCells.Add(platformCellLeftInstance);
        createdCells.Add(platformCellHallwayInstance);
        createdCells.Add(platformCellRightInstance);
    }

    public float getCellWidth() {
        return cellWidth;
    }
}
