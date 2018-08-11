using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum InfluenceType {stabilization, gold}

//public struct TileWithDistance {
//    public Tile tile;
//    public int distance;

//    public TileWithDistance(Tile t, int d)
//    {
//        tile = t;
//        distance = d;
//    }
//}

public class Tile : MonoBehaviour {

    public Transform container;

    private int hp;
    private int tileX;
    private int tileY;
    private int maxDampeningDistance = 3;
    private float baseHpLossRisk = 0.05f;
    private float stabilizationBaseHpReduction = 0.01f;

    private Tile xMinusTile = null;
    private Tile xPlusTile = null;
    private Tile yMinusTile = null;
    private Tile yPlusTile = null;

    private List<Tile> immediateTiles() {
        List<Tile> tiles = new List<Tile> { xMinusTile, xPlusTile, yMinusTile, yPlusTile };
        tiles.RemoveAll(item => item == null);
        return tiles;
    }

    //private Dictionary<Tile, int> tilesWithinDistance(int distance, Dictionary<Tile, int> existingTiles = null) {
    //    if (existingTiles == null) {
    //        existingTiles = new Dictionary<Tile, int> { };
    //    }




    //    if (distance > 1) {
    //        existingTiles = tilesWithinDistance(distance - 1, existingTiles);
    //    }
    //}

    //private List<Tile> tilesWithinDistance()

    public void SetHeight(float height) {
        container.localPosition = new Vector3(0f, height, 0f);
    }

    public void initTile(int x, int y, List<List<Tile>> tileList) {
        tileX = x;
        tileY = y;

        int minTileListX = 0;
        int maxTileListX = tileList[0].Count - 1;
        int minTileListY = 0;
        int maxTileListY = tileList.Count - 1;

        if (x > minTileListX) {
            xMinusTile = tileList[y][x-1];
        }
        if (x < maxTileListX) {
            xPlusTile = tileList[y][x+1];
        }
        if (y > minTileListY)
        {
            yMinusTile = tileList[y-1][x];
        }
        if (y < maxTileListY)
        {
            yPlusTile = tileList[y+1][x];
        }

    }

    // Some prototype function for dampening influence
    private float influenceDampener(float influence, int distance, InfluenceType influenceType) {
        if (distance > maxDampeningDistance) {
            return 0.0f;
        }
        return influence / distance;
    }

    public float getInfluence(InfluenceType influenceType) {
        switch (influenceType) {
            case InfluenceType.stabilization : return 0.5f * stabilizationBaseHpReduction;
            case InfluenceType.gold : return 0.0f;
        }
        throw new Exception("Bad InfluenceType for getting tile influence");
    }

    public float hpLossRisk()
    {
        //Get stabilization influence from neighboring tiles
        //TODO: Go further than neighboring tiles
        //TODO: Instead of just subtracting risk, make it more of a compounding percentage reduction effect.
        float risk = baseHpLossRisk;
        foreach (var tile in immediateTiles())
        {
            risk -= influenceDampener(tile.getInfluence(InfluenceType.stabilization), 1, InfluenceType.stabilization);
        }
        return risk;
    }



    public override int GetHashCode() {
       return tileX * 1000 + tileY;
    }
}
