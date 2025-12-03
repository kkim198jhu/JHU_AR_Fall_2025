using Meta.XR.MRUtilityKit;
using UnityEngine;
using static Meta.XR.MRUtilityKit.MRUKAnchor;



public class WallGen : MonoBehaviour
{

    void Start()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom(); 

        foreach (MRUKAnchor anc in room.Anchors)
        {
            if (anc.HasLabel("WALL_FACE"))
            {
                var go = anc.gameObject;

                if (!go.TryGetComponent<Collider>(out _))
                    go.AddComponent<BoxCollider>(); 

            }
        }
    }
}