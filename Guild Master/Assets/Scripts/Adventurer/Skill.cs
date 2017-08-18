using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Skill
{
    public string Name;
    public SkillType Type;

    public int Level;
    public int Experience;
    public int ExperienceForNextLevel;

    // The distance one can travel in one day.
    public double Distance;

    public double MaxUses;
    public double CurrentUses;

    public double LevelFactor;

    public Skill(string name, SkillType type, int experienceForNextLevel, double distance, double max_uses, double levelMultiplier)
    {
        Name = name;
        Type = type;
        Level = 0;
        Experience = 0;
        ExperienceForNextLevel = experienceForNextLevel;
        Distance = distance;
        MaxUses = max_uses;
        CurrentUses = MaxUses;
        LevelFactor = levelMultiplier;
    }

    public Skill(Skill s)
    {
        Name = s.Name;
        Type = s.Type;
        Level = 0;
        Experience = 0;
        ExperienceForNextLevel = s.ExperienceForNextLevel;
        Distance = s.Distance;
        MaxUses = s.MaxUses;
        CurrentUses = MaxUses;
        LevelFactor = s.LevelFactor;
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

/// <summary>
/// A list of all the skills by name.
/// </summary>
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

/// <summary>
/// The four skill types. Each has different specific uses.
/// </summary>
public enum SkillType
{
    Movement,
    Combat,
    Social,
    Perception
}

