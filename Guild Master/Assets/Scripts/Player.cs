using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public string Name;
    public int Currency;

    public int[] CurrentTime;

    public InputField PlayerInformation;
    public Text CurrencyField;
    public Text TimeField;

    private void changeName(string name)
    {
        Name = name;
        PlayerInformation.readOnly = true;
    }

    // Use this for initialization
    void Start()
    {
        Name = "PlaceHolder";
        Currency = 1000;
        CurrentTime = new int[3] { 1, 1, 1 }; // Days/Hours/Minutes
        TimeField.text = CurrentTime[0] + "d " + CurrentTime[1] + "h " + CurrentTime[2] + "m ";

        PlayerInformation.onEndEdit.AddListener((value) => changeName(value));
    }

    // Update is called once per frame
    void Update()
    {
        CurrencyField.text = Currency.ToString();
        UpdateGameClock();
    }

    static private double real_time_elapsed;
    void UpdateGameClock()
    {
        real_time_elapsed += Time.deltaTime;
        if (real_time_elapsed >= 1)
        {
            CurrentTime[2] += 1;
            if (CurrentTime[2] == 60)
            {
                CurrentTime[1] += 1;
                CurrentTime[2] = 1;
                if (CurrentTime[1] == 24)
                {
                    CurrentTime[0] += 1;
                    CurrentTime[1] = 1;
                }
            }
            TimeField.text = CurrentTime[0] + "d " + CurrentTime[1] + "h " + CurrentTime[2] + "m ";
            real_time_elapsed = 0;
        }
    }
}
