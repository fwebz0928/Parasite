using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Level", menuName = "Levels", order = 0)]
public class LevelData : ScriptableObject
{
    public string LevelName = " ";
    public float level_duration = 120;
    public List<FEnemySpawnData> spawnable_enemies = new List<FEnemySpawnData>();
    public List<Sprite> WallSprites = new List<Sprite>();
}

[System.Serializable]
public struct FEnemySpawnData
{
    public float SpawnChance;
    public Enemy EnemyPrefab;
}
