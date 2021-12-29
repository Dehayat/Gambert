using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static void RotateTowards(Vector3 targetPosition, Transform transformToMove)
    {
        if (transformToMove == null)
        {
            return;
        }
        Vector3 lookDirection = targetPosition - transformToMove.position;
        lookDirection.z = 0;
        lookDirection.Normalize();
        float rotationAngle = Vector3.SignedAngle(Vector3.up, lookDirection, Vector3.forward);
        transformToMove.rotation = Quaternion.AngleAxis(rotationAngle, Vector3.forward);
    }

    public static Vector2 CalcVelocity(Vector2 distance, float initalSpeed, float gravityScale = 1f)
    {
        float timeToDestination = -Mathf.Sqrt(initalSpeed * initalSpeed + 2f * distance.y * Physics2D.gravity.y * gravityScale);
        timeToDestination -= initalSpeed;
        timeToDestination /= Physics2D.gravity.y * gravityScale;

        float xVelocity = distance.x / timeToDestination;
        return new Vector2(xVelocity, initalSpeed);
    }
}
