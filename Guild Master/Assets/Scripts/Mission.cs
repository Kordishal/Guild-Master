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

    public PMC.Target Target;

    public MissionGoal Type;

    private Guild guild;

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

                guild.GuildMaster.Currency += (int)Reward;
            }
        }

        isRunning = false;
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
                    GameObject.Find("RunningMissionDistanceNext").GetComponent<Text>().text = World.Distance(CurrentLocation.Value, CurrentLocation.Next.Value).ToString();
                GameObject.Find("RunningMissionTraveledDistance").GetComponent<Text>().text = CurrentStage.Value.DistanceTraveled.ToString();
                break;
            case StageNames.RetrieveTarget:
                break;
            case StageNames.ReturnToGuildHall:
                if (CurrentLocation != PathToMissionLocation.First)
                    GameObject.Find("RunningMissionDistanceNext").GetComponent<Text>().text = World.Distance(CurrentLocation.Value, CurrentLocation.Previous.Value).ToString();
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
                foreach (GameObject o in guild.Missions)
                    if (o.GetComponent<Mission>().isSelected)
                        o.GetComponent<Mission>().isSelected = false;

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
        Target = PMC.Items[UnityEngine.Random.Range(0, PMC.Items.Count)];

        MaxAdventurers = 1;

        Reward = UnityEngine.Random.Range(50, 100);

        // Stages of the missions a adventurer has to go throught.
        Stages = new LinkedList<Stage>();

        // Travel Stages        
        Stages.AddFirst(PMC.Stages[(int)StageNames.GoToDestination]);
        
        // Fullfil Mission Stages
             
        Stages.AddLast(PMC.Stages[(int)StageNames.RetrieveTarget]);

        // Travel Back Stages

        Stages.AddLast(PMC.Stages[(int)StageNames.ReturnToGuildHall]);

        CurrentStage = Stages.First;

        PathToMissionLocation = new LinkedList<Location>(World.findShortestPath(guild.GuildHall, Destination));
        CurrentLocation = PathToMissionLocation.First;

        var temp = CurrentLocation;
        string debug_string = "Path (" + Name + "): ";
        while (temp != PathToMissionLocation.Last)
        {
            debug_string += temp.Value.Name + " -> ";
            temp = temp.Next;
        }
        debug_string += temp.Value.Name + "!";
        Debug.Log(debug_string);


        Name = "Mission: Find the " + Target.Name;
        Description = "The " + Target.Name + " has been lost. Please find it for us!";
    }

 

    /// <summary>
    /// Class for procedural mission content.
    /// </summary>
    public class PMC
    {
        static PMC()
        {
            Stages = new Stage[(int)StageNames.MAX_STAGES];

            Stages[(int)StageNames.GoToDestination] = new Stage(StageNames.GoToDestination, "Go to Destination", 3, goToLocationOfMission, -1);
            Stages[(int)StageNames.RetrieveTarget] = new Stage(StageNames.RetrieveTarget, "Retrieve Target", 4, retrieveTarget, 10);
            Stages[(int)StageNames.ReturnToGuildHall] = new Stage(StageNames.ReturnToGuildHall, "Return to Guild Hall", 1, returnToGuildHall, -1);
        }

        static public Stage[] Stages;

        static public List<Target> Items = new List<Target>() {
            new Target("Cat", Category.Animal),
            new Target("Dog", Category.Animal),
            new Target("Bird", Category.Animal),
            new Target("Necklace", Category.Item),
            new Target("Trinket", Category.Item),
            new Target("Sword", Category.Item),
            new Target("Magical Herb", Category.Plant),
            new Target("Herb", Category.Plant),
            new Target("Brother", Category.Person),
            new Target("Sister", Category.Person),
        };

        public class Target
        {
            public string Name;
            public Category Category;

            public Target(string name, Category category)
            {
                Name = name;
                Category = category;
            }
        }


        static private void returnToGuildHall(Mission mission)
        {
            if (mission.CurrentLocation.Value == mission.guild.GuildHall)
                mission.CurrentStage.Value.FinishedWith = Stage.FinishState.Success;
            else
            {
                int distance_required = World.Distance(mission.CurrentLocation.Value, mission.CurrentLocation.Previous.Value);

                if (mission.CurrentStage.Value.DistanceTraveled >= distance_required)
                {
                    mission.CurrentStage.Value.DistanceTraveled = 0;
                    mission.CurrentLocation = mission.CurrentLocation.Previous;
                }
                else
                {
                    var travel = mission.Adventurers.getFastestTravel();

                    if (travel.Distance > 0)
                    {
                        mission.CurrentStage.Value.DistanceTraveled += travel.Distance;
                    }
                }
            }
        }

        static private void goToLocationOfMission(Mission mission)
        {
            if (mission.CurrentLocation.Value == mission.Destination)
                mission.CurrentStage.Value.FinishedWith = Stage.FinishState.Success;
            else
            {
                int distance_required = World.Distance(mission.CurrentLocation.Value, mission.CurrentLocation.Next.Value);

                if (mission.CurrentStage.Value.DistanceTraveled >= distance_required)
                {
                    mission.CurrentStage.Value.DistanceTraveled = 0;
                    mission.CurrentLocation = mission.CurrentLocation.Next;
                }
                else
                {
                    var travel = mission.Adventurers.getFastestTravel();

                    if (travel.Distance > 0)
                    {
                        mission.CurrentStage.Value.DistanceTraveled += travel.Distance;
                        mission.Adventurers.useGroupSkill(travel.SkillUsed);

                    }
                }
            }
        }

        static private void retrieveTarget(Mission mission)
        {
            AdventurerSkills.SkillNames used_skill_name = AdventurerSkills.SkillNames.LAST_ENTRY;

            switch (mission.Target.Category)
            {
                case Category.Animal:
                    used_skill_name = AdventurerSkills.SkillNames.Tracking;
                    break;
                case Category.Item:
                    used_skill_name = AdventurerSkills.SkillNames.Searching;
                    break;
                case Category.Person:
                    used_skill_name = AdventurerSkills.SkillNames.Tracking;
                    break;
                case Category.Plant:
                    used_skill_name = AdventurerSkills.SkillNames.Searching;
                    break;
            }

            AdventurerSkills.Skill used_skill = mission.Adventurers.Members[0].Skills[(int)used_skill_name];

            for (int i = 1; i < mission.Adventurers.Members.Count; i++)
                if (used_skill.Level < mission.Adventurers.Members[i].Skills[(int)used_skill_name].Level)
                    used_skill = mission.Adventurers.Members[i].Skills[(int)used_skill_name];

            int result = used_skill.throwDiceVs(mission.CurrentStage.Value.Difficulty);

            Debug.Log("Result: " + result);

            if (result <= -20)
                mission.CurrentStage.Value.FinishedWith = Stage.FinishState.CriticalFailure;
            else if (result <= 0)
                mission.CurrentStage.Value.FinishedWith = Stage.FinishState.Failure;
            else if (result > 0)
                mission.CurrentStage.Value.FinishedWith = Stage.FinishState.Success;
            else if (result > 20)
                mission.CurrentStage.Value.FinishedWith = Stage.FinishState.CriticalSuccess;
        }


        public enum Category
        {
            Item,
            Animal,
            Plant,
            Person,
        }
    }

    /// <summary>
    /// Each mission is compossed of various stages. To complete a mission each stage has to be completed successfully by the party.
    /// </summary>
    public class Stage
    {
        public StageNames Name;
        public string DisplayName;
        public int Difficulty;
        public StageAction Action;
        public int Repeatability;
        public int Repeated = 0;
        public double DistanceTraveled;
        public FinishState FinishedWith = FinishState.None;

        public Stage(StageNames name, string display_name, int difficulty, StageAction action, int repeatability)
        {
            Name = name;
            DisplayName = display_name;
            Difficulty = difficulty;
            Action = action;
            Repeatability = repeatability;
        }

        public enum FinishState
        {
            CriticalFailure,
            Failure,
            None,
            Success,
            CriticalSuccess
        }

        public delegate void StageAction(Mission mission);
    }

    public enum StageNames
    {
        GoToDestination,
        ReturnToGuildHall,
        RetrieveTarget,

        MAX_STAGES
    }

    public enum MissionGoal
    {
        RetrieveTarget,
        //KillTarget,
        //StealTarget,
        //ExploreLocation,
    }
}
