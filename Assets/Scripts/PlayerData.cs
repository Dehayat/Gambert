using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    public static bool hasCheckPoint;
    public static Vector3 checkPoint;

    public static void SetCheckPoint(Vector3 position)
    {
        hasCheckPoint = true;
        checkPoint = position;
    }
}
