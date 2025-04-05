using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuoyantObject : MonoBehaviour
{
    public float displacementSize = 50.0f;

    private Transform _transform;

    public Rigidbody rigidbody;

    private Collider _collider;

    private List<LiquidObject> _currentLiquidObjects;

    [SerializeField]
    private AnimationCurve _surfaceTensionCurve;

    [SerializeField]
    private float _lowDisplacementValue = 10f;
    [SerializeField]
    private float _highDisplacementValue = 50f;
    [SerializeField]
    private float _lowMass = 20f;
    [SerializeField]
    private float _highMass = 200f;

    private float _surfaceTensionDecelerationFactorLight = 0.75f;
    private float _surfaceTensionDecelerationFactorHeavy = 0.5f;

    private float _maxVelocity = 20.0f;

    private void Awake()
    {
        this._transform = GetComponent<Transform>();
        this._collider = GetComponent<Collider>();
        this.rigidbody = GetComponent<Rigidbody>();
        this._currentLiquidObjects = new List<LiquidObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        this.UpdateDisplacementFactors();    

        this.rigidbody.AddForce(Vector3.up * this.CalculateBuoyancyForce(), ForceMode.Force);

        if (this.rigidbody.velocity.magnitude > this._maxVelocity)
        {
            this.rigidbody.velocity = (this.rigidbody.velocity.normalized * this._maxVelocity);
        }
    }

    private float CalculateBuoyancyForce()
    {
        float totalBuoyancyForce = 0.0f;

        for (int i = 0; i < this._currentLiquidObjects.Count; i++) 
        {
            float displacementVolume = this._currentLiquidObjects[i].displacementFactor * this.displacementSize;
            float buoyancyForce = this._currentLiquidObjects[i].liquidDensity * Physics.gravity.magnitude * displacementVolume;
            totalBuoyancyForce += buoyancyForce;
        }

        return totalBuoyancyForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Liquid")
        {
            return;
        }

        this._currentLiquidObjects.Add(other.gameObject.GetComponent<LiquidObject>());

        this.ApplyImpactDecelerationForce();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Liquid")
        {
            return;
        }

        this._currentLiquidObjects.Remove(other.gameObject.GetComponent<LiquidObject>());
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Liquid")
        {
            return;
        }   
    }

    private void UpdateDisplacementFactors()
    {
        Vector3 penetrationDirection = Vector3.zero;
        float penetrationDistance = 0.0f;

        for (int i = 0; i < this._currentLiquidObjects.Count; i++)
        {
        
            Physics.ComputePenetration(this._collider, this._transform.position, this._transform.rotation,
                                        this._currentLiquidObjects[i].liquidCollider, this._currentLiquidObjects[i].gameObject.transform.position, this._currentLiquidObjects[i].gameObject.transform.rotation,
                                        out penetrationDirection, out penetrationDistance);

            this._currentLiquidObjects[i].CalculateDisplacement(this._collider.bounds.size.x, this._collider.bounds.size.y, penetrationDirection * penetrationDistance);
        }
    }

    public bool IsSubmerged()
    { 
        return (this._currentLiquidObjects.Count > 0);
    }

    public void ChangeToLightObject()
    {
        this.displacementSize = this._highDisplacementValue;       
        this.rigidbody.mass = this._lowMass;
    }

    public void ChangeToHeavyObject()
    {
        this.displacementSize = this._lowDisplacementValue;        
        this.rigidbody.mass = this._highMass;
    }

    public void ApplyImpactDecelerationForce()
    {
        float tValue = this.rigidbody.velocity.magnitude / this._maxVelocity;
        float decelerationAmount = this._surfaceTensionCurve.Evaluate(tValue);

        float decelerationFactor = 0.0f;
        float newVelocity = this.rigidbody.velocity.magnitude;

        if (this.displacementSize >= this._highDisplacementValue)
        {            
            decelerationFactor = this._surfaceTensionDecelerationFactorLight;            
            newVelocity = this.rigidbody.velocity.magnitude - (this.rigidbody.velocity.magnitude * (decelerationAmount * decelerationFactor));
        }
        else
        {         
            decelerationFactor = this._surfaceTensionDecelerationFactorHeavy;            
            newVelocity = this.rigidbody.velocity.magnitude - (this.rigidbody.velocity.magnitude * (decelerationAmount * decelerationFactor));
        }
        /*
        Debug.LogError("TValue:" + tValue +
            "\nDecelerationAmount: " + decelerationAmount +
            "\nPercant to Decelerate" + (decelerationFactor * decelerationAmount) +
            "\nVelocity to Subtract: " + (this.rigidbody.velocity.magnitude * (decelerationAmount * decelerationFactor)));
            */
        //Debug.LogError("Before: " + this.rigidbody.velocity.magnitude + "\nAfter: " + newVelocity);

        this.rigidbody.velocity = this.rigidbody.velocity.normalized * newVelocity;        
    }
}
