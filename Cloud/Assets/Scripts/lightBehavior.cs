using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightBehavior : MonoBehaviour
{
    //[SerializeField] private int m_segmentCount = 20;
    [SerializeField] private float m_segmentLength = 0.01f;
    [SerializeField] private float m_rayLength = 4f;
    [SerializeField] private GameObject m_cloud;
    [SerializeField] private GameObject m_line;
    private GameObject[] m_lines;

    private float m_lineSize = 0;

    private AbsorptionMap m_absorptionMap;
    [SerializeField] private Color m_rayColor = Color.white;




    // Start is called before the first frame update
    void Start()
    {
        m_absorptionMap = m_cloud.GetComponent<AbsorptionMap>();
        m_lineSize = m_line.transform.localScale.z * 10;
        int totalLines = Mathf.RoundToInt(m_rayLength / m_lineSize);
        m_lines = new GameObject[totalLines];

        for (int i = 0; i < totalLines; i++)
        {
            m_lines[i] = Instantiate(m_line, transform.position, Quaternion.Euler(180, 0, -90));
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLines();
        RenderLine();
    }

    void UpdateLines()
    {
        Vector3 position = transform.position;
        for(int i = 0; i < m_lines.Length; i++)
        {
            position += m_lineSize * transform.forward;
            m_lines[i].transform.position = position;
        }
    }

    void ChangeLineColor(int from, int to, Color color)
    {
        for(int i = from; i < to; i++){
            m_lines[i].GetComponent<Renderer>().material.color = color; 
        }
    }

    void RenderLine()
    {
        Vector3 samplePosition = transform.position;
        
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit))
        {
            samplePosition = hit.point;
        }
        else
        {
            ChangeLineColor(0, m_lines.Length, m_rayColor);
            return;
        }

        int lastUpdatedLine = 0;
        int currentLine = Mathf.RoundToInt(hit.distance/m_lineSize);

        Color rayColor = m_rayColor;
        ChangeLineColor(lastUpdatedLine, currentLine, rayColor);


        //float stepLength = m_absorptionMap.Bounds.x / m_segmentCount;
        int steps = Mathf.RoundToInt(m_absorptionMap.Bounds.x / m_segmentLength);
        float riemannsum = 0;

        int linesPerStepLength = Mathf.RoundToInt(m_segmentLength / m_lineSize);
        lastUpdatedLine = currentLine;

        for (int i = 0; i < steps; i++){
            samplePosition += (transform.forward * m_segmentLength);
            if (i == 0)
                continue;
            float absorption = m_absorptionMap.GetAbsorption(new Vector2(samplePosition.z, samplePosition.y));

            riemannsum += absorption * m_segmentLength;
            Debug.Log(riemannsum);
            currentLine += linesPerStepLength;
            rayColor.a = Mathf.Pow(10, -riemannsum);
            ChangeLineColor(lastUpdatedLine, currentLine, rayColor);
            lastUpdatedLine = currentLine;
        }

        rayColor.a = Mathf.Pow(10, -riemannsum);
        ChangeLineColor(lastUpdatedLine, m_lines.Length, rayColor);
    }
}
