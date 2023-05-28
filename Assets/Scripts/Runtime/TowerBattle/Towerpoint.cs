using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Towerpoint : MonoBehaviour
{
    [Header("Attributes")]
    public bool hideDebug;
    [SerializeField] private Vector2 scale = Vector2.one;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private int rows = 1;
    [SerializeField] private int columns = 1;
    [SerializeField] private Vector2 startingPoint = Vector2.zero;
    [SerializeField] private Vector2 gridScale = Vector2.one;
    [SerializeField] private List<Vector3> points;

    public Vector3[] Points => points.Select(p => new Vector3(p.x * scale.x, p.y * scale.y, 0f)).ToArray();
    public Vector3 Scale => scale;
    public Vector3 CurrentPosition => _currentPosition;
    
    private Vector3 _currentPosition;
    private bool _gameStarted;
    
    private void Start()
    {
        _gameStarted = true;
        _currentPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        if (hideDebug) return;

        points.Clear();

        for (int i = 0; i < rows * columns; i++)
        {
            // Point Positioning
            if (i == 0)
            {
                points.Add(startingPoint);
            }
            else
            {
                // 0, 0 1, 0 2, 0 3, 0 4, 0
                // 0,-1 1,-1 2,-1 3,-1 4,-1
                // 0,-2 1,-2 2,-2 3,-2 4,-2

                float x = startingPoint.x + (i % columns) * gridScale.x;
                float y = startingPoint.y + -1 * (Mathf.Floor(i / columns)) * gridScale.y;
                points.Add(new Vector3(x, y, 0f));
            }

            // Gizmos Modifiers
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(Points[i], radius * scale.x);
        }
    }
}
