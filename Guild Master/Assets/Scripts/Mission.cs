using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour {

    private static int ID = 0;
    public string Name;
    public double Reward;
    public string Description;

    public int Duration;
    private int elapsed_time;

    public int MaxAdventurers;
    private List<Adventurer> adventurers_running_the_mission;

    private Guild guild;


    public bool isSelected;

    public bool isAvailable;
    public bool isRunning;
    public bool isFinished;

    public void startMission(List<Adventurer> adventurers)
    {
        isRunning = true;
        adventurers_running_the_mission = new List<Adventurer>(adventurers);
        foreach (Adventurer a in adventurers_running_the_mission)
            a.isAvailable = false;

        guild.Calendar.hourlyTrigger += runningMission;
    }

    private void runningMission(object sender, EventArgs e)
    {




        if (isRunning)
        {
            elapsed_time += 1;
            if (elapsed_time >= Duration)
                endMission();
        }
    }

    public void endMission()
    {
        // decrease reward by the cost of the adventurers.
        double full_reward = Reward;
        foreach (Adventurer a in adventurers_running_the_mission)
        {
            Reward = Reward - (full_reward * a.Cost);
            a.isAvailable = true;
        }


        guild.GuildMaster.Currency += (int)Reward;

        isRunning = false;
        isFinished = true;
        removeMission();
    }

    private void removeMission()
    {
        guild.Missions.Remove(gameObject);
        MissionButton.enabled = false;
        guild.Calendar.hourlyTrigger -= runningMission;

        // Resort the mission view to fill the content up from the top again.
        int count = 1;
        foreach (GameObject mission in guild.Missions)
        {
            mission.GetComponent<Button>().transform.SetParent(guild.MissionView.transform.GetChild(0).GetChild(0));
            RectTransform mission_rect = mission.GetComponent<Button>().GetComponent<RectTransform>();
            mission_rect.anchoredPosition = new Vector2(0, 30 + (-60 * count));
            mission_rect.offsetMax = new Vector2(0, mission_rect.offsetMax.y);
            mission_rect.offsetMin = new Vector2(0, mission_rect.offsetMin.y);
            count += 1;
        } 

        Destroy(gameObject);
    }

    public Text NameLabel;
    public Text RewardLabel;
    public Button MissionButton;


    // Use this for initialization
    void Start () {
        guild = GameObject.Find("Guild").GetComponent<Guild>();
        guild.Missions.Add(gameObject);

        // Puts the mission button into the scroll view.
        MissionButton.transform.SetParent(guild.MissionView.transform.GetChild(0).GetChild(0));
        RectTransform mission_rect = MissionButton.GetComponent<RectTransform>();
        mission_rect.anchoredPosition = new Vector2(0, 30 + (-60 * (guild.Missions.Count)));
        mission_rect.offsetMax = new Vector2(0, mission_rect.offsetMax.y);
        mission_rect.offsetMin = new Vector2(0, mission_rect.offsetMin.y);

        RectTransform content_rect = guild.MissionView.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        content_rect.offsetMin = new Vector2(content_rect.offsetMin.x, content_rect.offsetMin.y - 60);

        ID = ID + 1;
        Name = "Mission " + ID;
        Reward = UnityEngine.Random.Range(0, 100);
        Duration = 10;
        MaxAdventurers = 2;
        isSelected = false;
        isAvailable = true;
        isRunning = false;
        isFinished = false;

        NameLabel.text = Name;
        RewardLabel.text = Reward.ToString();
        Description = "Hello World: " + Name;

        // For the first created mission is selected automatically in order to ensure that 
        // never an adventure can be selected without having a mission selected.
        if (ID == 1)
        {
            onClicked();
        }
	}
	
	// Update is called once per frame
	void Update () {
        adjustButtonColor();

        if (isFinished)
            removeMission();

        if (elapsed_time >= Duration)
            endMission();

	}

    private void adjustButtonColor()
    {
        if (isSelected)
           MissionButton.image.color = Color.blue;
        else if (!isAvailable)
            MissionButton.image.color = Color.gray;
        else
            MissionButton.image.color = Color.green;
    }

    // Only if the mission is available.
    public void onClicked()
    {
        if (isAvailable)
        {
            if (!isSelected)
            {
                foreach (GameObject o in guild.Missions)
                    if (o.GetComponent<Mission>().isSelected)
                        o.GetComponent<Mission>().isSelected = false;

                isSelected = true;
                guild.SelectedMission = gameObject;
                guild.MissionDescription.text = Description;
            }
        }
    }
}
