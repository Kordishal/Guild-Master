using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar : MonoBehaviour {

    private float total_real_time_passed;
    private float one_second;

    private int hours;
    private int total_hours;

    private int days;
    private int total_days;

    private int year;
    private int total_years;

    private Dictionary<string, List<int>> parts_of_day;
    private int nextDailyEvent;

    private int current_weekday;
    private string[] weekdays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

    private int current_month;
    private string[] months = { "January", "February", "March", "April", "Mai", "June", "July", "August", "September", "October", "November", "December" };

    private int current_season;
    private string[] seasons = { "Winter", "Spring", "Summer", "Fall" };


    public Text CurrentDate;

    private bool isPaused = false;
    public void onClick_Pause()
    {
        if (isPaused) isPaused = false;
        else isPaused = true;
    }

    public int getTick()
    {
        return total_hours;
    }

	// Use this for initialization
	void Start () {
        current_weekday = 0;
        current_month = 0;
        current_season = 0;

        hours = 9;
        total_hours = 0;

        days = 1;
        total_days = 0;

        year = 456;
        total_years = 0;

        total_real_time_passed = 0;
        one_second = 0;

        parts_of_day = new Dictionary<string, List<int>>();
        parts_of_day.Add("Morning", new List<int> { 6, 7, 8, 9, 10, 11 });
        parts_of_day.Add("Lunch", new List<int> { 12, 13 });
        parts_of_day.Add("Afternoon", new List<int> { 14, 15, 16, 17, 18 });
        parts_of_day.Add("Evening", new List<int> { 19, 20, 21, 22, 23 });
        parts_of_day.Add("Night", new List<int> { 24, 1, 2, 3, 4, 5 });

        nextDailyEvent = 12;

        updateSeason();
        //updatePartsOfDay();
        updateCurrentDate();
	}
	
	// Update is called once per frame
	void Update () {
        total_real_time_passed += Time.deltaTime;
        if (isPaused)
            return;

        one_second += Time.deltaTime;

        if (one_second > 0.5)
        {
            hours += 1;
            total_hours += 1;
            newHour(new EventArgs());

            if (nextDailyEvent == hours)
                onDailyEventTrigger(new EventArgs());

            if (hours == 25)
            {
                determineNextDailyEventTrigger();
                hours = 1;

                days += 1;
                total_days += 1;

                current_weekday += 1;
                if (current_weekday >= weekdays.Length)
                    current_weekday = 0;
                

                if (days == 31)
                {
                    days = 1;

                    current_month += 1;
                    if (current_month > months.Length)
                    {
                        current_month = 0;
                        year += 1;
                        total_years += 1;
                    }
                }
            }

            one_second = 0;

            //updatePartsOfDay();
            updateSeason();
            updateCurrentDate();
        }
	}

    private void updateCurrentDate()
    {
        CurrentDate.text = hours + "h - " + weekdays[current_weekday] + ", " + days + ". " + months[current_month] + " " + year + "; " + seasons[current_season];
    }
    private void updateSeason()
    {
        switch (current_month)
        {
            case 11:
            case 0:
            case 1:
                current_season = 0;
                break;
            case 2:
            case 3:
            case 4:
                current_season = 1;
                break;
            case 5:
            case 6:
            case 7:
                current_season = 2;
                break;
            case 8:
            case 9:
            case 10:
                current_season = 3;
                break;

        }
    }

    /*
    private void updatePartsOfDay()
    {
        if (parts_of_day["Morning"].Contains(hours))
            part_of_day = "Moring";
        else if (parts_of_day["Lunch"].Contains(hours))
            part_of_day = "Lunch";
        else if (parts_of_day["Afternoon"].Contains(hours))
            part_of_day = "Afternoon";
        else if (parts_of_day["Evening"].Contains(hours))
            part_of_day = "Evening";
        else if (parts_of_day["Night"].Contains(hours))
            part_of_day = "Night";
    }
    */


    private void determineNextDailyEventTrigger()
    {
        int chance = UnityEngine.Random.Range(0, 4);

        switch (chance)
        {
            case 0:
                nextDailyEvent = parts_of_day["Morning"][UnityEngine.Random.Range(0, parts_of_day["Morning"].Count)];
                break;
            case 1:
                nextDailyEvent = parts_of_day["Lunch"][UnityEngine.Random.Range(0, parts_of_day["Lunch"].Count)];
                break;
            case 2:
                nextDailyEvent = parts_of_day["Afternoon"][UnityEngine.Random.Range(0, parts_of_day["Afternoon"].Count)];
                break;
            case 3:
                nextDailyEvent = parts_of_day["Evening"][UnityEngine.Random.Range(0, parts_of_day["Evening"].Count)];
                break;
            case 4:
                nextDailyEvent = parts_of_day["Night"][UnityEngine.Random.Range(0, parts_of_day["Night"].Count)];
                break;
        }
    }


    public event EventHandler<EventArgs> dailyEventTrigger;
    protected virtual void onDailyEventTrigger(EventArgs e)
    {
        EventHandler<EventArgs> handler = dailyEventTrigger;
        if (handler != null)
            handler(this, e);

    }

    public event EventHandler<EventArgs> hourlyTrigger;
    protected virtual void newHour(EventArgs e)
    {
        EventHandler<EventArgs> handler = hourlyTrigger;
        if (handler != null)
            handler(this, e);
    }
}
