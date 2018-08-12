using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileContainer : MonoBehaviour {

    public Tile tile;

    public void OnMouseDown() {
        tile.Click();
    }

    public void OnMouseEnter() {
        tile.Highlight();
    }

    public void OnMouseExit() {
        tile.UnHighlight();
    }
}
