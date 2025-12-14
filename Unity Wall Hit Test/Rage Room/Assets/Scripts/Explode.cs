using UnityEngine;

public class Explode : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ParticleSystem explosion1; //ALL PREFABS
    public ParticleSystem explosion2;
    public Rigidbody rb;
    
    public ParticleSystem spark;
    public float expThreshold = 2f;


    //private MeshCollider()
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision col)
    {
        Vector3 vel = rb.linearVelocity; // standard Rigidbody velocity
        Vector3 hitPoint = col.GetContact(0).point;
        Quaternion rot = Quaternion.LookRotation(col.GetContact(0).normal);

        if (vel.magnitude > expThreshold)
        {
            SpawnAndPlay(explosion1, hitPoint, rot);
            SpawnAndPlay(explosion2, hitPoint, rot);

            Destroy(gameObject, 0.1f);
        }
        else
        {
            SpawnAndPlay(spark, hitPoint, rot);
        }



    }

    void SpawnAndPlay(ParticleSystem prefab, Vector3 pos, Quaternion rot)
    {
        if (prefab == null) return;

        ParticleSystem ps = Instantiate(prefab, pos, rot);
        ps.Play();

        float life = ps.main.duration;
        if (ps.main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
            life += ps.main.startLifetime.constantMax;
        else
            life += ps.main.startLifetime.constant;

        Destroy(ps.gameObject, .1f);






    }
}
