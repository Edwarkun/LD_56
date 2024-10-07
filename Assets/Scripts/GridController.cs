using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridController : MonoBehaviour
{
    public int currentlySelectedTower = 0;
    public int resources = 0;
    public Button[] towerButtons;
    public GameObject[] towerButtonsHighlights;
    public TextMeshProUGUI resourcesText;
    public GameObject gridSprite;
    public static GridController Instance { get; private set; }

    public Camera cam;

    public static float MapWidth = 11; //Amount of tiles
    public static float MapHeight = 11;

    public static float TileWidth = 1.0f; //Unity units
    public static float TileHeight = 0.5f; 

    public Dictionary<Tuple<int, int>, GameObject> grid = new Dictionary<Tuple<int, int>, GameObject>();

    public GameObject[] baseTowerPrefabs;
    public Dictionary<Tuple<int, int>, Turret> placeables = new Dictionary<Tuple<int, int>, Turret>();

    public Queue<GameObject> debris = new Queue<GameObject>();
    public Dictionary<Tuple<int, int>, int> currentTurrets = new Dictionary<Tuple<int, int>, int>();

    public GameObject lastSelectedTile = null;

    public GameObject lever;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
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
        if (GameController.Instance.RoundStarted)
            return;

        if (resources == 0 || currentlySelectedTower == 0)
            return;

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

        if (row < 0 || col < 0 || row >= MapHeight || col >= MapWidth) 
            return;

        if (!Input.GetKeyDown(KeyCode.Mouse0))
            return;

        Tuple<int, int> fixedPos = Tuple.Create(row, col);
        if (placeables.ContainsKey(fixedPos))
        {
            Turret t = placeables[fixedPos];
            if ((t.ID == 1 || t.ID == 4 || t.ID == 5) && currentlySelectedTower == 1 )
                return;
            if ((t.ID == 2 || t.ID == 6) && currentlySelectedTower == 2)
                return;
            if (t.ID == 3 && currentlySelectedTower == 3)
                return;
        }

        lever.SetActive(true);

        resources--;
        resourcesText.text = resources.ToString();

        if (placeables.ContainsKey(fixedPos))
        {
            //Check if the construction can be disassembled
            if (placeables[fixedPos].originCombination != null){

                int originRow = (int)(-placeables[fixedPos].transform.position.x - placeables[fixedPos].transform.position.y * 2.0f);
                int originCol = (int)(placeables[fixedPos].transform.position.x - placeables[fixedPos].transform.position.y * 2.0f);
                Tuple<int, int> destroyedLocalPos = Tuple.Create(fixedPos.Item1 - originRow, fixedPos.Item2 - originCol);

                Combination tempComb = placeables[fixedPos].originCombination;
                Destroy(placeables[fixedPos].gameObject);

                //Destroy the tower and spawn the recipe
                for (int j = 0; j < tempComb.layout.Length; j++)
                {
                    Tuple<int, int> targetPos = Tuple.Create(fixedPos.Item1 - destroyedLocalPos.Item1 + (int)tempComb.layout[j].x, fixedPos.Item2 - destroyedLocalPos.Item2 + (int)tempComb.layout[j].y);
                    if(targetPos != fixedPos)
                        SpawnTower(tempComb.towerID[j], targetPos);
                }
            }
            Destroy(placeables[fixedPos].gameObject);
            currentTurrets.Remove(fixedPos);
        }

        SpawnTower(currentlySelectedTower, fixedPos);
        lastSelectedTile.transform.GetChild(0).gameObject.SetActive(false);
    }
    //IDs -> 0->empty, 1->DMG, 2->SHIELD, 3->REPULSOR

    public void SpawnTower(int towerID, Tuple<int, int> targetPos)
    {
        currentTurrets[targetPos] = towerID;

        Turret tower = Instantiate(baseTowerPrefabs[towerID], null).GetComponent<Turret>();
        tower.transform.position = grid[targetPos].transform.position;
        placeables[targetPos] = tower;
        tower.CalculatePositions(targetPos);

        if (!GameController.Instance.canCombine)
            return;

        Vector2 offset = Vector2.zero;

        for (int i = 0; i < tower.combinations.Length; i++)
        {
            //Check all the possible positions of this tower relative to the combination
            for (int j = 0; j < tower.combinations[i].layout.Length; j++)
            {
                if (currentlySelectedTower != tower.combinations[i].towerID[j])
                    continue;

                bool valid = true;
                offset = tower.combinations[i].layout[j];
                for (int k = 0; k < tower.combinations[i].layout.Length && valid; k++)
                {
                    if (k == j)
                        continue;

                    Tuple<int, int> tempPos = Tuple.Create<int, int>(targetPos.Item1 + (int)tower.combinations[i].layout[k].x - (int)offset.x, targetPos.Item2 + (int)tower.combinations[i].layout[k].y - (int)offset.y);
                    if (placeables.ContainsKey(tempPos))
                    {
                        if (placeables[tempPos].ID != tower.combinations[i].towerID[k])
                            valid = false;
                    }
                    else
                    {
                        valid = false;
                    }
                }
                if (valid)
                {
                    AudioManager.Instance.PlaySounds("Buildings_Fusion");
                    Tuple<int, int> targetFusionPos = Tuple.Create<int, int>(targetPos.Item1 - (int)offset.x, targetPos.Item2 - (int)offset.y);
                    Destroy(placeables[targetFusionPos].gameObject);
                    currentTurrets.Remove(targetFusionPos);
                    SpawnTower(tower.combinations[i].towerToSpawnID, targetFusionPos);
                    return;
                }
            }
        }
        if(tower.originCombination != null)
        {
            for(int i = 1; i < tower.originCombination.layout.Length; i++)
            {
                int posX = targetPos.Item1 + (int)tower.originCombination.layout[i].x;
                int posY = targetPos.Item2 + (int)tower.originCombination.layout[i].y;
                Tuple<int, int> tempPos = Tuple.Create(posX, posY);
                if(placeables.ContainsKey(tempPos))
                    Destroy(placeables[tempPos].gameObject);
                currentTurrets.Remove(tempPos);
                placeables[tempPos] = tower;
            }
        }
    }

    public void SelectTower(int ID)
    {
        if(ID == currentlySelectedTower)
        {
            currentlySelectedTower = 0;
        }
        currentlySelectedTower = ID;
        for (int i = 1; i < towerButtons.Length + 1; i++)
        {
            towerButtons[i - 1].interactable = i != ID;
            towerButtonsHighlights[i - 1].SetActive(i == ID);
        }
    }
    public void SetupRound(int resources)
    {
        this.resources = resources;
        resourcesText.text = resources.ToString();
        while (debris.Count > 0)
        {
            Destroy(debris.Dequeue());
        }

        foreach(var tower in placeables)
        {
            if (tower.Value != null)
                Destroy(tower.Value.gameObject);
        }
        placeables.Clear();

        Dictionary<Tuple<int, int>, int> temp = new Dictionary<Tuple<int, int>, int>(currentTurrets);
        foreach (var t in temp)
        {
            SpawnTower(t.Value, t.Key);
        }

        for (int i = 0; i < towerButtons.Length; i++)
        {
            towerButtons[i].interactable = true;
        }

        foreach (var g in grid)
        {
            g.Value.SetActive(true);
        }

        gridSprite.SetActive(true);
    }
    public void StartRound()
    {
        gridSprite.SetActive(false);
        for(int i = 0; i < towerButtons.Length; i++)
        {
            towerButtons[i].interactable = false;
            towerButtonsHighlights[i].SetActive(false);
        }

        foreach (var g in grid)
        {
            g.Value.SetActive(false);
        }
        currentlySelectedTower = 0;
        if (lastSelectedTile != null)
            lastSelectedTile.transform.GetChild(0).gameObject.SetActive(false);
    }
}
