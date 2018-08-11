using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public Transform container;

    public void SetHeight(float height) {
        container.localPosition = new Vector3(0f, height, 0f);
    }
}
