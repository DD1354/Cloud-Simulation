using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class ligthScattering : MonoBehaviour
{
    private LineRenderer lr;
    [SerializeField][Range(-1f, 1f)]
    private float m_scatter = 0.5f;

    [SerializeField]
    private bool threeDimension;

    [SerializeField]
    private int noOfSamples = 20;
    private float[] sampleAngles;
    
    private Vector3 origin = new Vector3(0,0,0);


    // Start is called before the first frame update
    void Start()
    {
        float sampleIncrease = (2*Mathf.PI) / noOfSamples;
        sampleAngles = new float[noOfSamples];
        for(int i = 0; i < noOfSamples; i++){
            sampleAngles[i] = i*sampleIncrease;
        }

        lr = GetComponent<LineRenderer>();   
    }

    // Update is called once per frame
    void Update()
    {
        ScatterLight();
    }
    
    void ScatterLight(){
        
        foreach(float angle in sampleAngles){
            Vector3 line = new Vector3(0, Mathf.Sin(angle), Mathf.Cos(angle));
            float dist = henyeyGreenstein(angle);
            line *= dist;
            Debug.DrawLine(origin, line, Color.white);
            if(threeDimension){
                foreach(float y in sampleAngles){
                    Debug.DrawLine(origin, Quaternion.Euler(0, 0, y * Mathf.Rad2Deg) * line, Color.white);
                }
            }
            //Debug.Log(henyeyGreenstein(origin.angle + angleVal));
        }

        // Call henyey with a sampled angle position (relative to the dir of the light ray)
        // return is the dir in both y and z
        // Point of origin is the point from which we're referencing
    }

    float henyeyGreenstein(float angle){
        return  (1 / (4 * Mathf.PI)) * 
                (1 - Mathf.Pow(m_scatter,2)) / 
                Mathf.Pow((1 + Mathf.Pow(m_scatter,2) - (2 * m_scatter * Mathf.Cos(angle))),(3 / 2));
    }
}
