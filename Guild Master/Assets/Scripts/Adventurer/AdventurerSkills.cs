using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerSkills : MonoBehaviour {

    public static Skill[] AllSkills;

	// Use this for initialization
	void Start () {

        AllSkills = new Skill[(int)SkillNames.LAST_ENTRY];

        // Movement Skills:

        AllSkills[(int)SkillNames.Walking] = new Skill("Walking", SkillType.Movement, 0, 0, 1, 100, 5, 1.01f);
        AllSkills[(int)SkillNames.Running] = new Skill("Running", SkillType.Movement, 0, 0, 1, 300, 0.2, 1.005f);
        AllSkills[(int)SkillNames.Riding] = new Skill("Horse Riding", SkillType.Movement, 0, 0, 1, 500, 1, 1.01f);
        AllSkills[(int)SkillNames.Teleportation] = new Skill("Teleporting", SkillType.Movement, 0, 0, 1, 500, 1, 1.1f);
        AllSkills[(int)SkillNames.PortalCreation] = new Skill("Portal Creation", SkillType.Movement, 0, 0, 1, 1000, 1, 1.1f);

        // Perception Skills:

        AllSkills[(int)SkillNames.Searching] = new Skill("Searching", SkillType.Perception, 0, 0, 1, 10, 0, 1);
        AllSkills[(int)SkillNames.Tracking] = new Skill("Tracking", SkillType.Perception, 0, 0, 1, 10, 0, 1);

        // Social Skills

        AllSkills[(int)SkillNames.Investigate] = new Skill("Investigate", SkillType.Social, 0, 0, 1, 10, 0, 1);

        // Combat Skills


        RectTransform rect_cont = GameObject.Find("AdventurerSkillsContent").GetComponent<RectTransform>();
        rect_cont.position = new Vector3(rect_cont.position.x, rect_cont.position.y, 0);
        rect_cont.offsetMin = new Vector2(rect_cont.offsetMin.x, rect_cont.offsetMin.y - 16 * (AllSkills.Length));

        RectTransform rect = GameObject.Find("AdventurerSkills").GetComponent<RectTransform>();
        rect.position = new Vector3(rect.position.x, rect.position.y, 0);
        rect.offsetMin = new Vector2(rect.offsetMin.x, rect.offsetMin.y - 16 * (AllSkills.Length));
    }
	
	// Update is called once per frame
	void Update () {
		
	}


}
