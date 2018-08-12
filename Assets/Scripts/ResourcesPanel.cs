using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesPanel : MonoBehaviour
{
    //Resouces
    private float gold = 10f;
    private float iron = 50f;
    private float coal = 100f;

    public Text ironText;
    public Text coalText;
    public Text goldText;
    public static ResourcesPanel Instance { get; private set; }

    void Awake()
    {

        // First we check if there are any other instances conflicting
        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        Instance = this;

    }
    // Use this for initialization
    void Start()
    {
        UpdateIron(100);
        UpdateCoal(40);
        UpdateGold(100);

    }

    // Update is called once per frame
    void Update()
    {
 
    }

    //Resources
    public float GetGold() {
        return gold;
    }

    public float GetIron() {
        return iron;
    }

    public float GetCoal() {
        return coal;
    }

    //Resources
    public void UpdateGold(float amount)
    {
        gold += amount;
        UpdateGoldText(gold);
    }

    public void UpdateIron(float amount)
    {
        iron += amount;
        UpdateIronText(iron);
    }


    public void UpdateCoal(float amount)
    {
        coal += amount;
        UpdateCoalText(coal);

    }

    private void UpdateIronText(float t)
    {
        ironText.text = t.ToString();
    }

    private void UpdateCoalText(float t)
    {
        coalText.text = t.ToString();
    }

    private void UpdateGoldText(float t)
    {
        goldText.text = t.ToString();
    }

}