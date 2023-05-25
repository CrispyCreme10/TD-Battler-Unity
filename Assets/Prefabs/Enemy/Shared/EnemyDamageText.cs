using UnityEngine;

public class EnemyDamageText : MonoBehaviour
{
    public float DestroyTime = 3f;
    public Vector3 Offset = new Vector3(1, 2, 0);

    private void Start()
    {
        Destroy(gameObject, DestroyTime);

        transform.localPosition += Offset; 
    }
}
