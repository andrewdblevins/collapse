using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    public GameObject TilePrefab;

    public Transform TileContainer;

    List<List<Tile>> Tiles = new List<List<Tile>>();

    private const int width = 15;

	void Start () {
		for (int i = 0; i < width; i++) {
            Tiles.Add(new List<Tile>());
            for (int j = 0; j < width; j++) {
                GameObject tileObj = GameObject.Instantiate(TilePrefab, TileContainer, false);
                tileObj.transform.localPosition = new Vector3((j - i) * 0.42f, (i + j) * 0.21f, (i + j) * 0.21f);
                //GameObject tileObj = GameObject.Instantiate(TilePrefab, new Vector3((j - i)*0.42f, (i + j) * 0.21f, (i + j) * 0.21f), Quaternion.identity, TileContainer);
                tileObj.name = "Tile( " + i + ", " + j + ")";
                Tile tile = tileObj.GetComponent<Tile>() as Tile;
                tile.Init();
                Tiles[i].Add(tile);
            }
        }

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < width; j++) {
                Tiles[i][j].initTile(i, j, Tiles);
            }
        }
	}

    public void EndTurn() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < width; j++) {
                Tiles[i][j].turnHappens();
            }
        }
    }

}
