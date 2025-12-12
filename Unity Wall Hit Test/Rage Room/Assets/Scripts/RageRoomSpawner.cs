using System.Collections.Generic;
using UnityEngine;

public class RageRoomSpawner : MonoBehaviour
{
    [Header("Rage object prefabs")]
    public List<GameObject> rageObjectPrefabs;

    [Header("Spawn origin")]
    public Transform spawnOrigin;        // AR Camera (child of AR Session Origin)
    public float forwardDistance = 1.5f;
    public float verticalOffset = -0.2f;

    [Header("Limits")]
    public int maxSimultaneousObjects = 50;

    [Header("Table spawning")]
    public GameObject tablePrefab;
    public float tableForwardDistance = 0f;
    public float tableVerticalOffset = -3.0f;
    public int itemsOnTable = 6;

    [Header("One-item spawning")]
    public Transform[] tableSpawnPoints; // Assign the table spawn points here
    public Transform rightHand;           // Optional, for reference

    private readonly List<GameObject> spawnedObjects = new List<GameObject>();
    private Transform currentTable;

    private void Start()
    {
        SpawnTableWithItems();
    }

    private void Update()
    {
        // B button on right Meta Quest controller = Button.Two
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            SpawnOneItemOnTable();
        }
    }

    // -----------------------------------------
    // Spawn ONE item on the table
    // -----------------------------------------
    private void SpawnOneItemOnTable()
    {
        if (tableSpawnPoints == null || tableSpawnPoints.Length == 0)
        {
            Debug.LogWarning("No table spawn points assigned!");
            return;
        }

        // Find first empty spawn point
        foreach (Transform point in tableSpawnPoints)
        {
            if (point.childCount == 0) // empty spot
            {
                int index = Random.Range(0, rageObjectPrefabs.Count);
                GameObject prefab = rageObjectPrefabs[index];

                GameObject instance = Instantiate(prefab, point.position, point.rotation, point);
                MakeGrabbable(instance);
                spawnedObjects.Add(instance);
                return; // stop after spawning ONE
            }
        }

        Debug.Log("All table spots are filled!");
    }

    public void SpawnTableWithItems()
    {
        if (tablePrefab == null)
        {
            Debug.LogWarning("RageRoomSpawner: No tablePrefab assigned.");
            return;
        }

        if (rageObjectPrefabs == null || rageObjectPrefabs.Count == 0)
        {
            Debug.LogWarning("RageRoomSpawner: No rageObjectPrefabs assigned for table items.");
            return;
        }

        Transform origin = spawnOrigin != null ? spawnOrigin : Camera.main.transform;

        Vector3 tablePosition =
            origin.position +
            origin.forward * tableForwardDistance +
            Vector3.up * tableVerticalOffset;

        Quaternion tableRotation = Quaternion.Euler(0f, origin.eulerAngles.y, 0f) * tablePrefab.transform.rotation;

        if (currentTable != null)
        {
            Destroy(currentTable.gameObject);
            currentTable = null;
        }

        GameObject tableInstance = Instantiate(tablePrefab, tablePosition, tableRotation);
        currentTable = tableInstance.transform;

        // Optional: assign table spawn points automatically
        if (tableSpawnPoints == null || tableSpawnPoints.Length == 0)
        {
            tableSpawnPoints = tableInstance.GetComponentsInChildren<Transform>(); 
            // Note: filter child transforms if needed
        }

        // Spawn initial items
        int count = Mathf.Min(itemsOnTable, tableSpawnPoints.Length);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, rageObjectPrefabs.Count);
            GameObject prefab = rageObjectPrefabs[index];

            GameObject instance = Instantiate(prefab, tableSpawnPoints[i].position, tableSpawnPoints[i].rotation, tableSpawnPoints[i]);
            MakeGrabbable(instance);
            spawnedObjects.Add(instance);
        }
    }

    private void MakeGrabbable(GameObject obj)
    {
        if (obj.GetComponent<Collider>() == null)
            obj.AddComponent<BoxCollider>();

        if (obj.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        obj.tag = "Grabbable";
    }
}
