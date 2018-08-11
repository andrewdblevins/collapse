using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileContainer : MonoBehaviour {

    public Tile tile;

    public void OnMouseDown() {
        tile.OnMouseDown();
    }

    public void OnMouseEnter() {
        tile.OnMouseEnter();
    }

    public void OnMouseExit() {
        tile.OnMouseExit();
    }
}
