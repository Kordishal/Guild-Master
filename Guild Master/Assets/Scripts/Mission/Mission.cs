using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour {

    public string Name;
    public double Reward;
    public string Description;

    public int MaxAdventurers;
    public Party Adventurers;

    public LinkedListNode<Location> CurrentLocation;
    public LinkedList<Location> PathToMissionLocation;

    public LinkedListNode<Stage> CurrentStage;
    public LinkedList<Stage> Stages;

    public Location Destination;

    public Target Target;

    public MissionGoal Type;

    public Guild guild;

    public bool isSelected;
    public bool isAvailable;

    public bool isRunning;
    public bool isSuccess;
    public bool isFinished;

    public void startMission(List<Adventurer> adventurers)
    {
        isRunning = true;
        Adventurers = new Party(adventurers);
        foreach (Adventurer a in Adventurers.Members)
            a.isAvailable = false;

        guild.Calendar.hourlyTrigger += runningMission;
    }

    private void runningMission(object sender, EventArgs e)
    {
        CurrentStage.Value.Action(this);

        CurrentStage.Value.Repeated += 1;

        switch (CurrentStage.Value.FinishedWith)
        {
            case Stage.FinishState.None:
                break;
            case Stage.FinishState.CriticalSuccess:
            case Stage.FinishState.Success:
                if (CurrentStage.Next != null)
                    CurrentStage = CurrentStage.Next;
                else
                {
                    CurrentStage = null;
                    isFinished = true;
                    isSuccess = true;
                }             
                break;
            case Stage.FinishState.CriticalFailure:
            case Stage.FinishState.Failure:
                if (CurrentStage.Value.Repeatability != -1)
                {
                    if (CurrentStage.Value.Repeated > CurrentStage.Value.Repeatability)
                    {
                        CurrentStage.Value.FinishedWith = Stage.FinishState.Failure;
                        isFinished = true;
                        isSuccess = false;
                    }
                }
                break;
        }

        if (isFinished)
        {
            endMission();
        }
    }

    public void endMission()
    {
        // decrease reward by the cost of the adventurers.
        // Only assign reward if the mission was a success.
        if (isSuccess)
        {
            double full_reward = Reward;
            foreach (Adventurer a in Adventurers.Members)
            {
                Reward = Reward - (full_reward * a.Cost);
                a.isAvailable = true;

                Guild.CurrentWealth += (int)Reward;
            }
        }

        isRunning = false;
        removeMission();
    }

    private void removeMission()
    {
        guild.removeMission(gameObject);
        guild.RunningMissions.Remove(this);
        GetComponent<Button>().enabled = false;
        guild.Calendar.hourlyTrigger -= runningMission;

        Destroy(gameObject);
    }

    public Text NameLabel;
    public Text RewardLabel;

    // Use this for initialization
    void Start () {
        guild = GameObject.Find("Guild").GetComponent<Guild>();

        isSelected = false;
        isAvailable = true;
        isRunning = false;
        isFinished = false;


        // TODO: Move to interface class.
        RewardLabel.text = Reward.ToString();
        NameLabel.text = Name;  
	}
	
	// Update is called once per frame
	void Update () {
        adjustButtonColor();
	}

    private void adjustButtonColor()
    {
        if (isSelected)
           GetComponent<Button>().image.color = Color.blue;
        else if (!isAvailable)
            GetComponent<Button>().image.color = Color.gray;
        else
            GetComponent<Button>().image.color = Color.green;
    }

    // Only if the mission is available.
    public void onClicked()
    {
        if (isAvailable)
        {
            if (!isSelected)
            {
                if (guild.SelectedMission != null)
                {
                    var temp = guild.SelectedMission.GetComponent<Mission>();
                    temp.isSelected = false;
                }

                isSelected = true;
                guild.SelectedMission = gameObject;
                // TODO: Update this to properly show the mission in the apropriate part.
                // guild.MissionDescription.text = Description;
            }
        }
    }

    public enum MissionGoal
    {
        RetrieveTarget,
        //KillTarget,
        //StealTarget,
        //ExploreLocation,
    }
}
