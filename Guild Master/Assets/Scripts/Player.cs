using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public string Name;
    public int Currency;
    public Text CurrencyField;

    // Use this for initialization
    void Start()
    {
        Name = "SoulLink";
        Currency = 1000;
    }

    // Update is called once per frame
    void Update()
    {
        CurrencyField.text = Currency.ToString();
    }
}
