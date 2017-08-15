using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handels all the changes to the status bar.
/// </summary>
public class StatusBarHandler : MonoBehaviour {

    public Text CurrentDate;
    public Button Pause;

    public void onClickedPauseButton()
    {
        if (Calendar.GamePaused) Calendar.GamePaused = false;
        else Calendar.GamePaused = true;
    }

    public Text CurrentWealth;

	// Use this for initialization
	void Start () {
        CurrentDate.text = Calendar.getDateTime();
	}
	
	// Update is called once per frame
	void Update () {
        CurrentDate.text = Calendar.getDateTime();

        CurrentWealth.text = Guild.CurrentWealth.ToString();
	}
}
