using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour {

    public int Identifier;

    public string Name;
    public double Reward;
    public string Description;

    public int MaxAdventurers;
    public Party Adventurers;

    public MissionGoal Goal;
    public List<Target> Targets;

    public LinkedListNode<Stage> CurrentStage;
    public LinkedList<Stage> Stages;

    public Guild Guild;

    public bool isSelected;
    public bool isAvailable;

    public bool isRunning;
    public bool isSuccess;
    public bool isFinished;

    public void StartMission(List<Adventurer> adventurers)
    {
        isRunning = true;
        Adventurers = new Party(adventurers);
        foreach (Adventurer a in Adventurers.Members)
            a.isAvailable = false;

        Guild.Calendar.hourlyTrigger += RunningMission;
    }

    private void RunningMission(object sender, EventArgs e)
    {

        if (Adventurers.isCamping)
        {
            Adventurers.Camp();
            return;
        }

        if (Adventurers.isResting)
        {
            Adventurers.Rest();
            return;
        }
            
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
                    if (CurrentStage.Value.Repeated >= CurrentStage.Value.Repeatability)
                    {
                        isFinished = true;
                        isSuccess = false;
                    }
                }
                break;
        }

        if (isFinished)
        {
            EndMission();
        }
    }

    public void EndMission()
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
        RemoveMission();
    }

    private void RemoveMission()
    {
        Guild.removeMission(gameObject);
        Guild.RunningMissions.Remove(this);
        GetComponent<Button>().enabled = false;
        Guild.Calendar.hourlyTrigger -= RunningMission;

        Destroy(gameObject);
    }

    public Text IdentifierLabel;
    public Text NameLabel;
    public Text RewardLabel;

    // Use this for initialization
    void Start () {
        Guild = GameObject.Find("Guild").GetComponent<Guild>();

        // Are the same for every mission.
        isSelected = false;
        isAvailable = true;
        isRunning = false;
        isFinished = false;

        // Are here because they do not change and it is the same code for every mission.
        IdentifierLabel.text = Identifier.ToString();
        NameLabel.text = Name;
        RewardLabel.text = Reward.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        AdjustButtonColor();
	}

    private void AdjustButtonColor()
    {
        if (isSelected)
           GetComponent<Button>().image.color = Color.blue;
        else if (!isAvailable)
            GetComponent<Button>().image.color = Color.gray;
        else
            GetComponent<Button>().image.color = Color.green;
    }

    // Only if the mission is available.
    public void OnClicked()
    {
        if (isAvailable)
        {
            if (!isSelected)
            {
                if (Guild.SelectedMission != null)
                {
                    var temp = Guild.SelectedMission.GetComponent<Mission>();
                    temp.isSelected = false;
                }

                isSelected = true;
                Guild.SelectedMission = gameObject;
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
