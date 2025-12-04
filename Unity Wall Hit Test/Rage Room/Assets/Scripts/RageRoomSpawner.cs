using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public float tableForwardDistance = 0f;  // in front of eyes
    public float tableVerticalOffset = -3.0f;  // below eye level
    public int itemsOnTable = 6;               // how many items to place

    [Header("XR Input")]
    public InputActionProperty leftYButtonAction; // assign in Inspector

    private readonly List<GameObject> spawnedObjects = new List<GameObject>();
    private Transform currentTable;

    private void Start()
    {
        // spawn a table and items when the scene starts
        SpawnTableWithItems();
    }

    private void OnEnable()
    {
        if (leftYButtonAction != null)
        {
            leftYButtonAction.action.Enable();
            leftYButtonAction.action.performed += OnLeftYPressed;
        }
    }

    private void OnDisable()
    {
        if (leftYButtonAction != null)
        {
            leftYButtonAction.action.performed -= OnLeftYPressed;
            leftYButtonAction.action.Disable();
        }
    }

    private void OnLeftYPressed(InputAction.CallbackContext context)
    {
        SpawnRandom();
    }

    public void SpawnRandom()
    {
        if (rageObjectPrefabs == null || rageObjectPrefabs.Count == 0)
        {
            Debug.LogWarning("RageRoomSpawner: No rageObjectPrefabs assigned.");
            return;
        }

        // clean up destroyed entries
        spawnedObjects.RemoveAll(o => o == null);

        // enforce max count
        if (spawnedObjects.Count >= maxSimultaneousObjects)
        {
            Destroy(spawnedObjects[0]);
            spawnedObjects.RemoveAt(0);
        }

        int index = Random.Range(0, rageObjectPrefabs.Count);
        GameObject prefab = rageObjectPrefabs[index];

        Transform origin = spawnOrigin != null ? spawnOrigin : Camera.main.transform;

        Vector3 spawnPos =
            origin.position +
            origin.forward * forwardDistance +
            Vector3.up * verticalOffset;

        Quaternion spawnRot = Random.rotation;

        // Spawn the object and make it grabbable
        GameObject instance = Instantiate(prefab, spawnPos, spawnRot);
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

        // have table face player
        Quaternion tableRotation = Quaternion.Euler(0f, origin.eulerAngles.y, 0f) * tablePrefab.transform.rotation;

        if (currentTable != null)
        {
            Destroy(currentTable.gameObject);
            currentTable = null;
        }

        GameObject tableInstance = Instantiate(tablePrefab, tablePosition, tableRotation);
        currentTable = tableInstance.transform;

        // ask the table to spawn items on its spawn points
        TableSpawnPoints tablePoints = tableInstance.GetComponent<TableSpawnPoints>();
        if (tablePoints == null)
        {
            Debug.LogWarning("RageRoomSpawner: table prefab has no TableSpawnPoints component.");
            return;
        }

        tablePoints.SpawnItems(rageObjectPrefabs, itemsOnTable);
    }

    /// Makes a spawned object AR-grabbable: adds Collider + Rigidbody and tags it for AR touch grabbing
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

        // mark for AR grabbing
        obj.tag = "Grabbable";
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnRandom();
        }
    }
#endif
}
