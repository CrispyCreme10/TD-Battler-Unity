using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Towerpoint : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private Vector3 scale = Vector3.one;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private Vector3[] points;

    public Vector3[] Points => points.Select(p => new Vector3(p.x * scale.x, p.y * scale.y, p.z * scale.z)).ToArray();
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
        if (!_gameStarted && transform.hasChanged)
        {
            _currentPosition = transform.position;
        }

        for (int i = 0; i < Points.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(Points[i] + _currentPosition, radius * scale.x);
        }
    }
}
