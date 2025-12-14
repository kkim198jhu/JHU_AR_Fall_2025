using UnityEngine;
using UnityEngine.VFX;

public class Splash : MonoBehaviour
{
    [Header("VFX Graph prefabs (VisualEffect on the prefab root)")]
    [SerializeField] private VisualEffect explosion1Prefab;
    [SerializeField] private VisualEffect explosion2Prefab;
    [SerializeField] private ParticleSystem sparkPrefab;


    public float expThreshold = 2f;
    private float vfxLifetime = 2f;
    private float startTime = -1f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    private void Update()
    {
        if (startTime == -1f)
            startTime = Time.time;
    }

    void OnCollisionEnter(Collision col)
    {
        
        Vector3 vel = rb != null ? rb.linearVelocity : Vector3.zero;

        ContactPoint cp = col.GetContact(0);
        Vector3 hitPoint = cp.point;
        Quaternion hitRot = Quaternion.LookRotation(cp.normal);
        if (Time.time - startTime > 3f)
        {
            if (vel.magnitude > expThreshold)
            {
                SpawnAndPlayVFX(explosion1Prefab, hitPoint, hitRot);
                SpawnAndPlayVFX(explosion2Prefab, hitPoint, hitRot);

            }
            else
            {
                ParticleSystem ps = Instantiate(sparkPrefab, hitPoint, hitRot);
                ps.Play();
                Destroy(ps.gameObject, .1f);
            }
        }
    }

    private void SpawnAndPlayVFX(VisualEffect prefab, Vector3 pos, Quaternion rot)
    {
        if (prefab == null) return;

        VisualEffect vfx = Instantiate(prefab, pos, rot);

        //vfx.SendEvent("OnPlay");

        vfx.Play();

        Destroy(vfx.gameObject, vfxLifetime);
    }
}