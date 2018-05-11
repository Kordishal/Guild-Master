using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party {

    private List<Adventurer> _members;
    public List<Adventurer> Members { get { return _members; } }

    private LinkedListNode<Location> _current_location;
    public LinkedListNode<Location> CurrentLocation { get { return _current_location; } }

    private LinkedList<Location> _path;
    public LinkedList<Location> Path { set { _path = value; } }

    public void Move()
    {
        _current_location = _current_location.Next;
        foreach (var member in _members)
            member.Location = _current_location.Value;
    }


    public bool isCamping;
    public bool isResting;

    private bool _campIsSetUp;
    public bool CampIsSetUp { get { return _campIsSetUp; } }

    public void Rest()
    {

    }

    public void SetupCamp()
    {
        isCamping = true;
        _campIsSetUp = false;

        while (!CampIsSetUp)
        {
            switch (_current_location.Value.Type)
            {
                case LocationType.Building:
                    if (_current_location.Value.hasShelter)
                    {
                        
                    }    
                    else
                    {

                    }

                    break;
                case LocationType.Settlement:
                    break;
                case LocationType.Forest:
                    break;
                case LocationType.Mountain:
                    break;
                case LocationType.Ruin:
                    break;
            }
        }
    }

    public void Camp()
    {
        if (Calendar.CurrentPartOfDay == PartOfDay.Night)
        {
            
        }
        else
        {
            isCamping = false;
        }
    }

    public Travel GetFastestTravel()
    {
        // get all the movement stuff and figure out which one goes the fastest and the furthest. 
        double max_distance_walking = int.MaxValue;
        bool all_adventurers_can_still_walk = true;

        foreach (Adventurer a in _members)
        {
            if (a.Skills[(int)SkillNames.Walking].CurrentUses > 0)
            {
                if (a.Skills[(int)SkillNames.Walking].Distance < max_distance_walking)
                    max_distance_walking = a.Skills[(int)SkillNames.Walking].Distance;
            }
            else
                all_adventurers_can_still_walk = false;
                
        }


        if (all_adventurers_can_still_walk)
        {
            return new Travel(SkillNames.Walking, max_distance_walking);
        }
        else
        {
            return new Travel(SkillNames.Walking, 0);
        }
    }

    public void UseGroupSkill(SkillNames skill)
    {
        foreach (Adventurer a in _members)
            a.Skills[(int)skill].CurrentUses -= 1;
    }

    public struct Travel
    {
        public SkillNames SkillUsed;
        public double Distance;

        public Travel(SkillNames skillUsed, double distance)
        {
            SkillUsed = skillUsed;
            Distance = distance;
        }
    }

    public Party(List<Adventurer> adventurers)
    {
        _members = adventurers;
    }

    public override string ToString()
    {
        string print = "";
        foreach (Adventurer a in _members)
            print += a.Name + " | ";

        return print;
    }
}
