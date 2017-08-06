using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour {

    public string Name;
    public string Description;

    public List<Adventurer> Adventurers;

    // Use this for initialization
    void Start () {
        Adventurers = new List<Adventurer>();
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
