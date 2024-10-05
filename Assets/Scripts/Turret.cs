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

    public void Start()
    {
        healthSystem.onDie += RemoveFromGrid;
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

}
