using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlatformConnectivity;

public class StageGeneration : MonoBehaviour
{
    public GameObject platformCell;
    public GameObject platformCellHallway;
    public GameObject platformCellLeft;
    public GameObject platformCellRight;
    public GameObject DebugCircle;
    //stageSize - 1 = small - 2 = medium - 3 = large
    public int stageSize = 3;

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
        debugClosed();
    }

    void Update() {
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
            createPlatform(platformToCreate);
        }
    }

    public void createPlatform(int type) {
        bool dontUpdateConnectivity = false;
        switch (type) {
            case 1:
                createPosition = platformControl.getPosition();
                create1x1();
                break;
            case 2:
                createPosition = platformControl.getPosition();
                if (platformControl.isRightClosed(createPosition)) {
                    if (platformControl.isLeftClosed(createPosition)) {
                        createPlatform(type);
                        dontUpdateConnectivity = true;
                    }
                    else {
                        createPosition += new Vector3(-cellWidth, 0, 0);
                        create1x2();
                    }
                }
                else create1x2();
                break;
            case 3:
                createPosition = platformControl.getPosition();
                if (platformControl.isRightClosed(createPosition)) {
                    if (platformControl.isLeftClosed(createPosition) || platformControl.isLeftClosed(createPosition + new Vector3(-cellWidth, 0, 0))) {
                        createPlatform(type);
                        dontUpdateConnectivity = true;
                    }
                    else {
                        createPosition += new Vector3(-cellWidth*2, 0, 0);
                        create1x3();
                    }
                }
                else if (platformControl.isRightClosed(createPosition + new Vector3(cellWidth, 0, 0))) {
                    createPlatform(type);
                    dontUpdateConnectivity = true;
                }
                else create1x3();
                break;
        }
        if (!dontUpdateConnectivity) {
            platformControl.updateConnectivity(createPosition, type);
        } else dontUpdateConnectivity = false;
    }

    //1 x 1 platform
    public void create1x1() {
        GameObject test = Instantiate(platformCell, createPosition, Quaternion.identity);
    }

    public void create1x2() {
        Instantiate(platformCellLeft, createPosition, Quaternion.identity);
        Instantiate(platformCellRight, createPosition + new Vector3(cellWidth, 0, 0), Quaternion.identity);
    }

    public void create1x3() {
        Instantiate(platformCellLeft, createPosition, Quaternion.identity);
        Instantiate(platformCellHallway, createPosition + new Vector3(cellWidth, 0, 0), Quaternion.identity);
        Instantiate(platformCellRight, createPosition + new Vector3(cellWidth*2, 0, 0), Quaternion.identity);
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
}
