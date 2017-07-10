using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Adventurer : MonoBehaviour {

    static private int ID = 0;
    public string Name;
    public int Level;

    public double Cost;
    public bool isSelected;
    public bool isAvailable;

    public Text NameLabel;
    public Text LevelLabel;
    public Text CostLabel;

    public Button AdventurerButton;

	// Use this for initialization
	void Start () {
        ID = ID + 1;
        Name = "Adventurer " + ID;
        Level = UnityEngine.Random.Range(1, 10);
        Cost = 0.1;

        NameLabel.text = Name;
        LevelLabel.text = Level.ToString();
        CostLabel.text = Cost.ToString("P2");

        isSelected = false;
        isAvailable = true;

        AdventurerButton.onClick.AddListener(onClicked);
    }
	
	// Update is called once per frame
	void Update () {
        adjustButtonColor();
	}

    public void onClicked()
    {
        // Only fire the event if the adventurer is available.
        if (isAvailable)
        { 
           onSelected(new EventArgs());
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


    public event EventHandler<EventArgs> OnSelected;
    protected virtual void onSelected(EventArgs e)
    {
        EventHandler<EventArgs> handler = OnSelected;
        if (handler != null)
            handler(this, e);

    }
}
