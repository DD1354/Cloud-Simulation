using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class lightBehavior : MonoBehaviour
{
    [SerializeField] private int m_segmentCount = 20; //idk
    [SerializeField] private float m_rayLength = 4f;

    [SerializeField] private Vector3 m_origin;
    [SerializeField] private Vector3 m_dir;
    [SerializeField] private GameObject cloud;
    LineRenderer lr;
    private Vector3[] raySegments;

    private DensityMap dm;
    private GradientAlphaKey[] gak;
    private float currentAlpha = 1f;
    private float originAlpha = 1f;



    // Start is called before the first frame update
    void Start()
    {
        dm = cloud.GetComponent<DensityMap>();
        lr = GetComponent<LineRenderer>();   
    }

    // Update is called once per frame
    void Update()
    {
        RenderLine();
    }

    void RenderLine()
    {
        m_origin = transform.position;
        float stepLength = m_rayLength/m_segmentCount;
        lr.positionCount = m_segmentCount;
        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;
        
        Gradient gradient = new Gradient();

        lr.SetPosition(0, m_origin);
        gak = new GradientAlphaKey[3];
        gak[0].alpha = originAlpha;
        gak[0].time = 0.0f;

        for(int i = 1; i < m_segmentCount; i++){
            Vector3 newPos = lr.GetPosition(i-1) + (m_dir.normalized * stepLength);
            lr.SetPosition(i, newPos);
            
            // Temporary solution for illustrating effect uwu
            float density = dm.GetDensity(newPos);
            Debug.Log(density);
            if(density < 1){
                currentAlpha = density;
            }

        }
        
            // Also temporary
            gak[1].alpha = currentAlpha;
            gak[1].time = 0.5f;
            gak[2].alpha = currentAlpha;
            gak[2].time = 1f;

        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            gak
        );
        
        lr.colorGradient = gradient;
    }
}
