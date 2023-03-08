using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensityMap : MonoBehaviour
{
    [SerializeField] private Texture2D xy;

    [SerializeField] private float densityModifier;
    [SerializeField] private Vector3 bounds = Vector3.one;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetDensity (Vector3 position)
    {
        float density = 1;
        if(xy == null) {
            return density * densityModifier;
        }
        
        Vector3 samplePosition = transform.InverseTransformPoint(position);
        Vector2 uvPosition = new Vector2(samplePosition.x / bounds.x * xy.width,
                                            samplePosition.y / bounds.y * xy.height);
        density = xy.GetPixel(Mathf.RoundToInt(uvPosition.x), 
            Mathf.RoundToInt(uvPosition.y)).grayscale * densityModifier;
        return 1-density;
    }
}
