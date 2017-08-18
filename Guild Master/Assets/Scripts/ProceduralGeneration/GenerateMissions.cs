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
        m.Target = MissionContent.Items[Random.Range(0, MissionContent.Items.Count)];

        m.MaxAdventurers = 1;

        m.Reward = 10;

        // Stages of the missions a adventurer has to go throught.
        m.Stages = new LinkedList<Stage>();

        // Travel Stages        
        m.Stages.AddFirst(MissionContent.Stages[(int)StageNames.GoToDestination]);

        // Fullfil Mission Stages

        m.Stages.AddLast(MissionContent.Stages[(int)StageNames.RetrieveTarget]);

        // Travel Back Stages

        m.Stages.AddLast(MissionContent.Stages[(int)StageNames.ReturnToGuildHall]);

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
            m.Target = MissionContent.Items[Random.Range(0, MissionContent.Items.Count)];

            m.MaxAdventurers = 1;

            m.Reward = 10;

            // Stages of the missions a adventurer has to go throught.
            m.Stages = new LinkedList<Stage>();

            // Travel Stages        
            m.Stages.AddFirst(MissionContent.Stages[(int)StageNames.GoToDestination]);

            // Fullfil Mission Stages

            m.Stages.AddLast(MissionContent.Stages[(int)StageNames.RetrieveTarget]);

            // Travel Back Stages

            m.Stages.AddLast(MissionContent.Stages[(int)StageNames.ReturnToGuildHall]);

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
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
