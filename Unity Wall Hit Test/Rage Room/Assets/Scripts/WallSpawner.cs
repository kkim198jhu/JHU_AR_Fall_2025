using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public GameObject wallPrefab;
    public Transform rightHand;
    public Transform playerCamera; // CenterEyeAnchor
    public float spawnDistance = 2f;

    //  NEW: adjust height of spawned objects
    public float verticalOffset = -0.25f;

    private bool hasSpawned = false;

    void Update()
    {
        float trigger = OVRInput.Get(
            OVRInput.Axis1D.PrimaryIndexTrigger,
            OVRInput.Controller.RTouch
        );

        if (trigger > 0.8f && !hasSpawned)
        {
            SpawnWall();
            hasSpawned = true;
        }
        
        if (trigger < 0.1f)
        {
            hasSpawned = false;
        }
    }

    void SpawnWall()
    {
        Vector3 spawnPos =
            rightHand.position +
            rightHand.forward * spawnDistance +
            Vector3.up * verticalOffset;

        GameObject wall = Instantiate(wallPrefab, spawnPos, Quaternion.identity);

        // Rotate wall to face the player (yaw only)
        Vector3 lookDirection = playerCamera.position - wall.transform.position;
        lookDirection.y = 0;

        wall.transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}
