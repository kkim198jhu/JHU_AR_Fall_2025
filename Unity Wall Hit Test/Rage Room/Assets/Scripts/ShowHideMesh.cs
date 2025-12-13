using UnityEngine;
using Meta.XR.MRUtilityKit;
public class ShowHideMesh : MonoBehaviour
{
    public OVRInput.RawButton showHideButton;

    private EffectMesh effectMesh;

    private void Start()
    {
        effectMesh = GetComponent<EffectMesh>();
    }


    private void Update()
    {
        if (OVRInput.GetDown(showHideButton))
        {
            effectMesh.HideMesh = !effectMesh.HideMesh;
        }
    }



}
