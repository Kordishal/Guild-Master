using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar : MonoBehaviour {

    private static float total_real_time_passed;
    private static float time_passed;

    public static int Hour;
    private static int total_hours;

    private static int days;
    private static int total_days;

    private static int year;
    private static int total_years;

    public static PartOfDay CurrentPartOfDay;
    private static Dictionary<PartOfDay, List<int>> parts_of_day;


    private static int next_daily_event;

    private static int current_weekday;
    private static string[] weekdays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

    private static int current_month;
    private static string[] months = { "January", "February", "March", "April", "Mai", "June", "July", "August", "September", "October", "November", "December" };

    private static int current_season;
    private static string[] seasons = { "Winter", "Spring", "Summer", "Fall" };

    public static bool GamePaused;

    public int getTick()
    {
        return total_hours;
    }

	// Use this for initialization
	void Start () {
        current_weekday = 0;
        current_month = 0;
        current_season = 0;

        Hour = 9;
        total_hours = 0;

        days = 1;
        total_days = 0;

        year = 456;
        total_years = 0;

        total_real_time_passed = 0;
        time_passed = 0;

        parts_of_day = new Dictionary<PartOfDay, List<int>>();
        parts_of_day.Add(PartOfDay.Morning, new List<int> { 6, 7, 8, 9, 10, 11 });
        parts_of_day.Add(PartOfDay.Lunch, new List<int> { 12, 13 });
        parts_of_day.Add(PartOfDay.Afternoon, new List<int> { 14, 15, 16, 17, 18 });
        parts_of_day.Add(PartOfDay.Evening, new List<int> { 19, 20, 21, 22, 23 });
        parts_of_day.Add(PartOfDay.Night, new List<int> { 24, 1, 2, 3, 4, 5 });

        // the first daily event is always at 12 o'clock.
        next_daily_event = 12;

        determine_season();

        GamePaused = false;
	}
	
	// Update is called once per frame
	void Update () {
        // Store the total time played...
        total_real_time_passed += Time.deltaTime;

        if (GamePaused)
            return;

        time_passed += Time.deltaTime;

        if (time_passed > 0.5)
        {
            Hour += 1;
            total_hours += 1;
            determine_part_of_day();
            fire_advance_hour_trigger(new EventArgs());

            if (next_daily_event == Hour)
                fire_daily_event_trigger(new EventArgs());

            if (Hour == 25)
            {
                set_next_daily_event_hour();
                Hour = 1;

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

            time_passed = 0;
            determine_season();
        }
	}

    /// <summary>
    /// Returns the current date & time in human readeable form.
    /// </summary>
    public static string getDateTime()
    {
        return Hour + "h - " + weekdays[current_weekday] + ", " + days + ". " + months[current_month] + " " + year + "; " + seasons[current_season];
    }

    /// <summary>
    /// What season it currently is. Depends on the month.
    /// </summary>
    private void determine_season()
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

    private void determine_part_of_day()
    {
        if (parts_of_day[PartOfDay.Morning].Contains(Hour))
            CurrentPartOfDay = PartOfDay.Morning;
        else if (parts_of_day[PartOfDay.Lunch].Contains(Hour))
            CurrentPartOfDay = PartOfDay.Lunch;
        else if (parts_of_day[PartOfDay.Afternoon].Contains(Hour))
            CurrentPartOfDay = PartOfDay.Afternoon;
        else if (parts_of_day[PartOfDay.Evening].Contains(Hour))
            CurrentPartOfDay = PartOfDay.Evening;
        else if (parts_of_day[PartOfDay.Night].Contains(Hour))
            CurrentPartOfDay = PartOfDay.Night;
    }



    /// <summary>
    /// Sets at random when the next daily event will be triggered.
    /// </summary>
    private void set_next_daily_event_hour()
    {
        int chance = UnityEngine.Random.Range(0, 4);

        switch (chance)
        {
            case 0:
                next_daily_event = parts_of_day[PartOfDay.Morning][UnityEngine.Random.Range(0, parts_of_day[PartOfDay.Morning].Count)];
                break;
            case 1:
                next_daily_event = parts_of_day[PartOfDay.Lunch][UnityEngine.Random.Range(0, parts_of_day[PartOfDay.Lunch].Count)];
                break;
            case 2:
                next_daily_event = parts_of_day[PartOfDay.Afternoon][UnityEngine.Random.Range(0, parts_of_day[PartOfDay.Afternoon].Count)];
                break;
            case 3:
                next_daily_event = parts_of_day[PartOfDay.Evening][UnityEngine.Random.Range(0, parts_of_day[PartOfDay.Evening].Count)];
                break;
            case 4:
                next_daily_event = parts_of_day[PartOfDay.Night][UnityEngine.Random.Range(0, parts_of_day[PartOfDay.Night].Count)];
                break;
        }
    }


    public event EventHandler<EventArgs> dailyEventTrigger;
    protected virtual void fire_daily_event_trigger(EventArgs e)
    {
        EventHandler<EventArgs> handler = dailyEventTrigger;
        if (handler != null)
            handler(this, e);

    }

    public event EventHandler<EventArgs> hourlyTrigger;
    protected virtual void fire_advance_hour_trigger(EventArgs e)
    {
        EventHandler<EventArgs> handler = hourlyTrigger;
        if (handler != null)
            handler(this, e);
    }
}

public enum PartOfDay
{
    Morning,
    Lunch,
    Afternoon,
    Evening,
    Night,
}
