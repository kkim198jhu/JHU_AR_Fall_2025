using System.Collections.Generic;
using UnityEngine;

public class RageRoomSpawner : MonoBehaviour
{
    [Header("Rage object prefabs")]
    public List<GameObject> rageObjectPrefabs;
    public List<GameObject> weaponsPrefabs;

    [Header("Spawn origin")]
    public Transform spawnOrigin;        // AR Camera (child of AR Session Origin)
    public float forwardDistance = 1.5f;
    public float verticalOffset = -0.3f;

    [Header("Limits")]
    public int maxSimultaneousObjects = 50;

    [Header("Table spawning")]
    public GameObject tablePrefab;
    public float tableForwardDistance = 0f;
    public float tableVerticalOffset = -5.0f;
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
        // X button on right Meta Quest controller = Button.Three
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            SpawnOneItemOnTable();
        }

        // Y button on right Meta Quest controller = Button.Four
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            SpawnWeapon();
        }
    }

    private void SpawnWeapon(){
        if (weaponsPrefabs == null || weaponsPrefabs.Count == 0)
        {
            Debug.LogWarning("No weapon prefabs assigned!");
            return;
        }

        // Pick random weapon
        GameObject weaponPrefab = weaponsPrefabs[Random.Range(0, weaponsPrefabs.Count)];

        // Choose spawn origin (right hand preferred)
        Transform origin = rightHand != null ? rightHand : spawnOrigin;

        Vector3 spawnPos =
            origin.position +
            origin.forward * 0.3f +
            Vector3.up * 0.15f;

        Quaternion spawnRot = origin.rotation;

        GameObject instance = Instantiate(weaponPrefab, spawnPos, spawnRot);

        MakeGrabbable(instance);
        spawnedObjects.Add(instance);
    }


    private void SpawnOneItemOnTable()
    {
        if (rageObjectPrefabs == null || rageObjectPrefabs.Count == 0){
            Debug.LogWarning("No rage object prefabs assigned!");
            return;
        }

        if (tableSpawnPoints == null || tableSpawnPoints.Length == 0){
            Debug.LogWarning("No table spawn points assigned!");
            return;
        }

        // Pick random spawn point
        Transform point = tableSpawnPoints[Random.Range(0, tableSpawnPoints.Length)];
        GameObject prefab = rageObjectPrefabs[Random.Range(0, rageObjectPrefabs.Count)];

        // Spawn slightly above the table
        Vector3 spawnPos = point.position + Vector3.up * 0.5f;
        GameObject instance = Instantiate(prefab, spawnPos, point.rotation);

        // Make grabbable + track
        MakeGrabbable(instance);
        spawnedObjects.Add(instance);
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

            GameObject instance = Instantiate(prefab,
                tableSpawnPoints[i].position + Vector3.up * 0.5f,
                tableSpawnPoints[i].rotation);
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
