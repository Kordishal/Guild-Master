using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guild : MonoBehaviour {

    public static int CurrentWealth;

    public Calendar Calendar;
    public GameObject EventDialogBox;

    public GameObject AdventurerDisplayBox;
    public GameObject CurrentlyDisplayedAdventurer;

    public void nextAdventurer_onClick()
    {
        if (Adventurers.Count != 0)
        {
            var i = Adventurers.FindIndex(x => x.GetComponent<Adventurer>().isDisplayed);
            CurrentlyDisplayedAdventurer.GetComponent<Adventurer>().isDisplayed = false;
            if (i == Adventurers.Count - 1)            
                CurrentlyDisplayedAdventurer = Adventurers[0];          
            else 
                CurrentlyDisplayedAdventurer = Adventurers[i + 1];         
            CurrentlyDisplayedAdventurer.GetComponent<Adventurer>().isDisplayed = true;
            CurrentlyDisplayedAdventurer.GetComponent<Adventurer>().printAdventurer();
        }
    }

    public List<GameObject> Adventurers;

    public List<GameObject> Missions;

    public Text MissionDescription;
    public GameObject SelectedMission;
    public Mission getSelectedMission
    {
        get
        {
            return SelectedMission.GetComponent<Mission>();
        }
    }
    public List<GameObject> SelectedAdventurers;
    public List<Adventurer> getSelectedAdventurers
    {
        get
        {
            List<Adventurer> list = new List<Adventurer>();
            foreach (GameObject o in SelectedAdventurers)
                list.Add(o.GetComponent<Adventurer>());
            return list;
        }
    }

    public GameObject CurrentRunningMissionDisplayBox;
    public LinkedListNode<Mission> CurrentDisplayedMission;
    public LinkedList<Mission> RunningMissions;

    public Location GuildHall;

    public ScrollRect AdventurerView;
    public ScrollRect MissionView;

    public Button AdventureButtonPrefab;
    public Button MissionButtonPrefab;

    public Button StartMission;

    public Text ErrorMessages;

    private bool is_warned;
    public void onStartMissionClick()
    {
        if (SelectedMission == null)
        {
            ErrorMessages.text = "No mission selected.";
            return;
        }
        if (SelectedAdventurers.Count == 0)
        {
            ErrorMessages.text = "You need to select at least one adventurer to send on a mission.";
            return;
        }
        if (SelectedAdventurers.Count == getSelectedMission.MaxAdventurers || (SelectedAdventurers.Count < getSelectedMission.MaxAdventurers && is_warned))
        {
            ErrorMessages.text = "Mission Started";
            is_warned = false;

            // Store them forst to be able to change the values away. THen run the mission.
            Mission m = getSelectedMission;
            List<Adventurer> advs = getSelectedAdventurers;
            getSelectedMission.isAvailable = false;
            getSelectedMission.isSelected = false;
            SelectedMission = null;
            // Only select a new mission if there is more than one available.
            if (Missions.Count > 1)
            {
                foreach (GameObject o in Missions)
                {
                    if (o.GetComponent<Mission>().isAvailable)
                    {
                        o.GetComponent<Mission>().onClicked();
                        break; // Once a new mission is selected move on.
                    }
                }
            }

              
            foreach (Adventurer a in getSelectedAdventurers)
            {
                a.onClicked();
                a.isAvailable = false;
            }

            RunningMissions.AddLast(m);

            if (RunningMissions.Count == 1)
            {
                CurrentDisplayedMission = RunningMissions.First;
                m.isDisplayed = true;
            }
                

            m.startMission(advs);

            foreach (var adv in getSelectedAdventurers)
                adv.isSelected = false;

            SelectedAdventurers.Clear();
        }
        else
        {
            is_warned = true;
            ErrorMessages.text = "Are you sure you want to send just " + SelectedAdventurers.Count + " Advendtuerers? The Mission allows a group of " + getSelectedMission.MaxAdventurers;
        }
        

    }

    public void onNextMissionClick()
    {
        if (RunningMissions.Count <= 1)
            return;

        CurrentDisplayedMission.Value.isDisplayed = false;

        if (CurrentDisplayedMission == RunningMissions.Last)
            CurrentDisplayedMission = RunningMissions.First;
        else
            CurrentDisplayedMission = CurrentDisplayedMission.Next;

        CurrentDisplayedMission.Value.isDisplayed = true;
    }


	// Use this for initialization
	void Start () {
        UnityEngine.Random.InitState(10100);

        SelectedMission = null;
        SelectedAdventurers = new List<GameObject>();
        RunningMissions = new LinkedList<Mission>();

        ErrorMessages.text = "Everything is Okay";

        Calendar.dailyEventTrigger += fireDailyEvent;

        GuildHall = World.GuildHall;

        // Create a bunch of missions and adventurers to begin with.

        for (int i = 0; i < 5; i++)
        {
            Button adventurer = Instantiate(AdventureButtonPrefab) as Button;
            Button mission = Instantiate(MissionButtonPrefab) as Button;
        }
        defaultValuesRunningMissionDisplay();

        CurrentWealth = 1000;

    }
	
	// Update is called once per frame
	void Update () {

    }

    public void fireDailyEvent(object sender, EventArgs e)
    {
        GameObject game_event = Instantiate(EventDialogBox);
    }

    private void defaultValuesRunningMissionDisplay()
    {
        GameObject.Find("RunningMissionTitle").GetComponent<Text>().text = "";
        GameObject.Find("RunningMissionDescription").GetComponent<Text>().text = "No Missions Running";
        GameObject.Find("RunningMissionReward").GetComponent<Text>().text = "0";
        GameObject.Find("RunningMissionMaxAdventurers").GetComponent<Text>().text = "0";
        GameObject.Find("RunningMissionAdventurers").GetComponent<Text>().text = "None";
        GameObject.Find("RunningMissionCurrentLocation").GetComponent<Text>().text = "-------";
        GameObject.Find("RunningMissionCurrentStage").GetComponent<Text>().text = "----";
        GameObject.Find("RunningMissionDistanceNext").GetComponent<Text>().text = "0";
        GameObject.Find("RunningMissionTraveledDistance").GetComponent<Text>().text = "0";
    }
}





