using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidObject : MonoBehaviour
{
    public float liquidDensity = 1.0f;
    public float displacementFactor = 0.0f;

    public Collider liquidCollider;

    private void Awake()
    {
        this.liquidCollider = GetComponent<Collider>();
    }

    public void CalculateDisplacement(float xBoundSize, float yBoundSize, Vector3 penetrationVector)
    {
        float xPenetrationFactor = (Mathf.Abs(penetrationVector.x) / xBoundSize);

        float yPenetrationFactor = (Mathf.Abs(penetrationVector.y) / yBoundSize);

        float totalPenetrationFactor = xPenetrationFactor + yPenetrationFactor;

        if (totalPenetrationFactor > 1.0f)
        {
            totalPenetrationFactor = 1.0f;
        }

        this.displacementFactor = totalPenetrationFactor;
    }
}
