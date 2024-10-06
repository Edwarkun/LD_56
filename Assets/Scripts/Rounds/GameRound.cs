using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CreateAssetMenu(fileName = "GameRound", menuName = "ScriptableObjects/GameRound", order = 1)]
public class GameRound : ScriptableObject
{
    //ho ajuntaria per� necessito el puto ODIN per fer-ho :^)
    public GameObject[] squadsToSpawn;
    public Vector2[] positions;
    public int resources;

    public void SpawnEnemies()
    {
        for(int i = 0; i < squadsToSpawn.Length; i++)
        {
            float posX = (-positions[i].x * GridController.TileWidth + positions[i].y * GridController.TileWidth) / 2f;
            float posY = 0.125f + (-positions[i].x * GridController.TileHeight - positions[i].y * GridController.TileHeight + -0.5f) / 2f;

            GameObject squad = Instantiate(squadsToSpawn[i], new Vector2 (posX, posY), Quaternion.identity, null);
        }
    }
}
