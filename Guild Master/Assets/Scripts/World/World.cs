using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The physical world in which the guild is situated. The locations within this world are 
/// stored in a graph. This graph is made up as a dictionary within a dictionary. For each location 
/// it returns a dictionary of its neighbours with a distance.
/// </summary>
public class World : MonoBehaviour {


    /// <summary>
    /// The guild hall is the home of the guild. It works as a base of operation. All adventurers start from here to go to missions.
    /// TODO: Expand the function of the guild hall.
    /// </summary>
    public static Location GuildHall;

    /// <summary>
    /// A list of all the locations in the world excluding the guild hall. This list is used to find random locations for mission.
    /// TODO: Further refine this list so that the type of location can be chosen more carefully.
    /// </summary>
    public static List<Location> Places;

    /// <summary>
    /// The navigation mesh for the adventurers to find their way around the world.
    /// </summary>
    private static Dictionary<Location, Dictionary<Location, int>> WorldGraph;


    /// <summary>
    /// When adding a new location always use this function to ensure that everything is added properly. 
    /// Still need to add all locations seperatly with all neighbours.
    /// </summary>
    public void addLocation(Location location, Dictionary<Location, int> neighbour_locations)
    {
        if (WorldGraph == null)
            WorldGraph = new Dictionary<Location, Dictionary<Location, int>>();

        WorldGraph[location] = neighbour_locations;
    }

    /// <summary>
    /// Uses dijkstras algorithm to find the shortest path from start to destination. Uses the distances as weights.
    /// Returns the shortest path from start to destination including the start.
    /// </summary>
    static public LinkedList<Location> findShortestPath(Location start, Location destination)
    {
        var previous = new Dictionary<Location, Location>();
        var distances = new Dictionary<Location, int>();
        var nodes = new List<Location>();

        List<Location> path = null;

        foreach (var location in WorldGraph)
        {
            if (location.Key == start)
                distances[location.Key] = 0;
            else
                distances[location.Key] = int.MaxValue;

            nodes.Add(location.Key);
        }

        while (nodes.Count != 0)
        {
            nodes.Sort((x, y) => distances[x] - distances[y]);

            var smallest = nodes[0];
            nodes.Remove(smallest);

            // Once the smallest path has reached the destination backtrack and add them to the path.
            if (smallest == destination)
            {
                path = new List<Location>();
                while (previous.ContainsKey(smallest))
                {
                    path.Add(smallest);
                    smallest = previous[smallest];
                }
                break;
            }

            // When the distance of two edges is bigger than the max value interupt the algorithm.
            if (distances[smallest] == int.MaxValue)
                break;

            // find the next shortest path
            foreach (var neighbour in WorldGraph[smallest])
            {
                var alt = distances[smallest] + neighbour.Value;
                if (alt < distances[neighbour.Key])
                {
                    distances[neighbour.Key] = alt;
                    previous[neighbour.Key] = smallest;
                }
            }
        }



        // Reverse the path as it goes from last to first and insert the beginning of 
        // the path as it is not included by the algorithm but needed for the game
        path.Reverse();
        var result = new LinkedList<Location>(path);
        result.AddFirst(start);
        result.AddLast(destination);
        
        Debug.Log("Start: " + start + " -> Destination: " + destination);
        Debug.Log(printPath(result));
        return result;
    }

    
    /// <summary>
    /// Simple lookup inside the graph to get a distance.
    /// </summary>
    public static int getDistance(Location start, Location destination)
    {
        return WorldGraph[start][destination];
    }


    /// <summary>
    /// Calculates the total distance of a path.
    /// </summary>
    public static int totalDistanceOfPath(LinkedList<Location> path)
    {
        int result = 0;
        var current = path.First;

        while (current != path.Last)
        {
            result += WorldGraph[current.Value][current.Next.Value];
            current = current.Next;
        }

        return result;
    }

    public static List<Location> getCloseNeighbours(Location current, int max_distance)
    {
        var temp = new List<Location>();

        foreach (var location in WorldGraph[current])
        {
            if (location.Value <= max_distance)
                temp.Add(location.Key);
        }

        return temp;
    }

    void Start () {
        gameObject.SetActive(!gameObject.activeSelf);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static string printPath(LinkedList<Location> path)
    {
        string result = "";
        var current = path.First;
        do
        {
            result += current.Value.Name + " -> ";
            current = current.Next;

        } while (current != path.Last);

        return result;
    }


}
