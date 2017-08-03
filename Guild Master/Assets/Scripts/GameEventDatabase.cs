using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventDatabase : MonoBehaviour {


    static public List<GameEventImplementation> GAME_EVENTS;

    public Guild GuildStats;

	// Use this for initialization
	void Start () {
        GAME_EVENTS = new List<GameEventImplementation>();

        GAME_EVENTS.Add(new GameEventImplementation("New Mission", "Images/Events/default_event_picture.png", 
            "You have recieved a new mission!", EventType.Mission, EventFrequencyType.Daily, 100, addNewMission));

        GAME_EVENTS.Add(new GameEventImplementation("New Adventurer", "Images/Events/default_event_picture.png", 
            "A new adventurer has joined your guild!", EventType.Adventurer, EventFrequencyType.Daily, 100, addNewAdventurer));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public class GameEventImplementation
    {
        public string EventTitle;
        public string EventImagePath;
        public string EventDescription;

        public EventType Type;
        public EventFrequencyType FrequencyType;
        public int Weight;

        public UnityAction Effect;

        public GameEventImplementation(string title, string image_path, string description, EventType type, EventFrequencyType frequency, int weight, UnityAction effect)
        {
            EventTitle = title;
            EventImagePath = image_path;
            EventDescription = description;
            Type = type;
            FrequencyType = frequency;
            Weight = weight;
            Effect = effect;
        }
    }

    public void addNewMission()
    {
        GuildStats.createNewMission();
    }

    public void addNewAdventurer()
    {
        GuildStats.createNewAdventurer();
    }

    public enum EventType
    {
        Adventurer,
        Mission,
    }

    public enum EventFrequencyType
    {
        Daily,
        Weekly,
        Monthly,
        Yearly,
    }

}
