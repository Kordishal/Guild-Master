using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameEventDatabase : MonoBehaviour {

    static public List<GameEventImplementation> GAME_EVENTS;

    public Button AdventureButtonPrefab;
    public Button MissionButtonPrefab;

    public GameObject EventDialogBoxPrefab;

    public Guild Guild;

	// Use this for initialization
	void Start () {
        GAME_EVENTS = new List<GameEventImplementation>();

        GAME_EVENTS.Add(new GameEventImplementation("New Mission", "Images/Events/default_event_picture.png", 
            "You have recieved a new mission!", EventType.Mission, EventFrequencyType.Daily, 50, addNewMission, nothing));

        GAME_EVENTS.Add(new GameEventImplementation("New Adventurer", "Images/Events/default_event_picture.png", 
            "A new adventurer has joined your guild!", EventType.Adventurer, EventFrequencyType.Daily, 50, addNewAdventurer, nothing));
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

        public UnityAction ImmediateEffect;
        public UnityAction AfterEffect;

        public GameEventImplementation(string title, string image_path, string description, EventType type, EventFrequencyType frequency, int weight, UnityAction immediate_effect, UnityAction after_effect)
        {
            EventTitle = title;
            EventImagePath = image_path;
            EventDescription = description;
            Type = type;
            FrequencyType = frequency;
            Weight = weight;
            ImmediateEffect = immediate_effect;
            AfterEffect = after_effect;
        }
    }
    // used when noting happens.
    public void nothing() { }

    public void addNewMission()
    {
        Button mission = Instantiate(MissionButtonPrefab) as Button;
    }
    public void addNewAdventurer()
    {
        Button adventurer = Instantiate(AdventureButtonPrefab) as Button;
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
