using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guild : MonoBehaviour {

    public Player GuildMaster;

    public Calendar Calendar;
    public GameObject EventDialogBox;

    public List<GameObject> Adventurers;
    public List<GameObject> Missions;


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

    public ScrollRect AdventurerView;
    public ScrollRect MissionView;

    public Button StartMission;

    public Text ErrorMessages;

    private bool is_warned;
    private void onStartMissionClick()
    {
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
            foreach (GameObject o in Missions)
            {
                if (o.GetComponent<Mission>().isAvailable)
                {
                    o.GetComponent<Mission>().onClicked();
                    continue; // Once a new mission is selected move on.
                }                
            }               
            foreach (Adventurer a in getSelectedAdventurers)
            {
                a.onClicked();
                a.isAvailable = false;
            }

            m.runMission(this, advs);
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

        StartMission.onClick.AddListener(onStartMissionClick);

        StartMission.enabled = false;
        SelectedMission = null;
        SelectedAdventurers = null;

        SelectedAdventurers = new List<GameObject>();

        ErrorMessages.text = "Everything is Okay";

        Calendar.dailyEventTrigger += fireDailyEvent;
	}
	
	// Update is called once per frame
	void Update () {
        if (SelectedMission != null & SelectedAdventurers.Count > 0)
            StartMission.enabled = true;
	}

    public void fireDailyEvent(object sender, EventArgs e)
    {
        Debug.Log("Fire Event");
        GameObject game_event = Instantiate(EventDialogBox);
    }




}
