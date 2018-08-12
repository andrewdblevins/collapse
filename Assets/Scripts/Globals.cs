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
    public float MineCost = 50f;
    public float HouseCost = 25f;
    public float StabilizerCost = 100f;
    public float FarmCost = 10f;
    public float DecayRate = 100f;
}
