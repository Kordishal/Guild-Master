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

    public int MaxAdventurers;
    public Party Adventurers;

    public LinkedListNode<Location> CurrentLocation;
    public LinkedList<Location> PathToMissionLocation;

    public LinkedListNode<Stage> CurrentStage;
    public LinkedList<Stage> Stages;

    public Location Destination;

    public MissionContent.Target Target;

    public MissionGoal Type;

    public Guild guild;

    public bool isSelected;
    public bool isDisplayed;

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
        Debug.Log("Running Mission: " + Name + " on Stage: " + CurrentStage.Value.Name);

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
        guild.Missions.Remove(gameObject);
        guild.RunningMissions.Remove(this);
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

        // ensures that the content of the scroll view is large enough to hold all missions.
        RectTransform content_rect = guild.MissionView.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        content_rect.offsetMin = new Vector2(content_rect.offsetMin.x, content_rect.offsetMin.y - 60);

        ID = ID + 1;
        isSelected = false;
        isAvailable = true;
        isRunning = false;
        isFinished = false;

        generateMission();

        RewardLabel.text = Reward.ToString();
        NameLabel.text = Name;

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

        if (isRunning && isDisplayed)
            printMission();
	}

    private void printMission()
    {
        GameObject.Find("RunningMissionTitle").GetComponent<Text>().text = Name;
        GameObject.Find("RunningMissionDescription").GetComponent<Text>().text = Description;
        GameObject.Find("RunningMissionReward").GetComponent<Text>().text = Reward.ToString();
        GameObject.Find("RunningMissionMaxAdventurers").GetComponent<Text>().text = MaxAdventurers.ToString();
        GameObject.Find("RunningMissionAdventurers").GetComponent<Text>().text = Adventurers.ToString();
        GameObject.Find("RunningMissionCurrentLocation").GetComponent<Text>().text = CurrentLocation.Value.Name;
        GameObject.Find("RunningMissionCurrentStage").GetComponent<Text>().text = CurrentStage.Value.DisplayName;

        switch (CurrentStage.Value.Name)
        {
            case StageNames.GoToDestination:
                if (CurrentLocation != PathToMissionLocation.Last)
                    GameObject.Find("RunningMissionDistanceNext").GetComponent<Text>().text = World.getDistance(CurrentLocation.Value, CurrentLocation.Next.Value).ToString();
                GameObject.Find("RunningMissionTraveledDistance").GetComponent<Text>().text = CurrentStage.Value.DistanceTraveled.ToString();
                break;
            case StageNames.RetrieveTarget:
                break;
            case StageNames.ReturnToGuildHall:
                if (CurrentLocation != PathToMissionLocation.First)
                    GameObject.Find("RunningMissionDistanceNext").GetComponent<Text>().text = World.getDistance(CurrentLocation.Value, CurrentLocation.Previous.Value).ToString();
                GameObject.Find("RunningMissionTraveledDistance").GetComponent<Text>().text = CurrentStage.Value.DistanceTraveled.ToString();
                break;

        }

        
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
                if (guild.SelectedMission != null)
                {
                    var temp = guild.SelectedMission.GetComponent<Mission>();
                    temp.isSelected = false;
                }

                isSelected = true;
                guild.SelectedMission = gameObject;
                guild.MissionDescription.text = Description;
            }
        }
    }

    private void generateMission()
    {
        Type = MissionGoal.RetrieveTarget;
        Destination = World.Places[UnityEngine.Random.Range(0, World.Places.Count)];
        Target = MissionContent.Items[UnityEngine.Random.Range(0, MissionContent.Items.Count)];

        MaxAdventurers = 1;

        Reward = UnityEngine.Random.Range(50, 100);

        // Stages of the missions a adventurer has to go throught.
        Stages = new LinkedList<Stage>();

        // Travel Stages        
        Stages.AddFirst(MissionContent.Stages[(int)StageNames.GoToDestination]);
        
        // Fullfil Mission Stages
             
        Stages.AddLast(MissionContent.Stages[(int)StageNames.RetrieveTarget]);

        // Travel Back Stages

        Stages.AddLast(MissionContent.Stages[(int)StageNames.ReturnToGuildHall]);

        CurrentStage = Stages.First;

        PathToMissionLocation = new LinkedList<Location>(World.findShortestPath(World.GuildHall, Destination));
        CurrentLocation = PathToMissionLocation.First;

        //var temp = CurrentLocation;
        //string debug_string = "Path (" + Name + "): ";
        //while (temp != PathToMissionLocation.Last)
        //{
        //    debug_string += temp.Value.Name + " -> ";
        //    temp = temp.Next;
        //}
        //debug_string += temp.Value.Name + "!";
        //Debug.Log(debug_string);


        Name = "Mission: Find the " + Target.Name;
        Description = "The " + Target.Name + " has been lost. Please find it for us!";
    }

    public enum MissionGoal
    {
        RetrieveTarget,
        //KillTarget,
        //StealTarget,
        //ExploreLocation,
    }
}
