using UnityEngine;

public class Bounce : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ParticleSystem explosion1;
    public ParticleSystem explosion2;
    public Rigidbody rb;
    
    public ParticleSystem spark;
    public float expThreshold = 10f;
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
        Vector3 vel = rb.linearVelocity;
        if (vel.magnitude > expThreshold)
        {
            explosion1.transform.position = col.contacts[0].point;
            explosion1.Play();
            if (explosion2 != null)
            {
                explosion2.transform.position = col.contacts[0].point;
                explosion2.Play();
            }
            Destroy(gameObject);
        }
        else
        {
            if (spark != null)
            {
                spark.transform.position = col.contacts[0].point;
                spark.Play();
            }
        }
        

        
    }
}
