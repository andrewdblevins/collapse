using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    public GameObject TilePrefab;

    public Transform TileContainer;

    List<List<GameObject>> tiles = new List<List<GameObject>>();

	void Start () {
		for (int i = 0; i < 15; i++) {
            for (int j = 0; j < 15; j++) {
                GameObject tile = GameObject.Instantiate(TilePrefab, new Vector3((j - i)*0.42f, (i + j) * 0.21f, (i + j) * 0.21f), Quaternion.identity, TileContainer);
                tile.name = "Tile( " + i + ", " + j + ")";
            }
        }
	}

}
