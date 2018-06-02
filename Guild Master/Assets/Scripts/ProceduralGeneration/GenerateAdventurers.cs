using System.Collections.Generic;
using UnityEngine;

public class GenerateAdventurers : MonoBehaviour
{

    public GameObject AdventurerPrefab;
    public List<GameObject> GenerateRandormStartUpAdventurers(int n)
    {
        var adventurers = new List<GameObject>();
        for (int i = 0; i < n; i++)
        {
            GameObject adventurer = Instantiate(AdventurerPrefab);

            var a = adventurer.GetComponent<Adventurer>();
            var skills = new List<Skill>();

            for (int j = 0; j < skill_list.Length; j++)
            {
                skills.Add(new Skill(skill_list[j]));
                skills[j].addLevels(Random.Range(0, 5));
            }

            a.Initialize(names[Random.Range(0, names.Count)], 1, skills, World.GuildHall, 0.1);
            World.GuildHall.Adventurers.Add(a);
            adventurers.Add(adventurer);
        }
        return adventurers;
    }   


    void Awake()
    {

        Debug.Log("Generate Adventurer Data");
        skill_list = new Skill[(int)SkillNames.LAST_ENTRY];

        // Movement Skills:

        skill_list[(int)SkillNames.Walking] = new Skill("Walking", SkillType.Movement, 1, 100, 5, 1.01f);
        skill_list[(int)SkillNames.Running] = new Skill("Running", SkillType.Movement, 1, 300, 0.2, 1.005f);
        skill_list[(int)SkillNames.Riding] = new Skill("Horse Riding", SkillType.Movement, 1, 500, 1, 1.01f);
        skill_list[(int)SkillNames.Teleportation] = new Skill("Teleporting", SkillType.Movement, 1, 500, 1, 1.1f);
        skill_list[(int)SkillNames.PortalCreation] = new Skill("Portal Creation", SkillType.Movement, 1, 1000, 1, 1.1f);

        // Perception Skills:

        skill_list[(int)SkillNames.Searching] = new Skill("Searching", SkillType.Perception, 1, 10, 0, 1);
        skill_list[(int)SkillNames.Tracking] = new Skill("Tracking", SkillType.Perception, 1, 10, 0, 1);

        // Social Skills

        skill_list[(int)SkillNames.Investigate] = new Skill("Investigate", SkillType.Social, 1, 10, 0, 1);

        // Combat Skills
    }

    void Start()
    {

        RectTransform rect_cont = GameObject.Find("AdventurerSkillsContent").GetComponent<RectTransform>();
        rect_cont.position = new Vector3(rect_cont.position.x, rect_cont.position.y, 0);
        rect_cont.offsetMin = new Vector2(rect_cont.offsetMin.x, rect_cont.offsetMin.y - 16 * (skill_list.Length));

        RectTransform rect = GameObject.Find("AdventurerSkills").GetComponent<RectTransform>();
        rect.position = new Vector3(rect.position.x, rect.position.y, 0);
        rect.offsetMin = new Vector2(rect.offsetMin.x, rect.offsetMin.y - 16 * (skill_list.Length));
    }

    private Skill[] skill_list;

    private List<string> names = new List<string>() { "Abrielle", "Adair", "Adara", "Adriel", "Aiyana", "Alissa", "Alixandra", "Altair", "Amara", "Anatola", "Anya",
        "Arcadia", "Ariadne", "Arianwen", "Aurelia", "Aurelian", "Aurelius", "Avalon", "Acalia", "Alaire", "Auristela", "Bastian", "Breena", "Brielle", "Briallan",
        "Briseis", "Cambria", "Cara", "Carys", "Caspian", "Cassia", "Cassiel", "Cassiopeia", "Cassius", "Chaniel", "Cora", "Corbin", "Cyprian", "Daire", "Darius",
        "Destin", "Drake", "Drystan", "Dagen", "Devlin", "Devlyn", "Eira", "Eirian", "Elysia", "Eoin", "Evadne", "Eliron", "Evanth", "Fineas", "Finian", "Fyodor",
        "Gareth", "Gavriel", "Griffin", "Guinevere", "Gaerwn", "Ginerva", "Hadriel", "Hannelore", "Hermione", "Hesperos", "Iagan", "Ianthe", "Ignacia", "Ignatius",
        "Iseult", "Isolde", "Jessalyn", "Kara", "Kerensa", "Korbin", "Kyler", "Kyra", "Katriel", "Kyrielle", "Leala", "Leila", "Lilith", "Liora", "Lucien", "Lyra",
        "Leira", "Liriene", "Liron", "Maia", "Marius", "Mathieu", "Mireille", "Mireya", "Maylea", "Meira", "Natania", "Nerys", "Nuriel", "Nyssa", "Neirin", "Nyfain",
        "Oisin", "Oralie", "Orion", "Orpheus", "Ozara", "Oleisa", "Orinthea", "Peregrine", "Persephone", "Perseus", "Petronela", "Phelan", "Pryderi", "Pyralia", "Pyralis",
        "Qadira", "Quintessa", "Quinevere", "Raisa", "Remus", "Rhyan", "Rhydderch", "Riona", "Renfrew", "Saoirse", "Sarai", "Sebastian", "Seraphim", "Seraphina", "Sirius",
        "Sorcha", "Saira", "Sarielle", "Serian", "Séverin", "Tavish", "Tearlach", "Terra", "Thalia", "Thaniel", "Theia", "Torian", "Torin", "Tressa", "Tristana", "Uriela",
        "Urien", "Ulyssia", "Vanora", "Vespera", "Vasilis", "Xanthus", "Xara", "Xylia", "Yadira", "Yseult", "Yakira", "Yeira", "Yeriel", "Yestin", "Zaira", "Zephyr", "Zora",
        "Zorion", "Zaniel", "Zarek" };
}
