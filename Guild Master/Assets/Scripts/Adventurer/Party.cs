using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party {

    public List<Adventurer> Members;

    public LinkedListNode<Location> CurrentLocation;

    public bool isCamping;
    public bool isResting;

    public bool camp_is_set_up;

    public void rest()
    {

    }

    public void setupCamp()
    {
        isCamping = true;
        camp_is_set_up = false;

        while (!camp_is_set_up)
        {
            switch (CurrentLocation.Value.Type)
            {
                case LocationType.Building:
                    if (CurrentLocation.Value.hasShelter)
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

    public void camp()
    {
        if (Calendar.CurrentPartOfDay == PartOfDay.Night)
        {
            
        }
        else
        {
            isCamping = false;
        }
    }

    public Travel getFastestTravel()
    {
        // get all the movement stuff and figure out which one goes the fastest and the furthest. 
        double max_distance_walking = int.MaxValue;
        bool all_adventurers_can_still_walk = true;

        foreach (Adventurer a in Members)
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

    public void useGroupSkill(SkillNames skill)
    {
        foreach (Adventurer a in Members)
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
        Members = adventurers;
    }

    public override string ToString()
    {
        string print = "";
        foreach (Adventurer a in Members)
            print += a.Name + " | ";

        return print;
    }
}
