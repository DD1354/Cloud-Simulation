using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorptionMap : MonoBehaviour
{
    [SerializeField] private Texture2D xy;

    [SerializeField] private Vector3 bounds = Vector3.one;
    public Vector3 Bounds { get { return bounds; } }
    [SerializeField] private float concentrationFactor = 1.0f;
    [SerializeField] private float attenuationFactor = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetAbsorption (Vector2 position)
    {

        float grayscale = 1;
        if(xy == null) {
            return 0;
        }

        Vector2 samplePosition = transform.InverseTransformPoint(position);
        samplePosition.x += 0.5f;
        samplePosition.y += 0.5f;

        Vector2 uvPosition = new Vector2(samplePosition.x / bounds.x * xy.width,
                                            samplePosition.y / bounds.y * xy.height);
        grayscale = xy.GetPixel(Mathf.RoundToInt(uvPosition.x), 
            Mathf.RoundToInt(uvPosition.y)).grayscale;
        float concentration = 1 - grayscale;
        return concentration*concentrationFactor*attenuationFactor;
    }
}
