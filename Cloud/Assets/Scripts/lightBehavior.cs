using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class lightBehavior : MonoBehaviour
{
    [SerializeField] private int m_segmentCount = 20; //idk
    [SerializeField] private float m_rayLength = 4f;

    [SerializeField] private GameObject cloud;


    [SerializeField] private GameObject line;
    private GameObject[] lines;

    private AbsorptionMap am;




    // Start is called before the first frame update
    void Start()
    {
        am = cloud.GetComponent<AbsorptionMap>();
         
    }

    // Update is called once per frame
    void Update()
    {
        RenderLine();
    }

    void RenderLine()
    {
        Vector3 samplePosition = transform.position;
        float stepLength = m_rayLength/m_segmentCount;
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit))
        {
            samplePosition = hit.point;
        }
        else
        {
            return;
        }



        float riemannsum = 0;
        for (int i = 0; i < m_segmentCount; i++){
            samplePosition += (transform.forward * stepLength);

            if (i == 0)
                continue;

            float absorption = am.GetAbsorption(samplePosition);
       

        }
        
    }
}
