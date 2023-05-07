using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveData
{
    public List<SpawnGroup> spawnGroups;
    public float spawnDelay;
}

[System.Serializable]
public class SpawnData
{
    public Spawnable spawnable;
    public Region region;
    public Vector3 point;
    public float radius = 3f;

    public Vector3 GetPositionInRegion()
    {
        var pos = Vector3.zero;
        switch (region)
        {
            case Region.Point:
                pos = GameController.GetClosestPositionInsideBounds(point + new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius),0));
                break;
            case Region.Top:
                pos = new Vector3(Mathf.Clamp(pos.x, -GameController.HORIZONTAL_MOVEMENT_BOUND, GameController.HORIZONTAL_MOVEMENT_BOUND), Mathf.Clamp(pos.y, 2, GameController.VERTICAL_MOVEMENT_BOUND), 0);
                break;
            case Region.Bottom:
                pos = new Vector3(Mathf.Clamp(pos.x, -GameController.HORIZONTAL_MOVEMENT_BOUND, GameController.HORIZONTAL_MOVEMENT_BOUND), Mathf.Clamp(pos.y, -GameController.VERTICAL_MOVEMENT_BOUND, -2), 0);
                break;
            case Region.Left:
                pos = new Vector3(Mathf.Clamp(pos.x, -GameController.HORIZONTAL_MOVEMENT_BOUND, -3), Mathf.Clamp(pos.y, -GameController.VERTICAL_MOVEMENT_BOUND, GameController.VERTICAL_MOVEMENT_BOUND), 0);
                break;
            case Region.Right:
                pos = new Vector3(Mathf.Clamp(pos.x, 3, GameController.HORIZONTAL_MOVEMENT_BOUND), Mathf.Clamp(pos.y, -GameController.VERTICAL_MOVEMENT_BOUND, GameController.VERTICAL_MOVEMENT_BOUND), 0);
                break;
            case Region.Center:
                pos = new Vector3(Mathf.Clamp(pos.x, -4, 4), Mathf.Clamp(pos.y, -3, 3), 0);
                break;
            default:
            case Region.Random:
                pos = new Vector3(Random.Range(-GameController.HORIZONTAL_MOVEMENT_BOUND, GameController.HORIZONTAL_MOVEMENT_BOUND), Random.Range(-GameController.VERTICAL_MOVEMENT_BOUND, GameController.VERTICAL_MOVEMENT_BOUND), 0);
                break;
        }
        return pos;
    }
}

public enum Spawnable
{
    StaticMinion,
    Stalker,
    LasetTurret,
    MissileTurret
}

public enum Region
{
    Point,
    Random,
    Top,
    Bottom,
    Left,
    Right,
    Center
}