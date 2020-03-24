using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    public GameObject sparkEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.tag == "BULLET")
        {
            GameObject spark = (GameObject)Instantiate(sparkEffect, coll.transform.position, Quaternion.identity);

            Destroy(spark, spark.GetComponent<ParticleSystem>().duration + 1.5f);

            Destroy(coll.gameObject);
        }
    }
}
