using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Adventurer : MonoBehaviour {

    private string _name;
    public string Name { get { return _name; } }

    private int _level;
    public int Level { get { return _level; } set { _level = value; } }

    private List<Skill> _skills;
    public List<Skill> Skills { get { return _skills; } set { _skills = value; } }

    private Location _location;
    public Location Location { get { return _location; } set { _location = value; } }

    private double _cost;
    public double Cost { get { return _cost; } set { _cost = value; } }

    public void Initialize(string name, int level, List<Skill> skills, Location location, double cost)
    {
        _name = name;
        _level = level;
        _skills = skills;
        _location = location;
        _cost = cost;
    }


    public bool isSelected;
    public bool isAvailable;

    public Text NameLabel;
    public Text LevelLabel;
    public Text CostLabel;

    private Guild _guild;
    public Guild Guild { get { return _guild; } set { _guild = value; } }

	void Start () {
        _guild = GameObject.Find("Guild").GetComponent<Guild>();

        NameLabel.text = Name;
        LevelLabel.text = Level.ToString();
        CostLabel.text = Cost.ToString("P2");

        isSelected = false;
        isAvailable = true; 
    }

    void Update()
    {
        AdjustButtonColor();
    }

    public void OnClicked()
    {
        // Only fire the event if the adventurer is available.
        if (isAvailable && _guild.SelectedMission != null)
        {
            // When the adventurer is already selected always deselect them.
            if (isSelected)
            {
                _guild.SelectedAdventurers.Remove(gameObject);
                isSelected = false;
            }
            else if (_guild.getSelectedMission.MaxAdventurers > _guild.SelectedAdventurers.Count)
            {
                // when there is still space make the adventurer part of the party.
                _guild.SelectedAdventurers.Add(gameObject);
                isSelected = true;
            }
            // Otherwise do nothing.
        }
    }

    private void AdjustButtonColor()
    {
        if (isSelected)
            GetComponent<Button>().image.color = Color.blue;
        else if (!isAvailable)
            GetComponent<Button>().image.color = Color.gray;
        else
            GetComponent<Button>().image.color = Color.green;
    }


}
