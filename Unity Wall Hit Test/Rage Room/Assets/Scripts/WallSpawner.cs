using Meta.XR.MRUtilityKit;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public GameObject wallPrefab;
    public Transform rightHand;
    public Transform playerCamera; // CenterEyeAnchor
    public float spawnDistance = 2f;
    public float verticalOffset = -0.25f;

    void Update()
    {
        // A button (Right controller) = Spawn wall
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            SpawnWall();
        }

        // X button (Left controller) = Destroy all walls
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            DestroyAllWalls();
        }
    }

    void SpawnWall()
    {
        Vector3 spawnPos =
            rightHand.position +
            rightHand.forward * spawnDistance +
            Vector3.up * verticalOffset;

        GameObject wall = Instantiate(wallPrefab, spawnPos, Quaternion.identity);
        wall.tag = "Wall";

        // Rotate wall to face the player (yaw only)
        Vector3 lookDirection = playerCamera.position - wall.transform.position;
        lookDirection.y = 0;
        wall.transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    void DestroyAllWalls()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            Destroy(wall);
        }
    }
}
