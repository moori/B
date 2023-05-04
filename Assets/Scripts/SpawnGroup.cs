using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpawnGroup : ScriptableObject
{
    public List<SpawnData> spawns;
    public float delayBetweenSpawns;
    public int minLevel;
    public int maxLevel;
    public float waveDuration;
}
