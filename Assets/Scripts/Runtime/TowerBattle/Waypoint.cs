using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Attributes")]
    public bool hideDebug;
    [SerializeField] private Vector3 scale = Vector3.one;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private List<Vector3> points;

    public List<Vector3> Points => points.Select(p => new Vector3(p.x * scale.x, p.y * scale.y, p.z * scale.z)).ToList();
    public Vector3 Scale => scale;
    public Vector3 CurrentPosition => _currentPosition;
    
    private Vector3 _currentPosition;
    private bool _gameStarted;
    private float _defaultRadius;
    private List<float> _distanceBetweenPoints = new List<float>();
    
    private void Awake()
    {
        for(int i = 1; i < points.Count; i++)
        {
            _distanceBetweenPoints.Add((points[i] - points[i - 1]).magnitude);
        }
    }

    private void Start()
    {
        _gameStarted = true;
        _currentPosition = transform.position;
    }

    public Vector3 GetWaypointPosition(int index)
    {
        return CurrentPosition + Points[index];
    }

    public float GetDistanceBetweenPoints(int index)
    {
        return index < 1 || index >= _distanceBetweenPoints.Count ? 0 : _distanceBetweenPoints[index];
    }

    public float GetTotalDistanceFromIndex(int waypointIndex)
    {
        if (waypointIndex < 0 || waypointIndex >= _distanceBetweenPoints.Count) return 0;

        float total = 0;
        for(int i = 0; i < waypointIndex; i++)
        {
            total += _distanceBetweenPoints[i];
        }

        return total;
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
        if (hideDebug) return;

        if (!_gameStarted && transform.hasChanged)
        {
            _currentPosition = transform.position;
        }

        for (int i = 0; i < Points.Count; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(Points[i] + _currentPosition, radius * scale.x);

            if (i >= Points.Count - 1) continue;
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(Points[i] + _currentPosition, Points[i + 1] + _currentPosition);
        }
    }
}
