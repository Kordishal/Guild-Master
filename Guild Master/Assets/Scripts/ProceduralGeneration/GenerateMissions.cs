using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateMissions : MonoBehaviour {

    public GameObject MissionButtonPrefab;

    public GameObject generateMission()
    {
        GameObject mission = Instantiate(MissionButtonPrefab);


        Mission m = mission.GetComponent<Mission>();
        m.Type = Mission.MissionGoal.RetrieveTarget;
        m.Destination = World.Places[Random.Range(0, World.Places.Count)];
        m.Target = createTarget();

        m.MaxAdventurers = 1;

        m.Reward = 10;

        // Stages of the missions a adventurer has to go throught.
        m.Stages = new LinkedList<Stage>();

        // Travel Stages        
        m.Stages.AddFirst(new Stage(Stages[(int)StageNames.GoToDestination]));

        // Fullfil Mission Stages

        m.Stages.AddLast(new Stage(Stages[(int)StageNames.RetrieveTarget]));

        // Travel Back Stages

        m.Stages.AddLast(new Stage(Stages[(int)StageNames.ReturnToGuildHall]));

        m.CurrentStage = m.Stages.First;

        m.PathToMissionLocation = new LinkedList<Location>(World.findShortestPath(World.GuildHall, m.Destination));
        m.CurrentLocation = m.PathToMissionLocation.First;

        m.Name = "Mission: Find the " + m.Target.Name;
        m.Description = "The " + m.Target.Name + " has been lost. Please find it for us!";

        return mission;
    }

    public List<GameObject> generateStartUpMissions()
    {
        List<GameObject> missions = new List<GameObject>();

        for (int i = 0; i < 5; i++)
        {
            missions.Add(Instantiate(MissionButtonPrefab));

            Mission m = missions[i].GetComponent<Mission>();
            m.Type = Mission.MissionGoal.RetrieveTarget;
            m.Destination = World.Places[Random.Range(0, World.Places.Count)];
            m.Target = createTarget();

            m.MaxAdventurers = 1;

            m.Reward = 10;

            // Stages of the missions a adventurer has to go throught.
            m.Stages = new LinkedList<Stage>();

            // Travel Stages        
            m.Stages.AddFirst(new Stage(Stages[(int)StageNames.GoToDestination]));

            // Fullfil Mission Stages

            m.Stages.AddLast(new Stage(Stages[(int)StageNames.RetrieveTarget]));

            // Travel Back Stages

            m.Stages.AddLast(new Stage(Stages[(int)StageNames.ReturnToGuildHall]));

            m.CurrentStage = m.Stages.First;

            m.PathToMissionLocation = new LinkedList<Location>(World.findShortestPath(World.GuildHall, m.Destination));
            m.CurrentLocation = m.PathToMissionLocation.First;

            m.Name = "Mission: Find the " + m.Target.Name;
            m.Description = "The " + m.Target.Name + " has been lost. Please find it for us!";

            // Make the first mission selected.
            if (i == 1)
                m.onClicked();
        }
        return missions;
    }

    void Awake ()
    {
        Debug.Log("Generate Mission Parameters");

        Stages = new Stage[(int)StageNames.MAX_STAGES];

        Stages[(int)StageNames.GoToDestination] = new Stage(StageNames.GoToDestination, "Go to Destination", 3, goToLocationOfMission, -1);
        Stages[(int)StageNames.RetrieveTarget] = new Stage(StageNames.RetrieveTarget, "Retrieve Target", 4, retrieveTarget, 10);
        Stages[(int)StageNames.ReturnToGuildHall] = new Stage(StageNames.ReturnToGuildHall, "Return to Guild Hall", 1, returnToGuildHall, -1);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Stage[] Stages;

    private Target createTarget()
    {
        var s = target_names[Random.Range(0, target_names.Length)];
        return new Target(s, Targets[s], World.Places[Random.Range(0, World.Places.Count)]);
    }
    private Dictionary<string, Category> Targets = new Dictionary<string, Category>() {
        { "Cat", Category.Animal },
        { "Dog", Category.Animal },
        { "Bird", Category.Animal },
        { "Necklace", Category.Item },
        { "Trinket", Category.Item },
        { "Sword", Category.Item },
        { "Magical Herb", Category.Plant },
        { "Herb", Category.Plant },
        { "Brother", Category.Person },
        { "Sister", Category.Person },
    };

    private string[] target_names = new string[] { "Cat", "Dog", "Bird", "Necklace", "Trinket", "Sword", "Magical Herb", "Herb", "Brother", "Sister" };


    // STAGE ACTIONS
    static private void travel(Mission mission)
    {

    }

    static private void returnToGuildHall(Mission mission)
    {
        if (mission.CurrentLocation.Value == World.GuildHall)
            mission.CurrentStage.Value.FinishedWith = Stage.FinishState.Success;
        else
        {
            int distance_required = World.getDistance(mission.CurrentLocation.Value, mission.CurrentLocation.Previous.Value);

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
        int distance_required = World.getDistance(mission.CurrentLocation.Value, mission.CurrentLocation.Next.Value);

        var travel = mission.Adventurers.getFastestTravel();

        if (travel.Distance > 0)
        {
            mission.CurrentStage.Value.DistanceTraveled += travel.Distance;
            mission.Adventurers.useGroupSkill(travel.SkillUsed);

            double carry_over = 0;

            do
            {
                if (mission.CurrentStage.Value.DistanceTraveled >= distance_required)
                {
                    carry_over = mission.CurrentStage.Value.DistanceTraveled - distance_required;

                    mission.CurrentStage.Value.DistanceTraveled = 0;
                    mission.CurrentLocation = mission.CurrentLocation.Next;

                    mission.CurrentStage.Value.DistanceTraveled += carry_over;

                    if (mission.CurrentLocation.Value == mission.Destination)
                    {
                        mission.CurrentStage.Value.FinishedWith = Stage.FinishState.Success;
                        return;
                    }
                }
                else
                    carry_over = 0;

            } while (carry_over > 0);
        }
    }

    static private void retrieveTarget(Mission mission)
    {
        SkillNames used_skill_name = SkillNames.LAST_ENTRY;

        switch (mission.Target.Category)
        {
            case Category.Animal:
                used_skill_name = SkillNames.Tracking;
                break;
            case Category.Item:
                used_skill_name = SkillNames.Searching;
                break;
            case Category.Person:
                used_skill_name = SkillNames.Tracking;
                break;
            case Category.Plant:
                used_skill_name = SkillNames.Searching;
                break;
        }

        Skill used_skill = mission.Adventurers.Members[0].Skills[(int)used_skill_name];

        for (int i = 1; i < mission.Adventurers.Members.Count; i++)
            if (used_skill.Level < mission.Adventurers.Members[i].Skills[(int)used_skill_name].Level)
                used_skill = mission.Adventurers.Members[i].Skills[(int)used_skill_name];

        int result = used_skill.throwDiceVs(mission.CurrentStage.Value.Difficulty);

        if (result <= -20)
            mission.CurrentStage.Value.FinishedWith = Stage.FinishState.CriticalFailure;
        else if (result <= 0)
            mission.CurrentStage.Value.FinishedWith = Stage.FinishState.Failure;
        else if (result > 0)
            mission.CurrentStage.Value.FinishedWith = Stage.FinishState.Success;
        else if (result > 20)
            mission.CurrentStage.Value.FinishedWith = Stage.FinishState.CriticalSuccess;
    }

}
