using UnityEngine;
using System;
using System.Collections.Generic;

public class LookupMap {
    private const int MAX_MAP_LENGTH = 30;
    private const int MAX_MAP_HEIGHT = 30;

    float cellWidth;
    int[,] map; 
    public LookupMap(float cellWidth) {
        map = new int[MAX_MAP_LENGTH, MAX_MAP_HEIGHT]; 
        for (int x = 0; x < MAX_MAP_LENGTH; x++)
            for (int y = 0; y < MAX_MAP_HEIGHT; y++)
                map[x, y] = -1;
        this.cellWidth = cellWidth;
    }

    private int sizeToUnits(float num) {
        //Debug.Log("Returning " + (int) Math.Round(num / cellWidth));
        return (int) Math.Round(num / cellWidth);
    }

    public void insertRoom(Room room) {
        Vector2 roomPivot = room.getPivot();
        int xPos = sizeToUnits(roomPivot.x) + (MAX_MAP_LENGTH / 2);
        int yPos = sizeToUnits(roomPivot.y) + (MAX_MAP_HEIGHT / 2);

        Vector2 roomDimensions = room.getRoomDimensions();
        int roomLength = sizeToUnits(roomDimensions.x);
        int roomHeight = sizeToUnits(roomDimensions.y);

        for (int x = 0; x < roomLength; x++) {
            for (int y = 0; y < roomHeight; y++) {
                map[xPos + x, yPos + y] = room.getRoomID();
            }
        }
    } 

    public bool isValidPosition(Vector2 position, Vector2Int dimensions) {
        int xPos = sizeToUnits(position.x) + (MAX_MAP_LENGTH / 2);
        int yPos = sizeToUnits(position.y) + (MAX_MAP_HEIGHT / 2);
        //Debug.Log("dimensions are " + dimensions.x + " by " + dimensions.y);
 
        for (int x = 0; x < dimensions.x; x++) {
            for (int y = 0; y < dimensions.y; y++) {
                if (map[xPos + x, yPos + y] != -1) {
                    //Debug.Log("Checking if " + (xPos + x) + " by " + (yPos + y) + " is valid.");
                    return false;
                }
            }
        }
        return true;
    }

    public int getIndexAt(Vector2 position) {
        int xPos = sizeToUnits(position.x) + (MAX_MAP_LENGTH / 2);
        int yPos = sizeToUnits(position.y) + (MAX_MAP_HEIGHT / 2);

        return map[xPos, yPos];
    }

    public void debugTest() {
        //Debug.Log("The size of the map is " + map.GetLength(0) + " by " + map.GetLength(1));
        Debug.Log("At the map at 1, 1 the value is " + map[1, 1]);
    }

    public void printMap() {
        string output = "";
        for (int j = MAX_MAP_LENGTH - 1; j >= 0; j--) {
            for (int i = 0; i < MAX_MAP_HEIGHT; i++) {
                output += map[i, j].ToString().PadLeft(2, ' ') + " ";
                if ((i + 1) % 30 == 0) {
                    output += "\n";
                }
            }
        }
        Debug.Log(output);
    }


}
