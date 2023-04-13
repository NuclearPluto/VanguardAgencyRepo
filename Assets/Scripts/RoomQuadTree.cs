using UnityEngine;
using System;
using System.Collections.Generic;

public class QuadTree {
    private class Quadrant {
        public List<Room> localRooms;
        public bool isLeaf = true;
        float[] localDimensions;
        Quadrant[] quadrants = new Quadrant[4];

        public Quadrant() {

        }

        public bool isRoomInQuad(Room room) {
            float[] roomDimensions = room.getWorldDimensions();

            if ((roomDimensions[2] - roomDimensions[0]) > (localDimensions[2] - localDimensions[0]))
                Debug.Log("ERROR, ROOM X LENGTH IS BIGGER THAN QUAD, RETHINK ALGORITHM");
            if ((roomDimensions[3] - roomDimensions[1]) > (localDimensions[3] - localDimensions[1]))
                Debug.Log("ERROR, ROOM Y LENGTH IS BIGGER THAN QUAD, RETHINK ALGORITHM");

            //If any of the 4 corners of the room are within quadrant
            if (roomDimensions[0] > localDimensions[0] && roomDimensions[0] < localDimensions[2]) 
                if (roomDimensions[1] > localDimensions[1] && roomDimensions[1] < localDimensions[3])
                    return true;
            if (roomDimensions[0] > localDimensions[0] && roomDimensions[0] < localDimensions[2]) 
                if (roomDimensions[3] > localDimensions[1] && roomDimensions[3] < localDimensions[3])
                    return true;
            if (roomDimensions[2] > localDimensions[0] && roomDimensions[2] < localDimensions[2]) 
                if (roomDimensions[1] > localDimensions[1] && roomDimensions[1] < localDimensions[3])
                    return true;
            if (roomDimensions[2] > localDimensions[0] && roomDimensions[2] < localDimensions[2]) 
                if (roomDimensions[3] > localDimensions[1] && roomDimensions[3] < localDimensions[3])
                    return true;

            return false;
        }
    }

    private List<Room> roomList;
    //0,      1,      2,     3
    //min x, min y, max x, max y
    float[] minMax;
    int numSplit;
    public QuadTree(List<Room> roomList, float[] minMax) {
        this.roomList = roomList;
        this.minMax = minMax;
        numSplit = (int)Math.Pow(roomList.Count, 1.0/4.0);
    }
}

