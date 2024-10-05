using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Device;
using static UnityEditor.PlayerSettings;

public class GridController : MonoBehaviour
{
    public static GridController Instance { get; private set; }

    public Camera cam;

    public readonly float MapWidth = 10; //Amount of tiles
    public readonly float MapHeight = 10;

    public readonly float TileWidth = 1.0f; //Unity units
    public readonly float TileHeight = 0.5f; 

    public Dictionary<Tuple<int, int>, GameObject> grid = new Dictionary<Tuple<int, int>, GameObject>();

    public GameObject[] baseTowerPrefabs;
    public Dictionary<Tuple<int, int>, Turret> placeables = new Dictionary<Tuple<int, int>, Turret>();

    public GameObject lastSelectedTile = null;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }

        for(int i = 0; i < MapWidth; i++)
        {
            for(int j = 0; j < MapHeight; j++)
            {
                Vector2 pos = new Vector2(i, j);
                float posX = (-pos.x * TileWidth + pos.y * TileWidth) / 2f;
                float posY = (-pos.x * TileHeight - pos.y * TileHeight + -0.5f) / 2f;

                GameObject newTile = Instantiate(baseTowerPrefabs[0], transform);

                newTile.transform.name = pos.x.ToString() + ", " + pos.y.ToString();

                newTile.transform.position = new Vector2(posX, posY);
                grid[Tuple.Create((int)pos.x, (int)pos.y)] = newTile;
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        GameObject tile = GetGridTile(mousePos);
        if(tile == null)
        {
            if (lastSelectedTile != null)
                lastSelectedTile.transform.GetChild(0).gameObject.SetActive(false);
            return;
        }
        if (lastSelectedTile != tile)
        {
            if (lastSelectedTile != null)
                lastSelectedTile.transform.GetChild(0).gameObject.SetActive(false);
            lastSelectedTile = tile;
            lastSelectedTile.transform.GetChild(0).gameObject.SetActive(true);
            AudioManager.Instance.PlaySounds("UI_MoveBuilding");
        }

        PlaceTowers(mousePos);
    }

    GameObject GetGridTile(Vector2 pos) {
        int row = (int)(-pos.x - pos.y * 2.0f);
        int col = (int)(pos.x - pos.y * 2.0f);

        Tuple<int, int> fixedPos = Tuple.Create(row, col);
        if (!grid.ContainsKey(fixedPos))
            return null;
        return grid[fixedPos];
    }

    void PlaceTowers(Vector2 pos)
    {
        int row = (int)(-pos.x - pos.y * 2.0f);
        int col = (int)(pos.x - pos.y * 2.0f);

        if(row < 0 || col < 0 || row >= MapHeight || col >= MapWidth) 
            return;

        for (int i = (int)KeyCode.Alpha1; i <= (int)KeyCode.Alpha3; i++)
        {
            if (!Input.GetKeyDown((KeyCode)i))
            {
                continue;
            }

            Tuple<int, int> fixedPos = Tuple.Create(row, col);
            if (placeables.ContainsKey(fixedPos))
            {
                //Check if the construction can be disassembled
                while (placeables[fixedPos].originCombination != null){

                    int originRow = (int)(-placeables[fixedPos].transform.position.x - placeables[fixedPos].transform.position.y * 2.0f);
                    int originCol = (int)(placeables[fixedPos].transform.position.x - placeables[fixedPos].transform.position.y * 2.0f);
                    Tuple<int, int> destroyedLocalPos = Tuple.Create(fixedPos.Item1 - originRow, fixedPos.Item2 - originCol);

                    Combination tempComb = placeables[fixedPos].originCombination;
                    Destroy(placeables[fixedPos].gameObject);
                    //Destroy the tower and spawn the recipe
                    for (int j = 0; j < tempComb.layout.Length; j++)
                    {
                        Tuple<int, int> targetPos = Tuple.Create(fixedPos.Item1 - destroyedLocalPos.Item1 + (int)tempComb.layout[j].x, fixedPos.Item2 - destroyedLocalPos.Item2 + (int)tempComb.layout[j].y);
                        
                        GameObject tower = Instantiate(baseTowerPrefabs[tempComb.towerID[j]], null);
                        tower.transform.position = grid[targetPos].transform.position;
                        placeables[targetPos] = tower.GetComponent<Turret>();
                    }
                }
                Destroy(placeables[fixedPos].gameObject);
            }

            //Check the combinations to see if something needs to be destroyed.
            Turret t = baseTowerPrefabs[1 + i - (int)KeyCode.Alpha1].GetComponent<Turret>();
            GameObject spawnedObject = null;
            Vector2 offset = Vector2.zero;

            //Check all the combinations
            for (int j = 0; j < t.combinations.Length; j++)
            {
                //Check all the possible positions of this tower relative to the combination
                for(int k = 0; k < t.combinations[j].layout.Length; k++)
                {
                    if(1 + i - (int)KeyCode.Alpha1 != t.combinations[j].towerID[k])
                        continue;

                    bool valid = true;
                    offset = t.combinations[j].layout[k];
                    for (int w = 0; w < t.combinations[j].layout.Length && valid; w++)
                    {
                        if (w == k)
                            continue;

                        Tuple<int, int> tempPos = Tuple.Create<int, int>(fixedPos.Item1 + (int)t.combinations[j].layout[w].x - (int)offset.x, fixedPos.Item2 + (int)t.combinations[j].layout[w].y - (int)offset.y);
                        if (placeables.ContainsKey(tempPos))
                        {
                            if (placeables[tempPos].ID != t.combinations[j].towerID[w])
                                valid = false;
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                    if (valid)
                    {
                        spawnedObject = Instantiate(t.combinations[j].towerToSpawn, null);
                        spawnedObject.transform.position = grid[Tuple.Create<int, int>(fixedPos.Item1 - (int)offset.x, fixedPos.Item2 - (int)offset.y)].transform.position;
                        lastSelectedTile.transform.GetChild(0).gameObject.SetActive(false);

                        spawnedObject.GetComponent<Turret>().CalculatePositions(Tuple.Create<int, int>(fixedPos.Item1 - (int)offset.x, fixedPos.Item2 - (int)offset.y));
                        placeables[fixedPos] = spawnedObject.GetComponent<Turret>();

                        //Delete other turrets that are now combined
                        for (int w = 0; w < t.combinations[j].layout.Length; w++)
                        {
                            Tuple<int, int> tempPos = Tuple.Create<int, int>(fixedPos.Item1 + (int)t.combinations[j].layout[w].x - (int)offset.x, fixedPos.Item2 + (int)t.combinations[j].layout[w].y - (int)offset.y);
                            if (w == k)
                                continue;
                            if (placeables.ContainsKey(tempPos))
                            {
                                Destroy(placeables[tempPos].gameObject);
                                //Assign all the positions occupied to the dictionary
                                placeables[tempPos] = placeables[fixedPos];
                            }
                        }
                        AudioManager.Instance.PlaySounds("Buildings_Fusion");
                        return;
                    }
                }
            }

            spawnedObject = Instantiate(baseTowerPrefabs[1 + i - (int)KeyCode.Alpha1], null);
            spawnedObject.transform.position = grid[fixedPos].transform.position;
            lastSelectedTile.transform.GetChild(0).gameObject.SetActive(false);

            spawnedObject.GetComponent<Turret>().CalculatePositions(fixedPos);
            placeables[fixedPos] = spawnedObject.GetComponent<Turret>();
        }
    }
    //IDs -> 0->empty, 1->DMG, 2->SHIELD, 3->REPULSOR
}
