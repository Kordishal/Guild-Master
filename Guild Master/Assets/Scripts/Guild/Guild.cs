using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guild : MonoBehaviour {

    public static int CurrentWealth;

    public Calendar Calendar;
    public GameObject EventDialogBox;

    private List<GameObject> adventurers;
    public List<GameObject> Adventurers { get { return adventurers; } }
    public void addAdventurer(GameObject adventurer)
    {
        adventurers.Add(adventurer);
        fire_change_adventurer_amount(new EventArgs());
    }
    public void addAdventurers(List<GameObject> adventurers_list)
    {
        adventurers = adventurers_list;
        fire_change_adventurer_amount(new EventArgs());
    }
    public void removeAdventurer(GameObject adventurer)
    {
        adventurers.Remove(adventurer);
        fire_change_adventurer_amount(new EventArgs());
    }


    private List<GameObject> missions;
    public List<GameObject> Missions
    {
        get { return missions; }
    }
    public void addMission(GameObject mission)
    {
        missions.Add(mission);
        fire_change_mission_amount(new EventArgs());
    }
    public void addMissions(List<GameObject> missions_list)
    {
        missions = missions_list;
        fire_change_mission_amount(new EventArgs());
    }
    public void removeMission(GameObject mission)
    {
        missions.Remove(mission);
        fire_change_mission_amount(new EventArgs());
    }


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

    public LinkedList<Mission> RunningMissions;


    // Move 
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
            // Only select a new mission if there is more than one available. As if there is only one, it is the mission that was just started, but is still in the list.
            if (Missions.Count > 1)
            {
                foreach (GameObject available_missions in Missions)
                {
                    if (available_missions == null)
                        continue;

                    if (available_missions.GetComponent<Mission>().isAvailable)
                    {
                        available_missions.GetComponent<Mission>().onClicked();
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

	// Use this for initialization
	void Start () {
        UnityEngine.Random.InitState(10100);

        SelectedMission = null;
        SelectedAdventurers = new List<GameObject>();
        RunningMissions = new LinkedList<Mission>();

        ErrorMessages.text = "Everything is Okay";

        Calendar.dailyEventTrigger += fireDailyEvent;
        CurrentWealth = 1000;
    }
	
	// Update is called once per frame
	void Update () {

    }

    // TODO: Move someplace else where it makes more sense...
    public void fireDailyEvent(object sender, EventArgs e)
    {
        GameObject game_event = Instantiate(EventDialogBox);
    }


    public event EventHandler<EventArgs> change_adventurer_amount;
    protected virtual void fire_change_adventurer_amount(EventArgs e)
    {
        EventHandler<EventArgs> handler = change_adventurer_amount;
        if (handler != null)
            handler(this, e);

    }

    public event EventHandler<EventArgs> change_mission_amount;
    protected virtual void fire_change_mission_amount(EventArgs e)
    {
        EventHandler<EventArgs> handler = change_mission_amount;
        if (handler != null)
            handler(this, e);

    }
}





