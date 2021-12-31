using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBounds : MonoBehaviour
{
    public Transform levelStart;
    public Transform levelEnd;

    private Vector3 levelStartPosition;
    private Vector3 levelEndPosition;

    public Rect bounds;

    private void Awake()
    {
        levelStartPosition = levelStart.position;
        levelEndPosition = levelEnd.position;
        bounds.xMin = levelStartPosition.x;
        bounds.yMin = levelStartPosition.y;
        bounds.xMax = levelEndPosition.x;
        bounds.yMax = levelEndPosition.y;
    }

    private void OnDrawGizmos()
    {
        if (levelStart != null)
            levelStartPosition = levelStart.position;
        if (levelEnd != null)
            levelEndPosition = levelEnd.position;
        Gizmos.color = Color.blue;
        Vector3 levelSize = levelEndPosition - levelStartPosition;
        levelSize.x = Mathf.Abs(levelSize.x);
        levelSize.y = Mathf.Abs(levelSize.y);
        Gizmos.DrawWireCube((levelStartPosition + levelEndPosition) / 2, levelSize);
    }

}
