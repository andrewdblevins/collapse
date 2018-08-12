using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum InfluenceType {stabilization, gold, wood, iron, coal, mine, saw}

public class Tile : MonoBehaviour
{

    public Transform container;
    public List<SpriteRenderer> Images;

    public List<SpriteRenderer> Dodads;
    public List<SpriteRenderer> OreImages;
    public List<SpriteRenderer> BuildingImages;

    public List<SpriteRenderer> UpArrowImages;

    private Board board;

    private Board.Building building = Board.Building.None;
    private float total_rotation = 0f;
    private float starting_max_rotation = 5f;
    private float max_rotation = 5f;
    private float last_rotation = 1f;
    private float rotation_rate = 5f;
    private int hp = -1;
    private int tileX;
    private int tileY;
    private const int maxDampeningDistance = 3;
    private const float baseHpLossRisk = 0.03f;
    public float modifiedHpLossRisk = baseHpLossRisk;
    public float normalizedHpLossRisk = baseHpLossRisk;
    private Tile xMinusTile = null;
    private Tile xPlusTile = null;
    private Tile yMinusTile = null;
    private Tile yPlusTile = null;


    private int minTileListX;
    private int maxTileListX;
    private int minTileListY;
    private int maxTileListY;

    //Cache of tiles w/ distance from this tile, w/ an additional filter of maxdistance as first key
    private Dictionary<int, Dictionary<Tile, int>> tileDistanceCache = new Dictionary<int, Dictionary<Tile, int>> { };

    public void Init()
    {
        SetHeight();

        if (UnityEngine.Random.value < 0.33f)
        {
            Dodads[0].gameObject.SetActive(true);
            if (UnityEngine.Random.value < 0.33f)
            {
                Dodads[1].gameObject.SetActive(true);
            }
        }

        if (UnityEngine.Random.value < 0.1f) {
            building = Board.Building.Wood;
            OreImages[2].gameObject.SetActive(true);
        } else if (UnityEngine.Random.value < 0.08f)
        {
            building = Board.Building.Iron;
            OreImages[0].gameObject.SetActive(true);
        } else if (UnityEngine.Random.value < 0.04f) {
            building = Board.Building.Coal;
            OreImages[1].gameObject.SetActive(true);
        }


        last_rotation = UnityEngine.Random.Range(-1.0f, 1.0f);


        UpArrowImages[2].gameObject.SetActive(false);
        UpArrowImages[1].gameObject.SetActive(false);
        UpArrowImages[0].gameObject.SetActive(false);
    }



    void Start(){
        max_rotation = starting_max_rotation;
        //if(hp > 0){
        //    hp += 5;
        //}
    }

    void Update()
    {
        Tremor();
        UpdateSway();



    }

    public void UpdateSway(){
        
        rotation_rate = 20 * modifiedHpLossRisk + Mathf.Max(1, (8 - hp));
        max_rotation = starting_max_rotation - 3 * modifiedHpLossRisk;
        if(building == Board.Building.Stabilizer){
            rotation_rate /= 10;
            max_rotation = Mathf.Min(max_rotation, starting_max_rotation / 5);
        }
        //max_rotation = rotation_rate * 3;
    }

    public void Tremor()
    {
        if (last_rotation >= 0)
        {
            last_rotation =  rotation_rate * Time.deltaTime;
            total_rotation += last_rotation;

        }
        else
        {
            last_rotation = -rotation_rate * Time.deltaTime;
            total_rotation += last_rotation;
        }

        transform.Rotate(new Vector3(0f, 0f, last_rotation));

        if (Mathf.Abs(total_rotation) > max_rotation)
        {
            if (total_rotation > 0)
            {
                last_rotation = -1;

            }
            else
            {
                last_rotation = 1;
            }
        }
    }

