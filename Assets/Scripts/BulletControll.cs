using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControll : MonoBehaviour
{
    public int bulletDamage = 35;
    public float bulletSpeed = 3000.0f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed * 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
