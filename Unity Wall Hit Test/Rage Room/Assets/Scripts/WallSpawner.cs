using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public GameObject wallPrefab;
    public Transform rightHand;
    public Transform playerCamera; // CenterEyeAnchor
    public float spawnDistance = 2f;

    private bool hasSpawned = false;

    void Update()
    {
        float trigger = OVRInput.Get(
            OVRInput.Axis1D.PrimaryIndexTrigger,
            OVRInput.Controller.RTouch
        );

        // spawn only once per pull
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
        Vector3 spawnPos = rightHand.position + rightHand.forward * spawnDistance;

        GameObject wall = Instantiate(wallPrefab, spawnPos, Quaternion.identity);

        // Rotate wall to face the player (yaw only)
        Vector3 lookDirection = playerCamera.position - wall.transform.position;
        lookDirection.y = 0; // keep the wall upright

        wall.transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}
