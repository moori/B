using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WaveData : ScriptableObject
{
    public List<SpawnData> mainSequence;
    public List<SpawnData> randomSpawns;
    public List<SpawnData> bosses;
    public int maxEnemies;
    public float spawnDelay;



    public enum Spawnable
    {
        StaticMinion,
        Stalker
    }

    [System.Serializable]
    public class SpawnData
    {
        public Spawnable spawnable;
        public Vector3 point;
    }
}
