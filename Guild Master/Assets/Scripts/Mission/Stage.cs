using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Each mission is made up of stages. The adventurers then need to solve each stage in order to complete the mission.
/// </summary>
public class Stage
{
    /// <summary>
    /// 
    /// </summary>
    public StageNames Name;
    public string DisplayName;
    public StageAction Action;
    public int Repeatability;
    public int Repeated = 0;

    public FinishState FinishedWith;

    // TARGET STAGES ONLY.
    public int Difficulty;
    public Target Target;

    // MOVEMENT STAGES ONLY.
    public double DistanceTraveled;
    public LinkedList<Location> PathToTargetLocation;

    public Stage()
    {
        DistanceTraveled = 0;
        Repeated = 0;
        FinishedWith = FinishState.None;
    }

    public Stage(StageNames name, string display_name, int difficulty, StageAction action, int repeatability)
    {
        Name = name;
        DisplayName = display_name;
        Difficulty = difficulty;
        Action = action;
        Repeatability = repeatability;
    }

    public Stage(Stage stage)
    {
        Name = stage.Name;
        DisplayName = stage.DisplayName;
        Difficulty = stage.Difficulty;
        Action = stage.Action;
        Repeatability = stage.Repeatability;
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
    MoveToTarget,
    ReturnToGuildHall,
    RetrieveTarget,

    MAX_STAGES
}
