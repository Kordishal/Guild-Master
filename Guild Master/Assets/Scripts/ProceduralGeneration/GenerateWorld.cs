using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWorld : MonoBehaviour {

    public GameObject WorldMapPanel;


    void Awake()
    {
        Debug.Log("Generate World");

        World w = WorldMapPanel.GetComponent<World>();
        World.Places = new List<Location>();

        GameObject HomeTown = new GameObject();
        HomeTown.name = "Home Town";
        HomeTown.AddComponent<Location>();
        HomeTown.transform.SetParent(gameObject.transform);
        Location home_town = HomeTown.GetComponent<Location>();
        home_town.Name = "Tarzinai";
        home_town.Description = "The town your guild resides in. It is a frontier town of humans & orcs.";
        World.Places.Add(home_town);

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
        World.Places.Add(inn);

        GameObject Forest = new GameObject();
        Forest.name = "Forest";
        Forest.AddComponent<Location>();
        Forest.transform.SetParent(gameObject.transform);
        Location forest = Forest.GetComponent<Location>();
        forest.Name = "The Northern Forest";
        forest.Description = "An uncharted thick forest with many monsters and other beings living within it. Tread carefully.";
        World.Places.Add(forest);

        GameObject Village = new GameObject();
        Village.name = "Village";
        Village.AddComponent<Location>();
        Village.transform.SetParent(Forest.gameObject.transform);
        Location village = Village.GetComponent<Location>();
        village.Name = "Downhill";
        village.Description = "A small village in the forest. Not much happens here.";
        World.Places.Add(village);

        GameObject MountainRange = new GameObject();
        MountainRange.name = "Mountain Range";
        MountainRange.AddComponent<Location>();
        MountainRange.transform.SetParent(gameObject.transform);
        Location mountain_range = MountainRange.GetComponent<Location>();
        mountain_range.Name = "The Khardrathic Mountains";
        mountain_range.Description = "To the east is a massive mountain range with many very high mountains. Dangerous beasts live here, but caverns within the mountains promise great ancient treasures.";
        World.Places.Add(mountain_range);

        GameObject DwarvenFortress = new GameObject();
        DwarvenFortress.name = "Dwarven Fortress";
        DwarvenFortress.AddComponent<Location>();
        DwarvenFortress.transform.SetParent(MountainRange.gameObject.transform);
        Location dwarven_fortress = DwarvenFortress.GetComponent<Location>();
        dwarven_fortress.Name = "Tori'dahl Ziurak";
        dwarven_fortress.Description = "A massiv and well guarded fortress of the dwarven kingdom.";
        World.Places.Add(dwarven_fortress);

        GameObject EasternForest = new GameObject();
        EasternForest.name = "Eastern Forest";
        EasternForest.AddComponent<Location>();
        EasternForest.transform.SetParent(gameObject.transform);
        Location eastern_forest = EasternForest.GetComponent<Location>();
        eastern_forest.Name = "The Dark Woods";
        eastern_forest.Description = "Before the Khardrathic Mountains the Dark Woods can be found.";
        World.Places.Add(eastern_forest);

        GameObject AncientRuin = new GameObject();
        AncientRuin.name = "Ancient Ruins";
        AncientRuin.AddComponent<Location>();
        AncientRuin.transform.SetParent(EasternForest.gameObject.transform);
        Location ancient_ruin = AncientRuin.GetComponent<Location>();
        ancient_ruin.Name = "The Hidden Ruins of the Lanatiel";
        ancient_ruin.Description = "Deep inside of the Dark Wood an ancient ruin can be found. Legends tell that the mythical Lanatiels dwelled here. Maybe they left behind some treasure?";
        World.Places.Add(ancient_ruin);


        //GameObject Lake = new GameObject();
        //Lake.name = "Lake";
        //Lake.AddComponent<Location>();
        //Lake.transform.SetParent(gameObject.transform);
        //Location lake = Lake.GetComponent<Location>();
        //lake.Name = "Lake Irdath";
        //lake.Description = "A vast lake with many monsters inside. Need to be able to swim or have a ship to navigate.";
        //Places.Add(lake);

        // Home Town
        w.addLocation(home_town, new Dictionary<Location, int>() { { guild_hall, 10 }, { forest, 300 }, { mountain_range, 1000 }, { inn, 10 }, { eastern_forest, 550 } });
        w.addLocation(guild_hall, new Dictionary<Location, int>() { { home_town, 10 }, { inn, 15 } });
        w.addLocation(inn, new Dictionary<Location, int>() { { home_town, 10 }, { guild_hall, 15 } });

        // Northern Forest
        w.addLocation(forest, new Dictionary<Location, int>() { { home_town, 300 }, { mountain_range, 1200 }, { eastern_forest, 790 }, { village, 50 } });
        w.addLocation(village, new Dictionary<Location, int>() { { forest, 50 } });

        // Eastern Mountains
        w.addLocation(mountain_range, new Dictionary<Location, int>() { { home_town, 1000 }, { forest, 1200 }, { eastern_forest, 250 }, { dwarven_fortress, 350 } });
        w.addLocation(dwarven_fortress, new Dictionary<Location, int>() { { mountain_range, 350 } });

        // Eastern Forest
        w.addLocation(eastern_forest, new Dictionary<Location, int>() { { home_town, 550 }, { mountain_range, 250 }, { forest, 790 }, { ancient_ruin, 100 } });
        w.addLocation(ancient_ruin, new Dictionary<Location, int>() { { eastern_forest, 100 } });

        World.GuildHall.Adventurers = new List<Adventurer>();

        foreach (Location l in World.Places)
            l.Adventurers = new List<Adventurer>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