    public void displayInfluenceIcon()
    {
        InfluenceType? influenceType = Board.getBuildingInfluence(building);
        if (influenceType == null) return;
        //getInfluence((InfluenceType)influenceType);

        //TODO: Create a separate structure if we start highlighting things within a larger distance
        foreach (KeyValuePair<Tile, int> entry in tilesWithinDistance(1))
        {
            Tile tile = entry.Key;
            float influence = tile.getInfluence((InfluenceType)influenceType);
            if (influence > 4.0f) {
                tile.UpArrowImages[2].gameObject.SetActive(true);
            } else if (influence > 2.0f) {
                tile.UpArrowImages[1].gameObject.SetActive(true);
            } else if (influence > 0.0f)
            {
                tile.UpArrowImages[0].gameObject.SetActive(true);
            }
            // Can later add other cases here if influence is being decreased

        }

    }

    public void stopDisplayingInfluenceIcon()
    {
        InfluenceType? influenceType = Board.getBuildingInfluence(building);
        if (influenceType == null) return;
        //getInfluence((InfluenceType)influenceType);

        //TODO: Create a separate structure if we start highlighting things within a larger distance
        foreach (KeyValuePair<Tile, int> entry in tilesWithinDistance(1)) {
            Tile tile = entry.Key;
            tile.UpArrowImages[2].gameObject.SetActive(false);
            tile.UpArrowImages[1].gameObject.SetActive(false);
            tile.UpArrowImages[0].gameObject.SetActive(false);
        }
    }


    public void SetBuilding(Board.Building b) {
        building = b;
        switch (b) {
            case Board.Building.House:
                BuildingImages[0].gameObject.SetActive(true);
                BuildingImages[1].gameObject.SetActive(true);
                break;
            case Board.Building.Mine:
                BuildingImages[2].gameObject.SetActive(true);
                break;
            case Board.Building.Stabilizer:
                BuildingImages[5].gameObject.SetActive(true);
                break;
            case Board.Building.Saw:
                BuildingImages[6].gameObject.SetActive(true);
                break;
            case Board.Building.House2:
                BuildingImages[0].gameObject.SetActive(true);
                BuildingImages[1].gameObject.SetActive(false);
                BuildingImages[2].gameObject.SetActive(true);
                break;
            case Board.Building.Mine2:
                BuildingImages[2].gameObject.SetActive(true);
                BuildingImages[3].gameObject.SetActive(true);
                break;
            case Board.Building.Stabilizer2:
                BuildingImages[5].gameObject.SetActive(true);
                BuildingImages[9].gameObject.SetActive(true);
                break;
            case Board.Building.Saw2:
                BuildingImages[7].gameObject.SetActive(true);
                break;
            default:
                Debug.Log("Not adding Building: " + b);
                break;
        }
    }

    public Board.Building GetBuildingType() {
        return building;
    }

    public bool IsEmpty() {
        return building == Board.Building.None;
    }

    public void Click() {
        Debug.Log("Clicked " + gameObject.name);
        board.TileClicked(this);
    }

    public void Highlight() {
        foreach (SpriteRenderer s in Images) {
            s.color = Color.yellow;
        }
    }

