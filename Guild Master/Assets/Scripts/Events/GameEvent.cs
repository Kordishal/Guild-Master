using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameEvent : MonoBehaviour
{ 
    public GameEventDatabase.EventFrequencyType Frequency;
    public GameEventDatabase.EventType Type;
    public int Weight;
    public UnityAction AfterEffect;


    public Image EventImage;
    public Text EventTitle;
    public Text EventDescription;
    public Button EventButton;

	// Use this for initialization
	void Start () {
        // Put to dialog box into the center of the screen.
        transform.SetParent(GameObject.Find("OverlayCanvas").transform);
        RectTransform rect_dialog = GetComponent<RectTransform>();
        rect_dialog.anchoredPosition = new Vector2(0, 0);

        EventTitle = transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        EventImage = transform.GetChild(1).gameObject.GetComponent<Image>();
        EventDescription = transform.GetChild(2).gameObject.GetComponent<Text>();
        EventButton = transform.GetChild(3).gameObject.GetComponent<Button>();
        chooseEvent();
    }


    private void chooseEvent()
    {
        int total_weight = 0;
        for (int i = 0; i < GameEventDatabase.GAME_EVENTS.Count; i++)
        {
            total_weight += GameEventDatabase.GAME_EVENTS[i].Weight;
        }

        int dice_result = UnityEngine.Random.Range(0, total_weight);

        int current_weight = 0;
        int previous_weight = 0;
        GameEventDatabase.GameEventImplementation chosen_game_event = null;
        for (int i = 0; i < GameEventDatabase.GAME_EVENTS.Count; i++)
        {
            current_weight += GameEventDatabase.GAME_EVENTS[i].Weight;
            if (dice_result >= previous_weight && current_weight > dice_result)
            {
                chosen_game_event = GameEventDatabase.GAME_EVENTS[i];
                break;
            }
            previous_weight += GameEventDatabase.GAME_EVENTS[i].Weight;
        }

        Frequency = chosen_game_event.FrequencyType;
        Type = chosen_game_event.Type;
        Weight = chosen_game_event.Weight;
        AfterEffect = chosen_game_event.AfterEffect;

        EventTitle.text = chosen_game_event.EventTitle;
        EventImage = Resources.Load(chosen_game_event.EventImagePath) as Image;
        EventDescription.text = chosen_game_event.EventDescription;

        chosen_game_event.ImmediateEffect();

        EventButton.onClick.AddListener(AfterEffect);
    }

    // Update is called once per frame
    void Update () {
		
	}


    public void OnClick_Ok()
    {
        Destroy(gameObject, 0.1f);
    }
}
