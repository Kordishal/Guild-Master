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

        GameObject HomeTown = new GameObject();
        HomeTown.name = "Home Town";
        HomeTown.AddComponent<Location>();
        HomeTown.transform.SetParent(gameObject.transform);
        Location home_town = HomeTown.GetComponent<Location>();
        home_town.Name = "Tarzinai";
        home_town.Description = "The town your guild resides in. It is a frontier town of humans & orcs.";
        Places.Add(home_town);

        GameObject GuildHall = new GameObject();
        GuildHall.name = "Guild Hall Test";
        GuildHall.AddComponent<Location>();
        GuildHall.transform.SetParent(gameObject.transform);
        Location guild_hall = GuildHall.GetComponent<Location>();
        guild_hall.Name = "The Best Guild Hall";
        guild_hall.Description = "Your Home.";
        World.GuildHall = guild_hall;

        GameObject Inn = new GameObject();
        Inn.name = "Inn";
        Inn.AddComponent<Location>();
        Inn.transform.SetParent(HomeTown.gameObject.transform);
        Location inn = Inn.GetComponent<Location>();
        inn.Name = "Waywards Inn";
        inn.Description = "A living place where people from all over gather to eat, drink and exchange stories.";
        Places.Add(inn);

        GameObject Forest = new GameObject();
        Forest.name = "Forest";
        Forest.AddComponent<Location>();
        Forest.transform.SetParent(gameObject.transform);
        Location forest = Forest.GetComponent<Location>();
        forest.Name = "The Great Forest of the South";
        forest.Description = "An uncharted thick forest with many monsters and other beings living within it. Tread carefully.";
        Places.Add(forest);

        GameObject Village = new GameObject();
        Village.name = "Village";
        Village.AddComponent<Location>();
        Village.transform.SetParent(Forest.gameObject.transform);
        Location village = Village.GetComponent<Location>();
        village.Name = "Downhill";
        village.Description = "A small village in the forest. Not much happens here.";
        Places.Add(village);

        GameObject MountainRange = new GameObject();
        MountainRange.name = "Mountain Range";
        MountainRange.AddComponent<Location>();
        MountainRange.transform.SetParent(gameObject.transform);
        Location mountain_range = MountainRange.GetComponent<Location>();
        mountain_range.Name = "The Khardrathic Mountains";
        mountain_range.Description = "To the east is a massive mountain range with many very high mountains. Dangerous beasts live here, but caverns within the mountains promise great ancient treasures.";
        Places.Add(mountain_range);

        GameObject DwarvenFortress = new GameObject();
        DwarvenFortress.name = "Dwarven Fortress";
        DwarvenFortress.AddComponent<Location>();
        DwarvenFortress.transform.SetParent(MountainRange.gameObject.transform);
        Location dwarven_fortress = DwarvenFortress.GetComponent<Location>();
        dwarven_fortress.Name = "Tori'dahl Ziurak";
        dwarven_fortress.Description = "A massiv and well guarded fortress of the dwarven kingdom.";
        Places.Add(dwarven_fortress);

        GameObject EasternForest = new GameObject();
        EasternForest.name = "Eastern Forest";
        EasternForest.AddComponent<Location>();
        EasternForest.transform.SetParent(gameObject.transform);
        Location eastern_forest = EasternForest.GetComponent<Location>();
        eastern_forest.Name = "The Dark Woods";
        eastern_forest.Description = "Before the Khardrathic Mountains the Dark Woods can be found.";
        Places.Add(eastern_forest);

        GameObject AncientRuin = new GameObject();
        AncientRuin.name = "Ancient Ruins";
        AncientRuin.AddComponent<Location>();
        AncientRuin.transform.SetParent(EasternForest.gameObject.transform);
        Location ancient_ruin = AncientRuin.GetComponent<Location>();
        ancient_ruin.Name = "The Hidden Ruins of the Lanatiel";
        ancient_ruin.Description = "Deep inside of the Dark Wood an ancient ruin can be found. Legends tell that the mythical Lanatiels dwelled here. Maybe they left behind some treasure?";
        Places.Add(ancient_ruin);


        //GameObject Lake = new GameObject();
        //Lake.name = "Lake";
        //Lake.AddComponent<Location>();
        //Lake.transform.SetParent(gameObject.transform);
        //Location lake = Lake.GetComponent<Location>();
        //lake.Name = "Lake Irdath";
        //lake.Description = "A vast lake with many monsters inside. Need to be able to swim or have a ship to navigate.";
        //Places.Add(lake);

        // Home Town
        addLocation(home_town, new Dictionary<Location, int>() { { guild_hall, 10 }, { forest, 300 }, { mountain_range, 1000 } });
        addLocation(guild_hall, new Dictionary<Location, int>() { { home_town, 10 }, { inn, 15 } });
        addLocation(inn, new Dictionary<Location, int>() { { home_town, 10 }, { guild_hall, 15 } });

        // Northern Forest
        addLocation(forest, new Dictionary<Location, int>() { { home_town, 300 }, { mountain_range, 1200 }, { eastern_forest, 790 }, { village, 50 } });
        addLocation(village, new Dictionary<Location, int>() { { forest, 50 } });

        // Eastern Mountains
        addLocation(mountain_range, new Dictionary<Location, int>() { { home_town, 1000 }, { forest, 1200 }, { eastern_forest, 250 } });
        addLocation(dwarven_fortress, new Dictionary<Location, int>() { { mountain_range, 350 } });

        // Easter Forest
        addLocation(eastern_forest, new Dictionary<Location, int>() { { home_town, 550 }, { mountain_range, 250 }, { forest, 790 } });
        addLocation(ancient_ruin, new Dictionary<Location, int>() { { eastern_forest, 100 } });

        foreach (var n in WorldGraph)
            Debug.Log("World Graph: " + n.ToString());
    }
	
	// Update is called once per frame
	void Update () {
		
	}




}
