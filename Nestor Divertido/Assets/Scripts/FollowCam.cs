using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float minMoveDistance = 0.1f; // Umbral mínimo para el movimiento

    void Start()
    {
        if (offset == Vector3.zero)
        {
            offset = transform.position - player.position;
        }
    }

    void LateUpdate()
    {
        Vector3 targetPosition = player.position + offset;
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > minMoveDistance)
        {
            transform.position = targetPosition;
        }
    }
}
