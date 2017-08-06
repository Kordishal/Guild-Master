using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerSkills : MonoBehaviour {

    public static List<Skill> AllSkills;

	// Use this for initialization
	void Start () {

        AllSkills = new List<Skill>();

        for (int i = 0; i < (int)SkillNames.LAST_ENTRY; i++)
        {
            AllSkills.Add(new Skill((SkillNames)System.Enum.GetValues(typeof(SkillNames)).GetValue(i), 0));
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public class Skill
    {
        public SkillNames Name;
        public int Level;
        public int throwDiceVs(int Difficulty)
        {
            return UnityEngine.Random.Range(0, 20) + Level - Difficulty;
        }

        public Skill(SkillNames name, int level)
        {
            Name = name;
            Level = level;
        }


    }

    public enum SkillNames
    {
        Walking,
        Investigate,
        Tracking,
        Perception,



        LAST_ENTRY
    }
}
