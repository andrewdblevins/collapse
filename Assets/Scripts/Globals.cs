using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour {

    public static Globals Instance { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        Instance = this;
    }

    public float GoldMultiplier = 3f;
    public float IronMultiplier = 3f;
    public float CoalMultiplier = 3f;

    //public float MineCost = 50f;
    //public float HouseCost = 25f;
    //public float StabilizerCost = 100f;
    //public float FarmCost = 10f;

    public Vector4 SawCost = new Vector4(25f, 0f, 0f, 0f);
    public Vector4 MineCost = new Vector4(50f, 0f, 0f, 0f);
    public Vector4 HouseCost = new Vector4(25f, 0f, 0f, 0f);
    public Vector4 StabilizerCost = new Vector4(50f, 50f, 0f, 0f);

    public Vector4 Saw2Cost = new Vector4(0f, 25f, 0f, 0f);
    public Vector4 Mine2Cost = new Vector4(0f, 25f, 25f, 0f);
    public Vector4 House2Cost = new Vector4(10f, 10f, 10f, 0f);
    public Vector4 Stabilizer2Cost = new Vector4(0f, 0f, 25f, 25f);

    public float DecayRate = 100f;
}
