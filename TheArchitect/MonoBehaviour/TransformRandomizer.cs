using UnityEngine;

/// Randomizes start transform of the object based on a list of locations
public class TransformRandomizer : MonoBehaviour
{
    [System.Serializable]
    public struct StartPoint
    {
        public Vector3 Position;
        public Vector3 Rotation;
    }

    [SerializeField] public StartPoint[] StartPoints;

    void Start()
    {
        int i = Random.Range(0, StartPoints.Length);
        this.transform.position = this.StartPoints[i].Position;
        this.transform.rotation = Quaternion.Euler(this.StartPoints[i].Rotation);
        Destroy(this);
    }

    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = new Color(1, 0, 0, 0.75F);
        Vector3 v = this.transform.position;
        foreach (var s in this.StartPoints)
        {
            #if UNITY_EDITOR
            Gizmos.DrawLine(v, s.Position);
            Gizmos.DrawSphere(s.Position, 0.25f);
            #endif
        }
    }
}