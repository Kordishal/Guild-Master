using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Each mission is made up of stages. The adventurers then need to solve each stage in order to complete the mission.
/// </summary>
public class Stage
{
    public StageNames Name;
    public string DisplayName;
    public int Difficulty;
    public StageAction Action;
    public int Repeatability;
    public int Repeated = 0;
    public double DistanceTraveled;
    public FinishState FinishedWith = FinishState.None;

    public Stage(StageNames name, string display_name, int difficulty, StageAction action, int repeatability)
    {
        Name = name;
        DisplayName = display_name;
        Difficulty = difficulty;
        Action = action;
        Repeatability = repeatability;
    }

    public enum FinishState
    {
        CriticalFailure,
        Failure,
        None,
        Success,
        CriticalSuccess
    }

    public delegate void StageAction(Mission mission);
}


/// <summary>
/// A list of all the possible stages by name.
/// </summary>
public enum StageNames
{
    GoToDestination,
    ReturnToGuildHall,
    RetrieveTarget,

    MAX_STAGES
}
