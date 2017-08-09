using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerSkills : MonoBehaviour {

    public static Skill[] AllSkills;

	// Use this for initialization
	void Start () {

        AllSkills = new Skill[(int)SkillNames.LAST_ENTRY];

        // Movement Skills:

        AllSkills[(int)SkillNames.Walking] = new Skill("Walking", SkillType.Movement, 0, 0, 1, 100, 5, 0.01f);
        AllSkills[(int)SkillNames.Running] = new Skill("Running", SkillType.Movement, 0, 0, 1, 300, 0.2, 0.005f);
        AllSkills[(int)SkillNames.Riding] = new Skill("Horse Riding", SkillType.Movement, 0, 0, 1, 500, 1, 0.01f);
        AllSkills[(int)SkillNames.Teleportation] = new Skill("Teleporting", SkillType.Movement, 0, 0, 1, 500, 1, 0.1f);
        AllSkills[(int)SkillNames.PortalCreation] = new Skill("Portal Creation", SkillType.Movement, 0, 0, 1, 1000, 1, 0.1f);

        // Perception Skills:

        AllSkills[(int)SkillNames.Searching] = new Skill("Searching", SkillType.Perception, 0, 0, 1, 10, 0, 1);
        AllSkills[(int)SkillNames.Tracking] = new Skill("Tracking", SkillType.Perception, 0, 0, 1, 10, 0, 1);

        // Social Skills

        AllSkills[(int)SkillNames.Investigate] = new Skill("Tracking", SkillType.Social, 0, 0, 1, 10, 0, 1);

        // Combat Skills
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public class Skill
    {
        public string Name;
        public SkillType Type;

        public int Level;
        public int Experience;
        public int ExperienceForNextLevel;

        public double Distance;

        // The distance one can travel in one day.
        public double MaxUses;
        public double CurrentUses;
        public double LevelFactor;

        public Skill(string name, SkillType type, int level, int experience, int experienceForNextLevel, double distance, double max_uses, double levelMultiplier)
        {
            Name = name;
            Type = type;
            Level = level;
            Experience = experience;
            ExperienceForNextLevel = experienceForNextLevel;
            Distance = distance;
            MaxUses = max_uses;
            CurrentUses = MaxUses;
            LevelFactor = levelMultiplier;
        }

        public int throwDiceVs(int Difficulty)
        {
            return UnityEngine.Random.Range(0, 20) + Level - Difficulty;
        }

        public void addExperience(int experience)
        {
            Experience += experience;

            if (Experience >= ExperienceForNextLevel)
            {
                Experience = Experience - ExperienceForNextLevel;
                Level += 1;
                ExperienceForNextLevel = ExperienceForNextLevel + Level;

                switch (Type)
                {
                    case SkillType.Combat:
                        break;
                    case SkillType.Movement:
                        Distance = Distance * LevelFactor;
                        MaxUses = MaxUses * LevelFactor;
                        break;
                    case SkillType.Perception:
                        Distance += LevelFactor;
                        break;
                    case SkillType.Social:
                        break;
                }

            }
        }

        public void addLevels(int level)
        {
            for (int i = 0; i < level; i++)
            {
                Level += 1;
                ExperienceForNextLevel = ExperienceForNextLevel + Level;

                switch (Type)
                {
                    case SkillType.Combat:
                        Distance += LevelFactor;
                        break;
                    case SkillType.Movement:
                        Distance = Distance * LevelFactor;
                        MaxUses = MaxUses * LevelFactor;
                        break;
                    case SkillType.Perception:
                        Distance += LevelFactor;
                        break;
                    case SkillType.Social:
                        Distance += LevelFactor;
                        break;
                }
            }
        }

    }

    public enum SkillNames
    {
        // Movement
        Walking,
        Running,
        Riding,
        Teleportation,
        PortalCreation,

        // Perception
        Searching,
        Tracking,

        // Social
        Investigate,

        // Combat




        LAST_ENTRY,

    }

    public enum SkillType
    {
        Movement,
        Combat,
        Social,
        Perception
    }
}
