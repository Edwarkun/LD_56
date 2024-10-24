using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Combination[] combinations;
    public int ID;

    public Combination originCombination;

    public List<Tuple<int, int>> positions = new List<Tuple<int, int>>();

    public HealthSystem healthSystem;

    public GameObject debriPrefab;

    public void Start()
    {
        healthSystem.onDie += RemoveFromGrid;
        healthSystem.onDie += SpawnDebris;
        AudioManager.Instance.PlaySounds("Buildings_ConstructSmall");
    }

    public void CalculatePositions(Tuple<int, int> startPosition)
    {
        if (originCombination == null)
        {
            positions.Add(startPosition);
        }
        else {
            foreach (var pos in originCombination.layout)
                positions.Add(Tuple.Create(startPosition.Item1 + (int)pos.x, startPosition.Item2 + (int)pos.y));
        }
    }

    private void RemoveFromGrid()
    {
        foreach (var position in positions)
            GridController.Instance.placeables.Remove(position);
    }
    private void SpawnDebris()
    {
        if (originCombination == null)
            GridController.Instance.debris.Enqueue(Instantiate(debriPrefab, this.transform.position, Quaternion.identity, null));
        else
            foreach (var pos in originCombination.layout)
            {
                float posX = (-pos.x * GridController.TileWidth + pos.y * GridController.TileWidth) / 2f;
                float posY = 0.125f + (-pos.x * GridController.TileHeight - pos.y * GridController.TileHeight + -0.5f) / 2f;

                GridController.Instance.debris.Enqueue(Instantiate(debriPrefab, this.transform.position + new Vector3(posX, posY, 0.0f), Quaternion.identity, null));
            }
    }

}
