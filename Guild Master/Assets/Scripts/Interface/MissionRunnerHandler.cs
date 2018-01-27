using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionRunnerHandler : MonoBehaviour {

    public ScrollRect AdventurerView;
    public ScrollRect MissionView;

    public Button StartMission;

    public Text Messages;

    public Guild Guild;

	// Use this for initialization
	void Start () {
        Guild.change_mission_amount += onChangedMissionCount;
        Guild.change_adventurer_amount += onChangedAdventurerCount; 
	}

    // Update is called once per frame
    void Update()
    {
        //TODO: Change it so this only runs if there is a change to the adventurer;
        if (current_adventurer_inspected != null)
            display_inspected_adventurer();

        if (current_mission_inspected != null)
            display_inspected_mission();
    }

    private GameObject current_adventurer_inspected;
    public void inspectNextAdventurer()
    {
        if (Guild.Adventurers.Count == 0)
            return;

        if (current_adventurer_inspected == null)
            current_adventurer_inspected = Guild.Adventurers[0];
        else
        {
            var i = Guild.Adventurers.IndexOf(current_adventurer_inspected) + 1;
            if (i >= Guild.Adventurers.Count)
                current_adventurer_inspected = Guild.Adventurers[0];
            else
                current_adventurer_inspected = Guild.Adventurers[i];
        }
    }
    private void display_inspected_adventurer()
    {
        var adv = current_adventurer_inspected.GetComponent<Adventurer>();
        GameObject.Find("AdventurerName").GetComponent<Text>().text = adv.Name;
        GameObject.Find("AdventurerLevel").GetComponent<Text>().text = adv.Level.ToString();

        string skills = "";
        foreach (Skill s in adv.Skills)
        {
            skills += s.Name + " " + s.Level + "|" + s.Experience + "|" + s.ExperienceForNextLevel + "|" + (int)s.CurrentUses + "/" + (int)s.MaxUses + "|" + (int)s.Distance + "\n";
        }
        GameObject t = GameObject.Find("AdventurerSkills");
        t.GetComponent<Text>().text = skills;
    }

    private GameObject current_mission_inspected;
    public void inspectNextMission()
    {
        if (Guild.Missions.Count == 0)
            return;

        if (current_mission_inspected == null)
            current_mission_inspected = Guild.Missions[0];
        else
        {
            var i = Guild.Missions.IndexOf(current_mission_inspected) + 1;
            if (i >= Guild.Missions.Count)
                current_mission_inspected = Guild.Missions[0];
            else
                current_mission_inspected = Guild.Missions[i];
        }
    }

    private void display_inspected_mission()
    {
        var m = current_mission_inspected.GetComponent<Mission>();
        GameObject.Find("RunningMissionTitle").GetComponent<Text>().text = m.Name;
        GameObject.Find("MissionDisplayIdentifier").GetComponent<Text>().text = m.Identifier.ToString();
        GameObject.Find("RunningMissionDescription").GetComponent<Text>().text = m.Description;
        GameObject.Find("RunningMissionReward").GetComponent<Text>().text = m.Reward.ToString();
        GameObject.Find("RunningMissionMaxAdventurers").GetComponent<Text>().text = m.MaxAdventurers.ToString();


        if (m.isRunning)
        {
            GameObject.Find("RunningMissionAdventurers").GetComponent<Text>().text = m.Adventurers.ToString();

            GameObject.Find("RunningMissionCurrentStage").GetComponent<Text>().text = m.CurrentStage.Value.DisplayName;

            if (m.Adventurers.CurrentLocation != null)
            {
                GameObject.Find("RunningMissionCurrentLocation").GetComponent<Text>().text = m.Adventurers.CurrentLocation.Value.Name;

                switch (m.CurrentStage.Value.Name)
                {
                    case StageNames.move_to_target:
                        if (m.Adventurers.CurrentLocation != m.CurrentStage.Value.path_to_target_location.Last)
                            GameObject.Find("RunningMissionDistanceNext").GetComponent<Text>().text = World.getDistance(m.Adventurers.CurrentLocation.Value, m.Adventurers.CurrentLocation.Next.Value).ToString();
                        GameObject.Find("RunningMissionTraveledDistance").GetComponent<Text>().text = m.CurrentStage.Value.DistanceTraveled.ToString();
                        break;
                    case StageNames.RetrieveTarget:
                        break;
                }
            }
        }
        else
        {
            GameObject.Find("RunningMissionAdventurers").GetComponent<Text>().text = "No Adventurers selected yet";
            GameObject.Find("RunningMissionCurrentStage").GetComponent<Text>().text = "Not Running";

            GameObject.Find("RunningMissionCurrentLocation").GetComponent<Text>().text = "None";
            GameObject.Find("RunningMissionDistanceNext").GetComponent<Text>().text = "0";
            GameObject.Find("RunningMissionTraveledDistance").GetComponent<Text>().text = "0";
        }
    }

    public void onChangedMissionCount(object sender, EventArgs e)
    {
        int count = 1;
        foreach (GameObject mission in ((Guild)sender).Missions)
        {
            // TODO: Figure out why some missions are null in this list...
            //if (mission == null)
            //    continue;

            mission.transform.SetParent(MissionView.transform.GetChild(0).GetChild(0));
            RectTransform mission_rect = mission.GetComponent<Button>().GetComponent<RectTransform>();
            mission_rect.anchoredPosition = new Vector2(0, 30 + (-60 * count));
            mission_rect.offsetMax = new Vector2(0, mission_rect.offsetMax.y);
            mission_rect.offsetMin = new Vector2(0, mission_rect.offsetMin.y);
            count += 1;
        }

        RectTransform content_rect = MissionView.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        content_rect.offsetMin = new Vector2(content_rect.offsetMin.x, -60 * (count - 1));
    }

    public void onChangedAdventurerCount(object sender, EventArgs e)
    {
        int count = 1;
        foreach (GameObject adventurer in ((Guild)sender).Adventurers)
        {
            // TODO: Figure out why some missions are null in this list...
            //if (mission == null)
            //    continue;

            adventurer.transform.SetParent(AdventurerView.transform.GetChild(0).GetChild(0));
            RectTransform mission_rect = adventurer.GetComponent<RectTransform>();
            mission_rect.anchoredPosition = new Vector2(0, 30 + (-60 * count));
            mission_rect.offsetMax = new Vector2(0, mission_rect.offsetMax.y);
            mission_rect.offsetMin = new Vector2(0, mission_rect.offsetMin.y);
            count += 1;
        }

        RectTransform content_rect = AdventurerView.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        content_rect.offsetMin = new Vector2(content_rect.offsetMin.x, -60 * (count - 1));
    }
}
