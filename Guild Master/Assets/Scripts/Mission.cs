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
    private List<Adventurer> adventurers_running_the_mission;

    public LinkedListNode<Location> CurrentLocation;
    public LinkedList<Location> PathToMissionLocation;

    public LinkedListNode<Stage> CurrentStage;
    public LinkedList<Stage> Stages;
    public PMC.Item TargetItem;
    public Location Destination;

    public MissionType Type;

    private Guild guild;


    public bool isSelected;

    public bool isAvailable;
    public bool isRunning;
    public bool isSuccess;
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
        Debug.Log("Running Mission: " + Name + " on Stage: " + CurrentStage.Value.Name);

        CurrentStage.Value.Action();

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
            foreach (Adventurer a in adventurers_running_the_mission)
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
        Type = MissionType.FindItem;
        MaxAdventurers = 1;

        Destination = World.Places[UnityEngine.Random.Range(0, World.Places.Count)];

        Reward = UnityEngine.Random.Range(50, 100);
        TargetItem = PMC.Items[UnityEngine.Random.Range(0, PMC.Items.Count)]; 

        Name = "Mission: Find the " + TargetItem.Name;
        Description = "The " + TargetItem.Name + " has been lost. Please find it for us!";

        Stages = new LinkedList<Stage>();

        Stages.AddFirst(new Stage("Go To Location", 3, AdventurerSkills.SkillNames.Walking, goToLocationOfMission, -1));
        Stages.AddLast(new Stage("Investigate For Lost Item", 2, AdventurerSkills.SkillNames.Investigate, useSkill, 5));
        Stages.AddLast(new Stage("Search For Lost Item", 4, AdventurerSkills.SkillNames.Perception, useSkill, 10));
        Stages.AddLast(new Stage("Return To Guild Hall", 1, AdventurerSkills.SkillNames.Walking, returnToGuildHall, -1));

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

    }

    public void returnToGuildHall()
    {
        if (CurrentLocation.Value == guild.GuildHall)
            CurrentStage.Value.FinishedWith = Stage.FinishState.Success;
        else
        {
            int distance_required = World.Distance(CurrentLocation.Value, CurrentLocation.Previous.Value);
            if (CurrentStage.Value.StepsDone == distance_required)
            {
                CurrentStage.Value.StepsDone = 0;
                CurrentLocation = CurrentLocation.Previous;
            }            
            else
                CurrentStage.Value.StepsDone += 1;
        }
    }

    public void goToLocationOfMission()
    {
        if (CurrentLocation.Value == Destination)
            CurrentStage.Value.FinishedWith = Stage.FinishState.Success;
        else
        {
            int distance_required = World.Distance(CurrentLocation.Value, CurrentLocation.Next.Value);

            if (CurrentStage.Value.StepsDone == distance_required)
            {
                CurrentStage.Value.StepsDone = 0;
                CurrentLocation = CurrentLocation.Next;
            }             
            else
                CurrentStage.Value.StepsDone += 1;
        }
    }

    public void useSkill()
    {   
        Adventurer hightest_skill_level = adventurers_running_the_mission[0];

        // Determine the adventurer with the highest skill level in the party for skill use.
        foreach (Adventurer adv in adventurers_running_the_mission)
            if (adv.Skills.Find(y => y.Name == CurrentStage.Value.Skill).Level > hightest_skill_level.Skills.Find(y => y.Name == CurrentStage.Value.Skill).Level)
                hightest_skill_level = adv;

        int result = hightest_skill_level.Skills.Find(y => y.Name == CurrentStage.Value.Skill).throwDiceVs(CurrentStage.Value.Difficulty);

        if (result <= -20)
            CurrentStage.Value.FinishedWith = Stage.FinishState.CriticalFailure;
        else if (result <= 0)
            CurrentStage.Value.FinishedWith = Stage.FinishState.Failure;
        else if (result > 0)
            CurrentStage.Value.FinishedWith = Stage.FinishState.Success;
        else if (result > 20)
            CurrentStage.Value.FinishedWith = Stage.FinishState.CriticalSuccess; 
    }


    /// <summary>
    /// Class for procedural mission content.
    /// </summary>
    public class PMC
    {
        static public List<Item> Items = new List<Item>() { new Item("Cat", true), new Item("Family Heirloom", false)};

        public struct Item
        {
            public string Name;
            public bool isAnimal;

            public Item(string name, bool is_animal) { Name = name; isAnimal = is_animal; }
        }
    }

    /// <summary>
    /// Each mission is compossed of various stages. To complete a mission each stage has to be completed successfully by the party.
    /// </summary>
    public class Stage
    {
        public string Name;
        public int Difficulty;
        public AdventurerSkills.SkillNames Skill;
        public StageAction Action;
        public int Repeatability;
        public int Repeated = 0;
        public int StepsDone;
        public FinishState FinishedWith = FinishState.None;

        public Stage(string name, int difficulty, AdventurerSkills.SkillNames skill, StageAction action, int repeatability)
        {
            Name = name;
            Difficulty = difficulty;
            Skill = skill;
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

        public delegate void StageAction();



    }

    public enum MissionType
    {
        FindItem,
        KillBeast,
    }
}
