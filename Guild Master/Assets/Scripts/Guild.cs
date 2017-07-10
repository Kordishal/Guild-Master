using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guild : MonoBehaviour {

    private int tick = 0;

    public Player GuildMaster;

    public List<GameObject> Adventurers;
    public List<GameObject> Missions;

    private GameObject selected_mission;
    private Mission getSelectedMission
    {
        get
        {
            return selected_mission.GetComponent<Mission>();
        }
    }
    private List<GameObject> selected_adventurers;
    private List<Adventurer> getSelectedAdventurers
    {
        get
        {
            List<Adventurer> list = new List<Adventurer>();
            foreach (GameObject o in selected_adventurers)
                list.Add(o.GetComponent<Adventurer>());
            return list;
        }
    }

    public ScrollRect AdventurerView;
    public GameObject AdventurerPrefab;
    public Button AdventureButtonPrefab;

    public ScrollRect MissionView;
    public GameObject MissionPrefab;
    public Button MissionButtonPrefab;

    public Button StartMission;

    public Text ErrorMessages;

    private bool is_warned;
    private void onStartMissionClick()
    {

        if (selected_adventurers.Count == getSelectedMission.MaxAdventurers || (selected_adventurers.Count < getSelectedMission.MaxAdventurers && is_warned))
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
            ErrorMessages.text = "Are you sure you want to send just " + selected_adventurers.Count + " Advendtuerers? The Mission allows a group of " + getSelectedMission.MaxAdventurers;
        }
        

    }

	// Use this for initialization
	void Start () {
        StartMission.onClick.AddListener(onStartMissionClick);

        StartMission.enabled = false;
        selected_mission = null;
        selected_adventurers = null;

        selected_adventurers = new List<GameObject>();

        ErrorMessages.text = "Everything is Okay";
	}
	
	// Update is called once per frame
	void Update () {
        tick += 1;

        if (tick % 100 == 0 && Adventurers.Count < 10)
        {
            createNewAdventurer();
            createNewMission();
        }

        if (selected_mission != null & selected_adventurers.Count > 0)
            StartMission.enabled = true;
	}


    private void createNewAdventurer()
    {
        GameObject adv_game_object = Instantiate(AdventurerPrefab) as GameObject;
        Adventurers.Add(adv_game_object);

        Adventurer adventurer = adv_game_object.GetComponent<Adventurer>();
        adventurer.AdventurerButton = Instantiate(AdventureButtonPrefab) as Button;
        adventurer.NameLabel = adventurer.AdventurerButton.transform.GetChild(0).gameObject.GetComponent("Text") as Text;
        adventurer.LevelLabel = adventurer.AdventurerButton.transform.GetChild(1).gameObject.GetComponent("Text") as Text;
        adventurer.CostLabel = adventurer.AdventurerButton.transform.GetChild(2).gameObject.GetComponent("Text") as Text;

        adventurer.AdventurerButton.transform.SetParent(AdventurerView.transform.GetChild(0).GetChild(0));
        RectTransform butten_rect = adventurer.AdventurerButton.GetComponent<RectTransform>();
        butten_rect.anchoredPosition = new Vector2(0, 30 + (-60 * Adventurers.Count));
        butten_rect.offsetMax = new Vector2(0, butten_rect.offsetMax.y);
        butten_rect.offsetMin = new Vector2(0, butten_rect.offsetMin.y);

        // Increases the space used by the content panel of the viewport. This is necessary as otherwise the viewport will not detect when the buttons will overflow its view.
        RectTransform content_rect = AdventurerView.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        content_rect.offsetMin = new Vector2(content_rect.offsetMin.x, content_rect.offsetMin.y - 60);

        adventurer.OnSelected += onAdventurerSelected;
    }

    public void onAdventurerSelected(object sender, EventArgs e)
    {
        Adventurer adventurer = (Adventurer)sender;

        // When the adventurer is already selected always deselect them.
        if (adventurer.isSelected)
        {
            selected_adventurers.Remove(adventurer.gameObject);
            adventurer.isSelected = false;
        }
        else if (getSelectedMission.MaxAdventurers > selected_adventurers.Count)
        {
            // when there is still space make the adventurer part of the party.
            selected_adventurers.Add(adventurer.gameObject);
            adventurer.isSelected = true;   
        }
        // Otherwise do nothing.
    }

    private void createNewMission()
    {
        GameObject mission_gameobject = Instantiate(MissionPrefab) as GameObject;
        Missions.Add(mission_gameobject);
        Button mission_button = Instantiate(MissionButtonPrefab) as Button;
        Mission mission = mission_gameobject.GetComponent<Mission>();
        mission.MissionButton = mission_button;
        mission.NameLabel = mission_button.transform.GetChild(0).gameObject.GetComponent("Text") as Text;
        mission.RewardLabel = mission_button.transform.GetChild(1).gameObject.GetComponent("Text") as Text;

        mission_button.transform.SetParent(MissionView.transform.GetChild(0).GetChild(0));
        RectTransform mission_rect = mission_button.GetComponent<RectTransform>();
        mission_rect.anchoredPosition = new Vector2(0, 30 + (-60 * Missions.Count));
        mission_rect.offsetMax = new Vector2(0, mission_rect.offsetMax.y);
        mission_rect.offsetMin = new Vector2(0, mission_rect.offsetMin.y);

        RectTransform content_rect = MissionView.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        content_rect.offsetMin = new Vector2(content_rect.offsetMin.x, content_rect.offsetMin.y - 60);

        mission.OnSelected += onMissionSelected;            
    }

    public void onMissionSelected(object sender, EventArgs e)
    {
        // Whenever a new mission is selected, deselect the old one as there can always only be one mission selected.
        Mission mission = (Mission)sender;
        if (!mission.isSelected)
        {
            foreach (GameObject o in Missions)
                if (o.GetComponent<Mission>().isSelected)
                    o.GetComponent<Mission>().isSelected = false;

            mission.isSelected = true;
            selected_mission = mission.gameObject;
        }
    }
}
