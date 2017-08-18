using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Adventurer : MonoBehaviour {

    public string Name;
    public int Level;

    public List<Skill> Skills;

    public Location Location;

    public double Cost;

    public bool isSelected;
    public bool isAvailable;

    public Text NameLabel;
    public Text LevelLabel;
    public Text CostLabel;

    public Guild guild;

	// Use this for initialization
	void Start () {
        guild = GameObject.Find("Guild").GetComponent<Guild>();



        // Increases the space used by the content panel of the viewport. This is necessary as otherwise the viewport will not detect when the buttons will overflow its view.


        NameLabel.text = Name;
        LevelLabel.text = Level.ToString();
        CostLabel.text = Cost.ToString("P2");

        isSelected = false;
        isAvailable = true; 
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
            GetComponent<Button>().image.color = Color.blue;
        else if (!isAvailable)
            GetComponent<Button>().image.color = Color.gray;
        else
            GetComponent<Button>().image.color = Color.green;
    }


}
