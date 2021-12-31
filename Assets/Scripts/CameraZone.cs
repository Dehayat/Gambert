using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public bool forcePosition;
    public bool limitMinY;
    public bool limitMaxY;
    public bool limitMinX;
    public bool limitMaxX;
    public Transform zoneMin;
    public Transform zoneMax;
    public Transform camPosition;

    private FollowCamera cam;

    public Rect bounds;

    private void Awake()
    {

        Vector3 startPos = zoneMin.position;
        Vector3 endPos = zoneMax.position;
        bounds.xMin = startPos.x;
        bounds.yMin = startPos.y;
        bounds.xMax = endPos.x;
        bounds.yMax = endPos.y;
    }

    private void Start()
    {
        cam = FindObjectOfType<FollowCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var rb = collision.attachedRigidbody;
        if (rb != null && rb.CompareTag("Player"))
        {
            cam.EnterZone(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var rb = collision.attachedRigidbody;
        if (rb != null && rb.CompareTag("Player"))
        {
            cam.ExitZone(this);
        }
    }

    private void OnDrawGizmos()
    {
        if (zoneMin == null || zoneMax == null) return;
        Gizmos.color = Color.cyan;
        Vector3 center = (zoneMin.position + zoneMax.position) / 2;
        Vector3 size = zoneMax.position - zoneMin.position;
        if (forcePosition)
        {
            Gizmos.DrawWireSphere(camPosition.position, 1.5f);
            return;
        }
        if (limitMinY)
        {
            Vector3 minCenter = center;
            minCenter.y = zoneMin.position.y;
            Vector3 minSize = size;
            minSize.y = 0.3f;
            Gizmos.DrawWireCube(minCenter, minSize);
        }
        if (limitMaxY)
        {
            Vector3 minCenter = center;
            minCenter.y = zoneMax.position.y;
            Vector3 minSize = size;
            minSize.y = 0.3f;
            Gizmos.DrawWireCube(minCenter, minSize);
        }
        if (limitMinX)
        {
            Vector3 minCenter = center;
            minCenter.x = zoneMin.position.x;
            Vector3 minSize = size;
            minSize.x = 0.3f;
            Gizmos.DrawWireCube(minCenter, minSize);
        }
        if (limitMaxX)
        {
            Vector3 minCenter = center;
            minCenter.x = zoneMax.position.x;
            Vector3 minSize = size;
            minSize.x = 0.3f;
            Gizmos.DrawWireCube(minCenter, minSize);
        }
    }
}
