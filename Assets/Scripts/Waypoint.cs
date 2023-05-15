using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private Vector3[] points;

    public Vector3[] Points => points;
    public Vector3 CurrentPosition => _currentPosition;
    
    private Vector3 _currentPosition;
    private bool _gameStarted;
    
    private void Start()
    {
        _gameStarted = true;
        _currentPosition = transform.position;
    }

    public Vector3 GetWaypointPosition(int index)
    {
        return CurrentPosition + Points[index];
    }

    public ((float, float), Vector3) GetWaypointsBounds()
    {
        var minX = points.ToList().Select(p => p.x).Min();
        var maxY = points.ToList().Select(p => p.y).Max();
        var maxX = points.ToList().Select(p => p.x).Max();
        var minY = points.ToList().Select(p => p.y).Min();

        return ((maxX - minX, maxY - minY), new Vector3((minX + maxX) / 2, (maxY + minY) / 2, 0f));
    }

    private void OnDrawGizmos()
    {
        if (!_gameStarted && transform.hasChanged)
        {
            _currentPosition = transform.position;
        }

        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(points[i] + _currentPosition, 0.5f);

            if (i >= points.Length - 1) continue;
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(points[i] + _currentPosition, points[i + 1] + _currentPosition);
        }
    }
}
