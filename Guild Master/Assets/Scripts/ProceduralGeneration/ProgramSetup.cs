using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The beginning of the program.
/// TODO: Add creation of the guild, missions, world etc. to here and related files.
/// 
/// </summary>
public class ProgramSetup : MonoBehaviour
{
    public Guild Guild;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Begin Game Setup");

        Guild.addAdventurers(GetComponent<GenerateAdventurers>().generateStartUpAdventurers());
        Guild.addMissions(GetComponent<GenerateMissions>().generateStartUpMissions());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
