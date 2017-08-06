using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    static public Location GuildHall;
    static public List<Location> Places;

    static public Dictionary<Location, Dictionary<Location, int>> WorldGraph;

    public void addLocation(Location location, Dictionary<Location, int> neighbour_locations)
    {
        WorldGraph[location] = neighbour_locations;
    }

    static public List<Location> findShortestPath(Location start, Location destination)
    {
        Debug.Log("Find path from " + start.Name + " to " + destination.Name);

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
        path.Insert(0, GuildHall);
        return path;
    }

    static public int Distance(Location start, Location destination)
    {
        return WorldGraph[start][destination];
    }

    static public int totalDistanceOfPath(LinkedList<Location> path)
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

	// Use this for initialization
	void Start () {
        WorldGraph = new Dictionary<Location, Dictionary<Location, int>>();
        Places = new List<Location>();

        GameObject GuildHall = new GameObject();
        GuildHall.name = "Guild Hall Test";
        GuildHall.AddComponent<Location>();
        GuildHall.transform.SetParent(gameObject.transform);
        Location guild_hall = GuildHall.GetComponent<Location>();
        guild_hall.Name = "The Best Guild Hall";
        guild_hall.Description = "Your Home.";
        World.GuildHall = guild_hall;

        GameObject HomeTown = new GameObject();
        HomeTown.name = "Home Town";
        HomeTown.AddComponent<Location>();
        HomeTown.transform.SetParent(gameObject.transform);
        Location home_town = HomeTown.GetComponent<Location>();
        home_town.Name = "Tarzinai";
        home_town.Description = "The town your guild resides in. It is a frontier town of humans & orcs.";
        Places.Add(home_town);

        GameObject Forest = new GameObject();
        Forest.name = "Forest";
        Forest.AddComponent<Location>();
        Forest.transform.SetParent(gameObject.transform);
        Location forest = Forest.GetComponent<Location>();
        forest.Name = "The Great Forest of the South";
        forest.Description = "An uncharted thick forest with many monsters and other beings living within it. Tread carefully.";
        Places.Add(forest);

        GameObject MountainRange = new GameObject();
        MountainRange.name = "Mountain Range";
        MountainRange.AddComponent<Location>();
        MountainRange.transform.SetParent(gameObject.transform);
        Location mountain_range = MountainRange.GetComponent<Location>();
        mountain_range.Name = "The Khardrathic Mountains";
        mountain_range.Description = "To the east is a massive mountain range with many very high mountains. Dangerous beasts live here, but caverns within the mountains promise great ancient treasures.";
        Places.Add(mountain_range);

        addLocation(guild_hall, new Dictionary<Location, int>() { { home_town, 1 } });
        addLocation(home_town, new Dictionary<Location, int>() { { guild_hall, 1 }, { forest, 3 }, { mountain_range, 10 } });
        addLocation(forest, new Dictionary<Location, int>() { { home_town, 3 }, { mountain_range, 8 } });
        addLocation(mountain_range, new Dictionary<Location, int>() { { home_town, 10 }, { forest, 8 } });

        foreach (var n in WorldGraph)
            Debug.Log("World Graph: " + n.ToString());
    }
	
	// Update is called once per frame
	void Update () {
		
	}




}
