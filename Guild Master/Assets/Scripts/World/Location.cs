﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour {

    public string Name;
    public string Description;
    public LocationType Type;

    public bool hasShelter;

    public List<Adventurer> Adventurers;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public override bool Equals(object other)
    {
        return Name == ((Location)other).Name;
    }
}


public enum LocationType
{
    Settlement, 
    Building,
    Forest,
    Mountain,
    Ruin,
}
