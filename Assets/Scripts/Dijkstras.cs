using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dijekstras
{
    Room currentNode;
    LookupMap lookupMap;
    List<Room> listRooms;
    Dictionary<Room, Dictionary<Room, Room>> shortestPaths;

    public Dijekstras(LookupMap lookupMap, List<Room> listRooms)
    {
        this.lookupMap = lookupMap;
        this.listRooms = listRooms;

        shortestPaths = new Dictionary<Room, Dictionary<Room, Room>>();

        foreach (Room startingRoom in listRooms)
        {
            Dictionary<Room, Room> nextRoom = new Dictionary<Room, Room>();
            Dictionary<Room, float> distances = new Dictionary<Room, float>();
            HashSet<Room> visitedRooms = new HashSet<Room>();
            List<Room> openList = new List<Room>();

            foreach (Room room in listRooms)
            {
                distances[room] = Mathf.Infinity;
                openList.Add(room);
            }

            distances[startingRoom] = 0;

            while (openList.Count > 0)
            {
                // Select the room with the smallest distance from the open list
                Room currentRoom = null;
                float minDistance = Mathf.Infinity;
                foreach (Room room in openList)
                {
                    if (distances[room] < minDistance)
                    {
                        minDistance = distances[room];
                        currentRoom = room;
                    }
                }

                openList.Remove(currentRoom);
                visitedRooms.Add(currentRoom);

                List<Room> neighbors = currentRoom.getConnectedRooms();
                foreach (Room neighbor in neighbors)
                {
                    if (!visitedRooms.Contains(neighbor))
                    {
                        // Calculate the tentative distance for the neighbor
                        float edgeWeight = currentRoom.getEdgeWeight(neighbor);
                        float tentativeDistance = distances[currentRoom] + edgeWeight;

                        // Update the distance and next room if the tentative distance is smaller
                        if (tentativeDistance < distances[neighbor])
                        {
                            distances[neighbor] = tentativeDistance;
                            nextRoom[neighbor] = currentRoom;
                        }
                    }
                }
            }

            // Store the next room Hashtable for the starting room
            shortestPaths[startingRoom] = nextRoom;
        }

        currentNode = listRooms[0];
    }

    public List<Room> GetShortestPath(Vector2 startRoomPosition, Vector2 endRoomPosition)
    {
        List<Room> path = new List<Room>();
        int startIndex = lookupMap.getIndexAt(startRoomPosition);
        int endIndex = lookupMap.getIndexAt(endRoomPosition);
        if (startIndex == -1 || endIndex == -1) {
            return path;
        }

        Room startRoom = listRooms[startIndex];
        Room endRoom = listRooms[endIndex];;

        // Check if the startRoom and endRoom are in the shortestPaths dictionary
        if (shortestPaths.ContainsKey(startRoom) && shortestPaths[startRoom].ContainsKey(endRoom))
        {
            Room currentRoom = endRoom;

            // Trace the path from endRoom to startRoom using the nextRoom dictionary
            while (currentRoom != startRoom)
            {
                path.Add(currentRoom);
                currentRoom = shortestPaths[startRoom][currentRoom];
            }

            path.Add(startRoom);
            path.Reverse();
        }
        else
        {
            //Debug.LogError("GetShortestPath: Start or end room not found in the shortestPaths dictionary.");
        }

        return path;
    }


}
