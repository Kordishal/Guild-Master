using System.Collections.Generic;

public class MissionContent
{
    static MissionContent()
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


    public enum Category
    {
        Item,
        Animal,
        Plant,
        Person,
    }
}

