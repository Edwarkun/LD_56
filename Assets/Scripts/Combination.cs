using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combination", menuName = "ScriptableObjects/Combination", order = 1)]
public class Combination : ScriptableObject
{
    public Vector2[] layout;
    public int[] towerID;
    public GameObject towerToSpawn;
}
