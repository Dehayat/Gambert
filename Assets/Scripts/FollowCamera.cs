using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float dampTimeX = 0.2f;
    public float dampTimeY = 0.2f;
    public float maxSpeedX = 15f;
    public float maxSpeedY = 10f;
    public Vector2 targetOffset = Vector2.zero;
    public Vector2 fallingOffset = Vector2.zero;
    public float fallMinSpeed = -10f;
    public float fallLerpDuration = 0.2f;
    public float returnFromOffsetTimeX = 0.1f;

    private List<CameraZone> zones;
    private CameraZone currentZone = null;
    public void EnterZone(CameraZone cameraZone)
    {
        if (!zones.Contains(cameraZone))
        {
            zones.Add(cameraZone);
            currentZone = cameraZone;
        }
    }
    public void ExitZone(CameraZone cameraZone)
    {
        if (zones.Contains(cameraZone))
        {
            zones.Remove(cameraZone);
        }
        if (zones.Count > 0)
        {
            currentZone = zones[zones.Count - 1];
        }
        else
        {
            currentZone = null;
        }
    }

    public float returnFromOffsetTimeY = 0.1f;
    public Camera cam;

    private Rigidbody2D targetRB;


    private void Awake()
    {
        targetRB = target.GetComponent<Rigidbody2D>();
        zones = new List<CameraZone>();
    }

    private float savedZ;
    private Rect levelBounds;
    private void Start()
    {
        savedZ = transform.position.z;
        currentOffset = targetOffset;
        levelBounds = FindObjectOfType<LevelBounds>().bounds;
        transform.position = ClampPositionInLevel(target.position);
    }


    private Vector3 dampVelocityX = Vector3.zero;
    private Vector3 dampVelocityY = Vector3.zero;
    private Vector2 currentOffset;
    private float dampOffsetX = 0f;
    private float dampOffsetY = 0f;
    private float fallTimer = 0;
    private void FixedUpdate()
    {
        Vector3 targetPosition = target.position;
        targetPosition = ClampPositionInLevel(targetPosition);
        targetPosition = ClampPositionInZone(targetPosition);
        if (targetRB.velocity.y < fallMinSpeed)
        {
            if (fallTimer < fallLerpDuration)
            {
                fallTimer += Time.fixedDeltaTime;
            }
            currentOffset = Vector2.Lerp(targetOffset, fallingOffset, fallTimer / fallLerpDuration);
        }
        else
        {
            fallTimer = 0f;
            currentOffset.x = Mathf.SmoothDamp(currentOffset.x, targetOffset.x, ref dampOffsetX, returnFromOffsetTimeX);
            currentOffset.y = Mathf.SmoothDamp(currentOffset.y, targetOffset.y, ref dampOffsetY, returnFromOffsetTimeY);
        }
        targetPosition.x += currentOffset.x;
        targetPosition.y += currentOffset.y;
        targetPosition.z = savedZ;
        Vector3 targetX = transform.position;
        targetX.x = targetPosition.x;
        Vector3 targetY = transform.position;
        targetY.y = targetPosition.y;
        Vector3 destinationX = Vector3.SmoothDamp(transform.position, targetX, ref dampVelocityX, dampTimeX, maxSpeedX);
        Vector3 destinationY = Vector3.SmoothDamp(transform.position, targetY, ref dampVelocityY, dampTimeY, maxSpeedY);
        transform.position = new Vector3(destinationX.x, destinationY.y, savedZ);

        FixZ();
    }

    private Vector3 ClampPositionInLevel(Vector3 targetPosition)
    {
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        float minX = targetPosition.x - width / 2;
        float maxX = targetPosition.x + width / 2;
        float minY = targetPosition.y - height / 2;
        float maxY = targetPosition.y + height / 2;
        if (minY < levelBounds.yMin)
        {
            minY = levelBounds.yMin;
            maxY = minY + height;
        }
        if (maxY > levelBounds.yMax)
        {
            maxY = levelBounds.yMax;
            minY = maxY - height;
        }
        if (minX < levelBounds.xMin)
        {
            minX = levelBounds.xMin;
            maxX = minX + width;
        }
        if (maxX > levelBounds.xMax)
        {
            maxX = levelBounds.xMax;
            minX = maxX - width;
        }
        //minX = Mathf.Clamp(minX, levelBounds.xMin, levelBounds.xMax);
        //maxX = Mathf.Clamp(maxX, levelBounds.xMin, levelBounds.xMax);
        //minY = Mathf.Clamp(minY, levelBounds.yMin, levelBounds.yMax);
        //maxY = Mathf.Clamp(maxY, levelBounds.yMin, levelBounds.yMax);
        float centerX = (minX + maxX) / 2;
        float centerY = (minY + maxY) / 2;
        return new Vector3(centerX, centerY, targetPosition.z);
    }

    private Vector3 ClampPositionInZone(Vector3 targetPosition)
    {
        if (currentZone == null) return targetPosition;
        if (currentZone.forcePosition)
        {
            return currentZone.camPosition.position;
        }
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        float minX = targetPosition.x - width / 2;
        float maxX = targetPosition.x + width / 2;
        float minY = targetPosition.y - height / 2;
        float maxY = targetPosition.y + height / 2;

        if (currentZone.limitMinY && minY < currentZone.bounds.yMin)
        {
            minY = currentZone.bounds.yMin;
            maxY = minY + height;
        }
        if (currentZone.limitMaxY && maxY > currentZone.bounds.yMax)
        {
            maxY = currentZone.bounds.yMax;
            minY = maxY - height;
        }
        if (currentZone.limitMinX && minX < currentZone.bounds.xMin)
        {
            minX = currentZone.bounds.yMin;
            maxX = minX + width;
        }
        if (currentZone.limitMaxX && maxX > currentZone.bounds.xMax)
        {
            maxX = currentZone.bounds.xMax;
            minX = maxX - width;
        }
        float centerX = (minX + maxX) / 2;
        float centerY = (minY + maxY) / 2;
        return new Vector3(centerX, centerY, targetPosition.z);
    }
    private void FixZ()
    {
        Vector3 position = transform.position;
        position.z = savedZ;
        transform.position = position;
    }

    public void SnapToTarget()
    {
        transform.position = ClampPositionInZone(ClampPositionInLevel(target.position));
    }
}
