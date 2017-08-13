using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Adventurer : MonoBehaviour {

    static private int ID = 0;
    public string Name;
    public int Level;

    public List<AdventurerSkills.Skill> Skills;

    public Location Location;

    public double Cost;
    public bool isSelected;
    public bool isAvailable;

    public Text NameLabel;
    public Text LevelLabel;
    public Text CostLabel;

    public Button AdventurerButton;

    private Guild guild;

	// Use this for initialization
	void Start () {
        guild = GameObject.Find("Guild").GetComponent<Guild>();
        guild.Adventurers.Add(gameObject);

        AdventurerButton.transform.SetParent(guild.AdventurerView.transform.GetChild(0).GetChild(0));
        RectTransform butten_rect = AdventurerButton.GetComponent<RectTransform>();
        butten_rect.anchoredPosition = new Vector2(0, 30 + (-60 * guild.Adventurers.Count));
        butten_rect.offsetMax = new Vector2(0, butten_rect.offsetMax.y);
        butten_rect.offsetMin = new Vector2(0, butten_rect.offsetMin.y);

        // Increases the space used by the content panel of the viewport. This is necessary as otherwise the viewport will not detect when the buttons will overflow its view.
        RectTransform content_rect = guild.AdventurerView.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        content_rect.offsetMin = new Vector2(content_rect.offsetMin.x, content_rect.offsetMin.y - 60);

        ID = ID + 1;
        Name = "Adventurer " + ID;
        Level = UnityEngine.Random.Range(1, 10);
        Cost = 0.1;

        NameLabel.text = Name;
        LevelLabel.text = Level.ToString();
        CostLabel.text = Cost.ToString("P2");

        isSelected = false;
        isAvailable = true;

        Skills = new List<AdventurerSkills.Skill>(AdventurerSkills.AllSkills);

        foreach (AdventurerSkills.Skill skill in Skills)
            skill.addLevels(UnityEngine.Random.Range(1, 10));

        Location = World.GuildHall;
        Location.Adventurers.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        adjustButtonColor();
    }

    public void onClicked()
    {
        // Only fire the event if the adventurer is available.
        if (isAvailable && guild.SelectedMission != null)
        {
            // When the adventurer is already selected always deselect them.
            if (isSelected)
            {
                guild.SelectedAdventurers.Remove(gameObject);
                isSelected = false;
            }
            else if (guild.getSelectedMission.MaxAdventurers > guild.SelectedAdventurers.Count)
            {
                // when there is still space make the adventurer part of the party.
                guild.SelectedAdventurers.Add(gameObject);
                isSelected = true;
            }
            // Otherwise do nothing.
        }
    }

    private void adjustButtonColor()
    {
        if (isSelected)
            AdventurerButton.image.color = Color.blue;
        else if (!isAvailable)
            AdventurerButton.image.color = Color.gray;
        else
            AdventurerButton.image.color = Color.green;
    }


}
