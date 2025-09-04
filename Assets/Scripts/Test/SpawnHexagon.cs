using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnHexagon : MonoBehaviour
{
    [SerializeField] private LayerMask _layerHexCannotSpawnOn;
    [SerializeField] private Collider2D _spawnableAreaCool;
    public GameObject HexToSpawn;

    private List<GameObject> spawnedHexagons = new List<GameObject>();
    
    public void SpawnHex()
    {
        Vector2 spawnPosition = GetRandomSpawnPosition(_spawnableAreaCool);
        GameObject spawnedHex = Instantiate(HexToSpawn, spawnPosition, Quaternion.identity);
        spawnedHexagons.Add(spawnedHex);
    }

    public Vector2 GetRandomSpawnPosition(Collider2D spawnableAreaCollider)
    {
        Vector2 spawnPosition = Vector2.zero;
        bool isSpawnPosValid = false;

        int attemptCount = 0;
        int maxAttempts = 200;

        while (!isSpawnPosValid && attemptCount < maxAttempts)
        {
            spawnPosition = GetRandomPointInCollider(spawnableAreaCollider);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 2f, _layerHexCannotSpawnOn);

            bool isInvalidCollision = false;

            foreach (Collider2D collider in colliders)
            {
                isInvalidCollision = true;
                break;
            }

            if (!isInvalidCollision)
            {
                isSpawnPosValid = true;
            }

            attemptCount++;
        }

        if (!isSpawnPosValid)
        {
            Debug.LogWarning("无法找到有效的生成位置");
        }

        return spawnPosition;
    }
    
    public Vector2 GetRandomPointInCollider(Collider2D collider)
    {
        Bounds bounds = collider.bounds;

        Vector2 minBounds = new Vector2(bounds.min.x, bounds.min.y);
        Vector2 maxBounds = new Vector2(bounds.max.x, bounds.max.y);

        float randomX = Random.Range(minBounds.x, maxBounds.x);
        float randomY = Random.Range(minBounds.y, maxBounds.y);

        return new Vector2(randomX, randomY);
    }

    public void SpawnClear()
    {
        foreach (GameObject hex in spawnedHexagons)
        {
            if (hex != null)
            {
                DestroyImmediate(hex);
            }
        }
        spawnedHexagons.Clear();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpawnHexagon))]
public class SpawnHexagonCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        EditorGUILayout.Space(15f);
        
        SpawnHexagon spawn = (SpawnHexagon)target;

        if (GUILayout.Button("Spawn Hexaogon"))
        {
            spawn.SpawnHex();
        }
        
        if (GUILayout.Button("Spawn Clear"))
        {
            spawn.SpawnClear();
        }
    }
}
#endif
