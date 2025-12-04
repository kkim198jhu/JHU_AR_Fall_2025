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

        // clamp to available spawn points
        itemCount = Mathf.Clamp(itemCount, 0, spawnPoints.Count);

        // shuffle spawn indices
        List<int> indices = new List<int>();
        for (int i = 0; i < spawnPoints.Count; i++)
            indices.Add(i);
        for (int i = 0; i < indices.Count; i++)
        {
            int j = Random.Range(i, indices.Count);
            int temp = indices[i];
            indices[i] = indices[j];
            indices[j] = temp;
        }

        // spawn items on the first itemCount shuffled points
        for (int k = 0; k < itemCount; k++)
        {
            Transform point = spawnPoints[indices[k]];
            if (point == null) continue;

            GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];
            GameObject instance = Instantiate(prefab, point.position, point.rotation);

            // make AR-grabbable
            MakeGrabbable(instance);
        }
    }

    private void MakeGrabbable(GameObject obj)
    {
        // add collider if missing
        if (obj.GetComponent<Collider>() == null)
            obj.AddComponent<BoxCollider>();

        // add Rigidbody if missing
        if (obj.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        // mark for AR grabbing
        obj.tag = "Grabbable";
    }
}
