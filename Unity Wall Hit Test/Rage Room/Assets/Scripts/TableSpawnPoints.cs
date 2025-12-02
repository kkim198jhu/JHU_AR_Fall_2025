using System.Collections.Generic;
using UnityEngine;

public class TableSpawnPoints : MonoBehaviour
{
    public List<Transform> spawnPoints = new List<Transform>();

    // called from RageRoomSpawner to put items on the table
    public void SpawnItems(List<GameObject> itemPrefabs, int itemCount)
    {
        if (itemPrefabs == null || itemPrefabs.Count == 0)
        {
            Debug.LogWarning("TableSpawnPoints: no item prefabs assigned.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("TableSpawnPoints: no spawn points assigned.");
            return;
        }

        // we won't spawn more items than we have slots
        itemCount = Mathf.Clamp(itemCount, 0, spawnPoints.Count);

        // make a shuffled list of spawn point indices so we use random slots
        List<int> indices = new List<int>();
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            indices.Add(i);
        }

        for (int i = 0; i < indices.Count; i++)
        {
            int j = Random.Range(i, indices.Count);
            int temp = indices[i];
            indices[i] = indices[j];
            indices[j] = temp;
        }

        // fill the first itemCount shuffled slots with random items
        for (int k = 0; k < itemCount; k++)
        {
            Transform point = spawnPoints[indices[k]];
            if (point == null) continue;

            GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];
            Instantiate(prefab, point.position, point.rotation);
        }
    }
}
