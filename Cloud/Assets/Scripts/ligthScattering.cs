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
    private float[] samplePoints;
    
    private Vector3 origin = new Vector3(0,0,0);


    // Start is called before the first frame update
    void Start()
    {
        float sampleIncrease = (2*Mathf.PI) / noOfSamples;
        samplePoints = new float[noOfSamples];
        for(int i = 0; i < noOfSamples; i++){
            samplePoints[i] = i*sampleIncrease;
        }

        lr = GetComponent<LineRenderer>();   
    }

    // Update is called once per frame
    void Update()
    {
        ScatterLight();
    }
    
    void ScatterLight(){
        
        foreach(float x in samplePoints){
            Vector3 line = new Vector3(0, Mathf.Sin(x), Mathf.Cos(x));
            float dist = henyeyGreenstein(origin.x + x);
            line *= dist;
            Debug.DrawLine(origin, line, Color.white);
            if(threeDimension){
                foreach(float y in samplePoints){
                    Debug.DrawLine(origin, Quaternion.Euler(0, 0, y * Mathf.Rad2Deg) * line, Color.white);
                }
            }
            //Debug.Log(henyeyGreenstein(origin.x + xVal));
        }

        // Call henyey with a sampled x position (relative to the dir of the light ray)
        // return is the dir in both y and z
        // Point of origin is the point from which we're referencing
    }

    float henyeyGreenstein(float x){
        return  (1 / (4 * Mathf.PI)) * 
                (1 - Mathf.Pow(m_scatter,2)) / 
                Mathf.Pow((1 + Mathf.Pow(m_scatter,2) - (2 * m_scatter * Mathf.Cos(x))),(3 / 2));
    }
}
