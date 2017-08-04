using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour {

    private static int ID = 0;
    public string Name;
    public double Reward;
    public int MaxAdventurers;

    static private int current_missions;

    private Guild guild;

    public bool isAvailable;
    public bool isSelected;
    public bool isFulfilled;

    public void runMission(Guild guild, List<Adventurer> adventurers)
    {
        double full_reward = Reward;
        foreach (Adventurer a in adventurers)
            Reward = Reward - (full_reward * a.Cost);

        guild.GuildMaster.Currency += (int)Reward;

        foreach (Adventurer a in adventurers)
        {
            a.isAvailable = true;
            Debug.Log(a.Name + ": " + a.isAvailable);
        }


        isFulfilled = true;
        current_missions -= 1;
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
        MaxAdventurers = 2;
        isSelected = false;
        isAvailable = true;
        isFulfilled = false;

        NameLabel.text = Name;
        RewardLabel.text = Reward.ToString();

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


    public void selectDifferentActiveMission(Mission mission)
    {
        isSelected = false;
        mission.onClicked();
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
            }
        }
    }
}
