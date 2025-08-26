using UnityEngine;

public class BoardVisualizer : MonoBehaviour
{
    public float cellSize = 2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Vector3 pos = transform.position + new Vector3(x * cellSize, 0, y * cellSize);
                Gizmos.DrawWireCube(pos, Vector3.one * 1.8f);
            }
        }
    }
}
