using Meta.XR.MRUtilityKit;
using UnityEditor;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public GameObject wallPrefab;
    public Transform rightHand;
    public Transform playerCamera; // CenterEyeAnchor
    public float spawnDistance = 2f;
    public OVRInput.RawButton showHideButton;

    // Adjust height of spawned objects
    public float verticalOffset = -0.25f;

    void Update()
    {
        // A button on right controller = Button.One
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            SpawnWall();
        }
        
        //if (OVRInput.GetDown(showHideButton))
        //{
            
        //    GameObject[] instances = PrefabUtility.FindAllInstancesOfPrefab(wallPrefab);

        //    foreach (GameObject instance in instances)
        //    {

        //        Destroy(instance, Random.value);
        //    }
        //}

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
