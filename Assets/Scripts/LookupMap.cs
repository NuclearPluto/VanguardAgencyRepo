using UnityEngine;
using System;
using System.Collections.Generic;

public class LookupMap {

    private List<Room> roomList;
    //min x, min y, max x, max y
    float[] minMax;
    float cellWidth;
    int[,] map; 
    public LookupMap(List<Room> roomList, float[] minMax) {
        this.roomList = roomList;
        this.minMax = minMax;
        this.cellWidth = roomList[0].getCellWidth();
        float xLength = minMax[2] - minMax[0];
        float yLength = minMax[3] - minMax[1];
        int xSize = (int)Math.Round((xLength/cellWidth));
        int ySize = (int)Math.Round((yLength/cellWidth));
        map = new int[xSize, ySize]; 
        for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y++)
                map[x, y] = -1;
        initializeMap(xSize, ySize);
        debugTest();
    }

    private void initializeMap(int xSize, int ySize) {
        Debug.Log("Created array is at size " + xSize + " by " + ySize);
        for (int index = 0; index < roomList.Count; index++) {
            Vector2 pivot = roomList[index].getPivot();
            Room room = roomList[index]; 

            int leftX = (int)Math.Round((Math.Abs(minMax[0]) + room.getCellWidth() / 2) / room.getCellWidth()) - 1;
            int leftY = (int)Math.Round((Math.Abs(minMax[1]) + room.getCellWidth() / 2) / room.getCellWidth()) - 1;

            int xIndice = (int)Math.Round(pivot.x / room.getCellWidth()) + leftX;
            int yIndice = (int)Math.Round(pivot.y / room.getCellWidth()) + leftY;
            int numXIters = (int)(Math.Round(room.getRoomDimensions().x / room.getCellWidth()));
            Debug.Log("xIndice is " + xIndice + " yIndice is " + yIndice);
            Debug.Log("At pivot " + room.getPivot() + " because size is " + numXIters);
            for (int i = 0; i < numXIters; i++) {
                //Debug.Log("inserting " + index + " at map[" + xIndice + i + ", " + yIndice + "]");
                map[xIndice + i, yIndice] = index;
            }
        }
    }

    public void debugTest() {
        //Debug.Log("The size of the map is " + map.GetLength(0) + " by " + map.GetLength(1));
        Debug.Log("At the map at 1, 1 the value is " + map[1, 1]);
    }
}
