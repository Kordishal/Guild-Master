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
    }

    public Text NameLabel;
    public Text RewardLabel;

    public Button MissionButton;

	// Use this for initialization
	void Start () {
        ID = ID + 1;
        Name = "Mission " + ID;
        Reward = UnityEngine.Random.Range(0, 100);
        MaxAdventurers = 2;
        isSelected = false;
        isAvailable = true;
        isFulfilled = false;

        NameLabel.text = Name;
        RewardLabel.text = Reward.ToString();

        MissionButton.onClick.AddListener(onClicked);

        // For the first created mission is selected automatically in order to ensure that 
        // never an adventure can be selected without having a mission selected.
        if (ID == 1)
        {
            onSelected(new EventArgs());
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
            onSelected(new EventArgs());
    }

    public event EventHandler<EventArgs> OnSelected;
    protected virtual void onSelected(EventArgs e)
    {
        EventHandler<EventArgs> handler = OnSelected;
        if (handler != null)
            handler(this, e);

    }
}
