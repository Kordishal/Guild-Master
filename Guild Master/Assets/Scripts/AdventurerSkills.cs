using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerSkills : MonoBehaviour {

    public static Skill[] AllSkills;

	// Use this for initialization
	void Start () {

        AllSkills = new Skill[(int)SkillNames.LAST_ENTRY];

        // Movement Skills:

        AllSkills[(int)SkillNames.Walking] = new Skill("Walking", SkillType.Movement, 0, 0, 1, 100, 500, 0.01f);
        AllSkills[(int)SkillNames.Running] = new Skill("Running", SkillType.Movement, 0, 0, 1, 300, 20, 0.005f);
        AllSkills[(int)SkillNames.Riding] = new Skill("Horse Riding", SkillType.Movement, 0, 0, 1, 500, 500, 0.01f);
        AllSkills[(int)SkillNames.Teleportation] = new Skill("Teleporting", SkillType.Movement, 0, 0, 1, 10, 10, 0.1f);
        AllSkills[(int)SkillNames.PortalCreation] = new Skill("Portal Creation", SkillType.Movement, 0, 0, 1, 10, 10, 0.1f);
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

        public double PrimaryValue;
        public double SecondaryValue;
        public double LevelMultiplier;

        public Skill(string name, SkillType type, int level, int experience, int experienceForNextLevel, double primaryValue, double secondaryValue, double levelMultiplier)
        {
            Name = name;
            Type = type;
            Level = level;
            Experience = experience;
            ExperienceForNextLevel = experienceForNextLevel;
            PrimaryValue = primaryValue;
            SecondaryValue = secondaryValue;
            LevelMultiplier = levelMultiplier;
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
                PrimaryValue += PrimaryValue * LevelMultiplier;
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
        Vision,
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
