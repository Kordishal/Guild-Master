using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameEvent : MonoBehaviour {

    private static int event_count;
    private int identifier;

    private Guild guild;

    public GameEventDatabase.EventFrequencyType Frequency;
    public GameEventDatabase.EventType Type;
    public int Weight;
    public UnityAction Effect;


    public Image EventImage;
    public Text EventTitle;
    public Text EventDescription;
    public Button EventButton;

    public GameObject PrefabDialogBox;

    private GameObject InstantiatedDialogBox;

	// Use this for initialization
	void Start () {
        identifier = event_count;
        event_count += 1;

        guild = GameObject.Find("Guild").GetComponent<Guild>();

        InstantiatedDialogBox = Instantiate(PrefabDialogBox);
        InstantiatedDialogBox.transform.SetParent(GameObject.Find("OverlayCanvas").transform);
        RectTransform rect_dialog = InstantiatedDialogBox.GetComponent<RectTransform>();
        rect_dialog.anchoredPosition = new Vector2(0, 0);

        EventTitle = InstantiatedDialogBox.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        EventImage = InstantiatedDialogBox.transform.GetChild(1).gameObject.GetComponent<Image>();
        EventDescription = InstantiatedDialogBox.transform.GetChild(2).gameObject.GetComponent<Text>();
        EventButton = InstantiatedDialogBox.transform.GetChild(3).gameObject.GetComponent<Button>();

        EventButton.onClick.AddListener(OnClick_Ok);

        GameEventDatabase.GameEventImplementation game_event = GameEventDatabase.GAME_EVENTS[0];

        Frequency = game_event.FrequencyType;
        Type = game_event.Type;
        Weight = game_event.Weight;
        Effect = game_event.Effect;

        EventTitle.text = game_event.EventTitle;
        EventImage = Resources.Load(game_event.EventImagePath) as Image;
        EventDescription.text = game_event.EventDescription;

        EventButton.onClick.AddListener(Effect);
    }

    // Update is called once per frame
    void Update () {
		
	}


    public void OnClick_Ok()
    {
        Destroy(InstantiatedDialogBox, 0.1f);
    }
}
