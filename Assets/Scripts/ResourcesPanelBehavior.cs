﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesPanelBehavior : MonoBehaviour
{

    public Text ironText;
    public Text coalText;
    public Text goldText;

    // Use this for initialization
    void Start()
    {
        updateIron(100);
        updateCoal(40);
        updateGold(100);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateIron(float t)
    {
        ironText.text = t.ToString();
    }

    public void updateCoal(float t)
    {
        coalText.text = t.ToString();
    }

    public void updateGold(float t)
    {
        goldText.text = t.ToString();
    }

}