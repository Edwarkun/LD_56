using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelRounds", menuName = "ScriptableObjects/LevelRounds", order = 1)]
public class LevelRounds : ScriptableObject
{
    public GameRound[] rounds;
    public int mapSizeX;
    public int mapSizeY;
}
