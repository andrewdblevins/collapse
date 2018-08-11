using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum InfluenceType {stabilization, gold}

public class Tile : MonoBehaviour {

    public Transform container;
    public List<SpriteRenderer> Images;

    private int hp;
    private int tileX;
    private int tileY;
    private int maxDampeningDistance = 3;
    private float baseHpLossRisk = 0.05f;

    private float stabilizationBaseHpReduction = 1.0f;

    private Tile xMinusTile = null;
    private Tile xPlusTile = null;
    private Tile yMinusTile = null;
    private Tile yPlusTile = null;

    //Cache of tiles w/ distance from this tile, w/ an additional filter of maxdistance as first key
    private Dictionary<int, Dictionary<Tile, int>> tileDistanceCache = new Dictionary<int, Dictionary<Tile, int>> { };

    private List<Tile> immediateTiles() {
        List<Tile> tiles = new List<Tile> { xMinusTile, xPlusTile, yMinusTile, yPlusTile };
        tiles.RemoveAll(item => item == null);
        return tiles;
    }

    public Dictionary<Tile, int> tilesWithinDistanceCalcImpl(int distance, Dictionary<Tile, int> existingTiles = null) {
        //Debug.Log("Starting distance: " + distance.ToString());
        if (existingTiles == null) {
            existingTiles = new Dictionary<Tile, int> { };
        }

        List<Tile> tilesToRecurse = new List<Tile> { };
        foreach (var tile in immediateTiles()) {
            //Debug.Log("Thinking of adding tile: " + tile.GetHashCode());
            //Debug.Log("Distance: " + distance.ToString());
            //Debug.Log("Contains: " + existingTiles.ContainsKey(tile).ToString());
            if (existingTiles.ContainsKey(tile) && existingTiles[tile] <= distance + 1)
                continue;

            //Debug.Log("Adding tile: " + tile.GetHashCode());
            //Debug.Log("Distance: " + distance.ToString());
            existingTiles[tile] = distance + 1;
            tilesToRecurse.Add(tile);
        }


        if (distance > 1) {
            foreach (var tile in tilesToRecurse)
            {
                existingTiles = tile.tilesWithinDistanceCalcImpl(distance - 1, existingTiles);
            }
        }
        return existingTiles;
    }

    private Dictionary<Tile, int> tilesWithinDistance(int distance) {
        if (distance <= 0) return new Dictionary<Tile, int> { };

        if (!tileDistanceCache.ContainsKey(distance)) {
            //Force creation of all caches with smaller distances first if they don't exist
            Dictionary<Tile, int> tilesWithinDistanceMinusOne = tilesWithinDistance(distance - 1);
            //tileDistanceCache[distance] = tilesWithinDistanceCalcImpl(distance, tilesWithinDistanceMinusOne);
            tileDistanceCache[distance] = tilesWithinDistanceCalcImpl(distance);
        }
        return tileDistanceCache[distance];
    }

    private void SetHeight() {
        float height = (hp * 0.05f);// - 0.1f;
        container.localPosition = new Vector3(0f, height, 0f);
    }

    public void Init() {
        // SetHeight(UnityEngine.Random.Range(-0.1f, 0.1f));
        SetHeight();

        bool flip = UnityEngine.Random.Range(0, 3) == 0;
        if (flip) {
            Images[1].gameObject.SetActive(true);
            flip = UnityEngine.Random.Range(0, 3) == 0;
            if (flip) {
                Images[2].gameObject.SetActive(true);
            }
        }
    }

    public void OnMouseDown() {
        Debug.Log("Clicked " + gameObject.name);
    }

    public void OnMouseEnter() {
        foreach (SpriteRenderer s in Images) {
            s.color = Color.yellow;
        }
    }

    public void OnMouseExit() {
        foreach (SpriteRenderer s in Images) {
            if (hp == 1) {
                s.color = Color.red;
            } else {
                s.color = Color.white;
            }
        }
    }

    public int getHp() {
        return hp;
    }

    public bool tileActive() {
        return hp > 0;
    }

    public void initTile(int x, int y, List<List<Tile>> tileList) {
        tileX = x;
        tileY = y;

        hp = 10;

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

        Init();
    }

    // Should call this in board constructor after all tiles are init-created, so as to speed up first loop later
    public void initTileDistances() {
        Dictionary<Tile, int> d = tilesWithinDistance(maxDampeningDistance);
        //foreach (KeyValuePair<Tile, int> kvp in d)
        //{
        //    //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
        //    Debug.Log(String.Format("Key = {0}, Value = {1}", kvp.Key.GetHashCode(), kvp.Value));
        //}
        //Debug.Log("tileDistance " + d);
    }

    // Some prototype function for dampening influence
    private float influenceDampener(float influence, int distance, InfluenceType influenceType) {
        if (distance > maxDampeningDistance) {
            return 0.0f;
        }
        return influence / distance;
    }

    //This is a probability, so a 0.20f reduction would reduce some previous risk from 0.05 to 0.04
    public float getInfluence(InfluenceType influenceType) {
        switch (influenceType) {
            case InfluenceType.stabilization : return 0.35f * stabilizationBaseHpReduction;
            case InfluenceType.gold : return 0.0f;
        }
        throw new Exception("Bad InfluenceType for getting tile influence");
    }

    public float hpLossRisk()
    {
        //Get stabilization influence from neighboring tiles
        float risk = baseHpLossRisk;
        foreach (KeyValuePair<Tile, int> entry in tilesWithinDistance(maxDampeningDistance))
        {
            risk *= (1.0f - influenceDampener(entry.Key.getInfluence(InfluenceType.stabilization), entry.Value, InfluenceType.stabilization));
        }
        return risk;
    }

    public void turnHappens() {
        if (UnityEngine.Random.value < hpLossRisk()) {
            hp -= 1;
        }

        if (hp == 0) {
            container.gameObject.SetActive(false);
        } else if (hp == 1) {
            foreach (SpriteRenderer s in Images) {
                s.color = Color.red;
            }
        }

        SetHeight();
    }

    public override int GetHashCode() {
       return tileX * 1000 + tileY;
    }
}
