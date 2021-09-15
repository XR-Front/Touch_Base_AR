using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp_particleTest : MonoBehaviour
{
    float rValue;
    bool addition = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float t = (Mathf.Sin(Time.time));

        gameObject.GetComponent<ParticleSystem>().startColor = Color.Lerp(Color.magenta, Color.cyan, t); ;
    }
}
