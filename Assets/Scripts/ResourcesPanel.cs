using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesPanel : MonoBehaviour
{
    //Resouces

    private float wood = 150f;
    private float iron = 0f;
    private float coal = 0f;
    private float gold = 0f;

    public Text woodText;
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
        UpdateWood(150f);
        UpdateIron(0f);
        UpdateCoal(0f);
        UpdateGold(0f);

    }

    // Update is called once per frame
    void Update()
    {
 
    }

    //Resources
    public float GetWood() {
        return wood;
    }

    public float GetIron() {
        return iron;
    }

    public float GetCoal() {
        return coal;
    }

    public float GetGold() {
        return gold;
    }


    //Resources

    public void UpdateWood(float amount) {
        wood += amount;
        UpdateWoodText(wood);

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

    public void UpdateGold(float amount) {
        gold += amount;
        UpdateGoldText(gold);
    }

    private void UpdateWoodText(float t) {
        woodText.text = t.ToString();
    }

    private void UpdateIronText(float t) {
        ironText.text = t.ToString();
    }

    private void UpdateCoalText(float t) {
        coalText.text = t.ToString();
    }

    private void UpdateGoldText(float t) {
        goldText.text = t.ToString();
    }


    public void UpdateAll(Vector4 delta) {
        UpdateWood(delta.x);
        UpdateIron(delta.y);
        UpdateCoal(delta.z);
        UpdateGold(delta.w);
    }

    public bool CanAfford(Vector4 cost) {
        Debug.Log("Can afford " + cost);
        return cost.x <= wood && cost.y <= iron && cost.z <= coal && cost.w <= gold;
    }
}