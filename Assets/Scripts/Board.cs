﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour {

    public enum Building { None, Mine, House, Stabilizer, Gold, Void};
    public Building selectedType = Building.None;

    public GameObject TilePrefab;

    public Transform TileContainer;
    public Text hintText;

    float tick = 10f;
    float tickTimer = 10f;

    List<List<Tile>> Tiles = new List<List<Tile>>();

    private const int width = 15;
    private const int maxHeight = 15 < width ? width : 15;
    private const int minHeight = 0;
    private const float probabilityHeightAssign = 7.0f / (width * width);

    private int? getHeight(int x, int y)
    {
        //Corners will always be very low
        if ((x == 0 || x == width - 1) && (y == 0 || y == width - 1))
        {
            return minHeight;
        }
        if (UnityEngine.Random.value < probabilityHeightAssign) {
            int minDistanceFromEdge = Mathf.Min(
                width - 1 - x,
                x,
                width - 1 - y,
                y
            );

            return UnityEngine.Random.Range(minDistanceFromEdge + minHeight + 2, maxHeight);
        }
        return null;

    }

	void Start () {
		for (int i = 0; i < width; i++) {
            Tiles.Add(new List<Tile>());
            for (int j = 0; j < width; j++) {
                GameObject tileObj = GameObject.Instantiate(TilePrefab, TileContainer, false);
                tileObj.transform.localPosition = new Vector3((j - i) * 0.42f, (i + j) * 0.21f, (i + j) * 0.21f);
                //GameObject tileObj = GameObject.Instantiate(TilePrefab, new Vector3((j - i)*0.42f, (i + j) * 0.21f, (i + j) * 0.21f), Quaternion.identity, TileContainer);
                tileObj.name = "Tile( " + i + ", " + j + ")";
                Tile tile = tileObj.GetComponent<Tile>() as Tile;
                //tile.Init();
                Tiles[i].Add(tile);
            }
        }

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < width; j++) {

                // Generate HP, with hp more likely to be higher near middle of map
                // Only give HP to some tiles, and then have the rest smooth themselves out

                Tiles[i][j].initTile(j, i, Tiles, this, getHeight(i, j));
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Tiles[i][j].initTileDistances();
            }
        }

        //Apply tile height smoothing
        int iCnt = 0;
        while (iCnt < 400) {
            bool ifSmoothed = Tiles[UnityEngine.Random.Range(minHeight, maxHeight)][UnityEngine.Random.Range(minHeight, maxHeight)].smoothHp();
            iCnt += 1;
        }
        iCnt = 0;
        while (iCnt < 40) {
            bool allSmoothed = true;
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    // Generate HP, with hp more likely to be higher near middle of map
                    // Only give HP to some tiles, and then have the rest smooth themselves out

                    bool ifSmoothed = Tiles[i][j].smoothHp();
                    if (!ifSmoothed) allSmoothed = false;
                }
            }
            iCnt += 1;
            if (allSmoothed) break;
        }
        EndTurn();
	}

    void Update()
    {
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0)
        {
            EndTurn();
        }


    }

    public void EndTurn() {
        tickTimer = tick;
        float totalHarvest = 0f;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < width; j++) {
                Tiles[i][j].turnHappens();
                totalHarvest += Tiles[i][j].goldHarvest();
                ResourcesPanel.Instance.UpdateGold(Tiles[i][j].goldHarvest());
            }
        }
        Debug.Log("totalHarvest: " + totalHarvest);
    }

    public void SetSelectedType(string type) {
        Building newType = (Building) System.Enum.Parse(typeof(Building), type);
        SetSelectedType(newType);
    }

    public void SetSelectedType(Building newType) {
        if (selectedType != newType) {
            selectedType = newType;
            switch (newType) {
                case Board.Building.None:
                    hintText.text = "";
                    break;
                case Board.Building.House:
                    hintText.text = "Cost 25:   Build a House next to Mines to make them mine more";
                    break;
                case Board.Building.Mine:
                    hintText.text = "Cost 50:   Build a Mine next to ore";
                    break;
                case Board.Building.Stabilizer:
                    hintText.text = "Cost 100:   Build a Stabilizer to prevent tiles from falling";
                    break;
                default: break;
            }
        } else {
            selectedType = Building.None;
        }
    }

    public void TileClicked(Tile tile) {
        if (selectedType != Building.None && tile.IsEmpty() && ResourcesPanel.Instance.GetGold() >= GetCost(selectedType)) {
            ResourcesPanel.Instance.UpdateGold(-GetCost(selectedType));
            tile.SetBuilding(selectedType);
            SetSelectedType(Building.None);
        }
    }

    private float GetCost(Building b) {
        switch (b) {
            case Building.None: return 0f;
            case Building.Mine: return 50f;
            case Building.House: return 25f;
            case Building.Stabilizer: return 100f;
            case Building.Gold: return 0f;
        }

        return 0f;
    }

}