    public void UnHighlight() {
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

    public void initTile(int x, int y, List<List<Tile>> tileList, Board board, int? initHp=-1) {
        this.board = board;
        tileX = x;
        tileY = y;

        if (initHp != null) hp = (int)initHp;

        minTileListX = 0;
        maxTileListX = tileList[0].Count - 1;
        minTileListY = 0;
        maxTileListY = tileList.Count - 1;

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

    public bool closerToEdge(Tile tile) {
        return distanceToEdge() < tile.distanceToEdge();
    }

    public int distanceToEdge() {
        return Mathf.Min(maxTileListX - 1 - tileX,
            tileX - minTileListX,
            maxTileListY - 1 - tileY,
            tileY - minTileListY);
    } 

    // If our height is null, smooth out the height using neighboring tiles
    // Return False if we failed to do smoothing (if no neighboring tiles had a height)
    public bool smoothHp() {
        if (hp >= 0) { return true; }
        List<Tile> neighborTilesWithHp = immediateTiles();
        neighborTilesWithHp.RemoveAll(item => item.getHp() < 0);

        //Debug.Log("Attempting to set height");
        //Debug.Log(neighborTilesWithHp.Count.ToString());

        if (neighborTilesWithHp.Count > 1) {
            int totalHp = 0;
            foreach(Tile tile in neighborTilesWithHp) {
                totalHp += tile.getHp();
            }
            hp = totalHp / neighborTilesWithHp.Count;
            //foreach (SpriteRenderer s in Images)
            //{
            //    s.color = Color.grey;
            //}
            //SetHeight();
            return true;
        }
        else if (neighborTilesWithHp.Count == 1) {
            int neighHp = neighborTilesWithHp[0].getHp();
            //hp = neighHp;
            if (closerToEdge(neighborTilesWithHp[0])) {
                hp = UnityEngine.Random.Range(Mathf.Max(neighHp - 2, board.getMinHeight()), neighHp);
            } else {
                hp = UnityEngine.Random.Range(neighHp, Mathf.Min(neighHp + 2, board.getMaxHeight()));
            }
            //Debug.Log("Tile: " + name);
            //Debug.Log("Neighbor " + neighborTilesWithHp[0].name + " hp: " + neighHp.ToString());
            //Debug.Log("Set hp: " + hp.ToString());
            //foreach (SpriteRenderer s in Images)
            //{
            //    s.color = Color.red;
            //}
            //SetHeight();
            return true;
        }
        else {
            return false;
        }
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
    private float influenceDampener(float influence, int hpDifference, int distance, InfluenceType influenceType) {
        if (distance > maxDampeningDistance) {
            return 0.0f;
        }
        return (1.0f + (float)hpDifference / 6.0f) * influence / distance;
    }

    //This is a probability, so a 0.20f reduction would reduce some previous risk from 0.05 to 0.04
    public float getInfluence(InfluenceType influenceType) {
        switch (influenceType) {
            case InfluenceType.stabilization :
                {
                    float reduction = 0.25f;
                    if (building == Board.Building.Stabilizer) reduction += 0.9f;
                    return reduction;
                }
            case InfluenceType.gold :
            case InfluenceType.iron :
            case InfluenceType.coal : 
                {
                    if (building == Board.Building.Mine) {
                        // Gather tilesWithinDistance(1) for houses and gather house influence
                        float houseEffect = 0f;
                        foreach (KeyValuePair<Tile, int> entry in tilesWithinDistance(1)) {
                            houseEffect += entry.Key.getInfluence(InfluenceType.mine);
                        }
                        return 1.0f + houseEffect;
                    }
                    return 0.0f;
                }

            case InfluenceType.wood: {
                    if (building == Board.Building.Saw) {
                        // Gather tilesWithinDistance(1) for houses and gather house influence
                        float houseEffect = 0f;
                        foreach (KeyValuePair<Tile, int> entry in tilesWithinDistance(1)) {
                            houseEffect += entry.Key.getInfluence(InfluenceType.saw);
                        }
                        return 1.0f + houseEffect;
                    }
                    return 0.0f;
                }
            case InfluenceType.mine: {
                    if (building == Board.Building.House) {
                        return 0.5f;
                    }
                    return 0.0f;
                }
            case InfluenceType.saw:
                {
                    if (building == Board.Building.House)
                    {
                        return 0.5f;
                    }
                    return 0.0f;
                }
        }
        throw new Exception("Bad InfluenceType for getting tile influence");
    }

    public float goldHarvest() {
        if (building == Board.Building.Gold) {
            float gold = 0.0f;
            foreach (KeyValuePair<Tile, int> entry in tilesWithinDistance(1)) {
                gold += entry.Key.getInfluence(InfluenceType.gold);
            }
            return gold * Globals.Instance.GoldMultiplier;
        }
        return 0.0f;
    }

    public float woodHarvest()
    {
        if (building == Board.Building.Wood)
        {
            float wood = 0f;
            foreach (KeyValuePair<Tile, int> entry in tilesWithinDistance(1))
            {
                wood += entry.Key.getInfluence(InfluenceType.wood);
            }
            return wood;
        }
        return 0.0f;
    }

    public float ironHarvest()
    {
        if (building == Board.Building.Iron) {
            float iron = 0.0f;
            foreach (KeyValuePair<Tile, int> entry in tilesWithinDistance(1)) {
                iron += entry.Key.getInfluence(InfluenceType.iron);
            }
            return iron * Globals.Instance.IronMultiplier;
        }
        return 0.0f;
    }

    public float coalHarvest()
    {
        if (building == Board.Building.Coal) {
            float coal = 0.0f;
            foreach (KeyValuePair<Tile, int> entry in tilesWithinDistance(1)) {
                coal += entry.Key.getInfluence(InfluenceType.coal);
            }
            return coal * Globals.Instance.CoalMultiplier;
        }
        return 0.0f;
    }

    public float hpLossRisk()
    {
        //Get stabilization influence from neighboring tiles
        float risk = baseHpLossRisk;
        foreach (KeyValuePair<Tile, int> entry in tilesWithinDistance(maxDampeningDistance))
        {
            risk *= (1.0f - influenceDampener(entry.Key.getInfluence(InfluenceType.stabilization), 
                                              entry.Key.getHp() - getHp(), 
                                              entry.Value, 
                                              InfluenceType.stabilization));
        }

        modifiedHpLossRisk = risk * Globals.Instance.DecayRate;

        return normalizedHpLossRisk;
    }

    public void turnHappens() {
        float randomDraw;
        randomDraw = UnityEngine.Random.value;

        if (randomDraw < hpLossRisk()) {
            hp -= 1;
        }


        if (hp <= 0) {
            building = Board.Building.Void;
            container.gameObject.SetActive(false);
        } else if (hp <= 1) {
            foreach (SpriteRenderer s in Images) {
                s.color = Color.red;
            }
        }

        SetHeight();
    }

    private List<Tile> immediateTiles()
    {
        List<Tile> tiles = new List<Tile> { xMinusTile, xPlusTile, yMinusTile, yPlusTile };
        tiles.RemoveAll(item => item == null);
        return tiles;
    }

    public Dictionary<Tile, int> tilesWithinDistanceCalcImpl(int distance, Dictionary<Tile, int> existingTiles = null)
    {
        //Debug.Log("Starting distance: " + distance.ToString());
        if (existingTiles == null)
        {
            existingTiles = new Dictionary<Tile, int> { };
        }

        List<Tile> tilesToRecurse = new List<Tile> { };
        foreach (var tile in immediateTiles())
        {
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


        if (distance > 1)
        {
            foreach (var tile in tilesToRecurse)
            {
                existingTiles = tile.tilesWithinDistanceCalcImpl(distance - 1, existingTiles);
            }
        }
        return existingTiles;
    }

    private Dictionary<Tile, int> tilesWithinDistance(int distance)
    {
        if (distance <= 0) return new Dictionary<Tile, int> { };

        if (!tileDistanceCache.ContainsKey(distance))
        {
            //Force creation of all caches with smaller distances first if they don't exist
            Dictionary<Tile, int> tilesWithinDistanceMinusOne = tilesWithinDistance(distance - 1);
            //tileDistanceCache[distance] = tilesWithinDistanceCalcImpl(distance, tilesWithinDistanceMinusOne);
            tileDistanceCache[distance] = tilesWithinDistanceCalcImpl(distance);
        }
        return tileDistanceCache[distance];
    }

    private void SetHeight()
    {
        float height = (getHp() * 0.05f);
        container.localPosition = new Vector3(0f, height, 0f);
    }

    public override int GetHashCode() {
       return tileX * 1000 + tileY;
    }


}
