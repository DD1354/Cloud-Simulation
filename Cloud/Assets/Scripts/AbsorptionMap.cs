using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorptionMap : MonoBehaviour
{
    [SerializeField] private Texture2D xy;

    [SerializeField] private float densityModifier;
    [SerializeField] private Vector3 bounds = Vector3.one;
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

    public float GetAbsorption (Vector3 position)
    {
        float grayscale = 1;
        if(xy == null) {
            return grayscale * densityModifier;
        }
        
        Vector3 samplePosition = transform.InverseTransformPoint(position);
        Vector2 uvPosition = new Vector2(samplePosition.x / bounds.x * xy.width,
                                            samplePosition.y / bounds.y * xy.height);
        grayscale = xy.GetPixel(Mathf.RoundToInt(uvPosition.x), 
            Mathf.RoundToInt(uvPosition.y)).grayscale * densityModifier;

        float concentration = 1 - grayscale;
        return concentration;
    }
}
