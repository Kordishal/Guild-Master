using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public string Name;
    public int Currency;


    public InputField PlayerInformation;
    public Text CurrencyField;

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
        PlayerInformation.onEndEdit.AddListener((value) => changeName(value));
    }

    // Update is called once per frame
    void Update()
    {
        CurrencyField.text = Currency.ToString();
    }
}
