using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateMissions : MonoBehaviour {

    private static int mission_count = 0;

    public GameObject MissionButtonPrefab;

    public GameObject generateMission(int max_targets)
    {
        GameObject mission = Instantiate(MissionButtonPrefab);
        Stage temp_stage;

        Mission m = mission.GetComponent<Mission>();
        m.Identifier = mission_count;
        mission_count += 1;
        m.Goal = Mission.MissionGoal.RetrieveTarget;

        m.Targets = new List<Target>();
        for (int i = 0; i < max_targets; i++)
            m.Targets.Add(createTarget());

        m.MaxAdventurers = 1;

        m.Reward = 10;

        m.Stages = new LinkedList<Stage>();

        // Adds a stage in the mission for each target which must be recovered, killed etc.
        foreach (Target t in m.Targets)
        {
            temp_stage = new Stage();

            temp_stage.Target = t;

            switch (m.Goal)
            {
                case Mission.MissionGoal.RetrieveTarget:
                    temp_stage.Name = StageNames.RetrieveTarget;
                    temp_stage.DisplayName = "Retrieve " + temp_stage.Target.Name;
                    // Implement a scaling difficulty depending on various factors.
                    temp_stage.Difficulty = Random.Range(0, 10);
                    temp_stage.Action = retrieveTarget;
                    temp_stage.Repeatability = 10;                   
                    break;
            }


            m.Stages.AddLast(temp_stage);
        }

        // Now add a movement stage inbetween each to ensure that the party actually moves where the mission can be finished.

        Location current_location = World.GuildHall;
        LinkedListNode<Stage> current_stage = m.Stages.First;
        do
        {
            if (current_stage.Value.Target.Location != current_location)
            {
                temp_stage = new Stage();
                temp_stage.Name = StageNames.move_to_target;
                temp_stage.DisplayName = "Move to " + current_stage.Value.Target.Location.Name;
                temp_stage.Action = move;
                temp_stage.Repeatability = -1;

                temp_stage.path_to_target_location = World.findShortestPath(current_location, current_stage.Value.Target.Location);

                m.Stages.AddBefore(current_stage, temp_stage);
            }

            current_location = current_stage.Value.Target.Location;
            if (current_stage.Next != null)
                current_stage = current_stage.Next;

        } while (current_stage != m.Stages.Last);

        // Add a movement stage to return to the guild hall at the end.
        temp_stage = new Stage();
        temp_stage.Name = StageNames.move_to_target;
        temp_stage.DisplayName = "Move to " + World.GuildHall;
        temp_stage.Action = move;
        temp_stage.Repeatability = -1;

        temp_stage.path_to_target_location = World.findShortestPath(current_location, World.GuildHall);

        m.Stages.AddLast(temp_stage);

        m.CurrentStage = m.Stages.First;

        string mission_name = "";
        switch (m.Goal)
        {
            case Mission.MissionGoal.RetrieveTarget:
                mission_name += "Find the Target";
                break;
        }


        m.Name = mission_name;

        string mission_description = "";

        switch (m.Goal)
        {
            case Mission.MissionGoal.RetrieveTarget:
                mission_description += "We lost the following item(s): ";
                break;
        }

        foreach (Target t in m.Targets)
            mission_description += t.Name + ", ";


        mission_description += "Please find ";

        if (m.Targets.Count > 1)
            mission_description += "them ";
        else
            mission_description += "it ";

        mission_description += "for us!";

        m.Description = mission_description;

        return mission;
    }

    public List<GameObject> generateStartUpMissions()
    {
        List<GameObject> missions = new List<GameObject>();

        for (int i = 0; i < 5; i++)
        {
            missions.Add(generateMission(1));

            // Make the first mission selected.
            if (i == 1)
                missions[i].GetComponent<Mission>().onClicked();
        }
        return missions;
    }

    void Awake ()
    {
        Debug.Log("Generate Mission Parameters");
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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
    static private void move(Mission mission)
    {
        if (mission.Adventurers.CurrentLocation == null)
            mission.Adventurers.CurrentLocation = mission.CurrentStage.Value.path_to_target_location.First;

        int distance_required = World.getDistance(mission.Adventurers.CurrentLocation.Value, mission.Adventurers.CurrentLocation.Next.Value);

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
                    mission.Adventurers.CurrentLocation = mission.Adventurers.CurrentLocation.Next;

                    mission.CurrentStage.Value.DistanceTraveled += carry_over;

                    if (mission.Adventurers.CurrentLocation == mission.CurrentStage.Value.path_to_target_location.Last)
                    {
                        mission.CurrentStage.Value.FinishedWith = Stage.FinishState.Success;
                        mission.Adventurers.CurrentLocation = null;
                        return;
                    }
                }
                else
                    carry_over = 0;

            } while (carry_over > 0);
        }
        else
        {
            if (Calendar.Hour >= 8 && Calendar.Hour <= 23)
                mission.Adventurers.isResting = true;
            else
                mission.Adventurers.setupCamp();
        }
    }

    static private void retrieveTarget(Mission mission)
    {
        SkillNames used_skill_name = SkillNames.LAST_ENTRY;

        switch (mission.CurrentStage.Value.Target.Category)
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
