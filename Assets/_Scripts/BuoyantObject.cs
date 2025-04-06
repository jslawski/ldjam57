using System.Collections.Generic;
using UnityEngine;

public class BuoyantObject : MonoBehaviour
{
    public bool isHeavy = false;

    private float _displacementSize = 50.0f;

    private Transform _transform;

    public Rigidbody buoyantRigidbody;

    private Collider _collider;

    private List<LiquidObject> _currentLiquidObjects;

    [SerializeField]
    private AnimationCurve _surfaceTensionCurve;
    
    private float _lowDisplacementValue = 50f;    
    private float _highDisplacementValue = 75f;   
    private float _lowMass = 20f;   
    private float _highMass = 100f;

    private float _surfaceTensionDecelerationFactorLight = 0.75f;
    private float _surfaceTensionDecelerationFactorHeavy = 0.5f;

    

    private Vector3 _lightGravity = new Vector3(0.0f, -50.0f, 0.0f);
    private Vector3 _heavyGravity = new Vector3(0.0f, -100.0f, 0.0f);

    private void Awake()
    {
        this._transform = GetComponent<Transform>();
        this._collider = GetComponent<Collider>();
        this.buoyantRigidbody = GetComponent<Rigidbody>();
        this._currentLiquidObjects = new List<LiquidObject>();
    }

    private void FixedUpdate()
    {
        this.UpdateDisplacementFactors();    

        this.buoyantRigidbody.AddForce(Vector3.up * this.CalculateBuoyancyForce(), ForceMode.Force);

        if (this.IsSubmerged() == false && this.isHeavy == true)
        {
            Physics.gravity = this._heavyGravity;
        }
        else
        {
            Physics.gravity = this._lightGravity;
        }
    }

    private float CalculateBuoyancyForce()
    {
        float totalBuoyancyForce = 0.0f;

        for (int i = 0; i < this._currentLiquidObjects.Count; i++) 
        {
            float displacementVolume = this._currentLiquidObjects[i].displacementFactor * this._displacementSize;
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

    public bool IsFullySubmerged()
    {
        float finalSubmergedValue = 0.0f;

        for (int i = 0; i < this._currentLiquidObjects.Count; i++)
        {
            finalSubmergedValue += this._currentLiquidObjects[i].displacementFactor;
        }

        return (finalSubmergedValue >= 1.0f);
    }

    public void ChangeToLightObject()
    {
        this._displacementSize = this._highDisplacementValue;       
        this.buoyantRigidbody.mass = this._lowMass;
        this.isHeavy = false;
    }

    public void ChangeToHeavyObject()
    {
        this._displacementSize = this._lowDisplacementValue;        
        this.buoyantRigidbody.mass = this._highMass;
        this.isHeavy = true;
    }

    public void ApplyImpactDecelerationForce()
    {
        float tValue = this.buoyantRigidbody.velocity.magnitude / PlayerController._maxUnsubmergedVelocity.magnitude;
        float decelerationAmount = this._surfaceTensionCurve.Evaluate(tValue);

        float decelerationFactor = 0.0f;
        float newVelocity = this.buoyantRigidbody.velocity.magnitude;

        if (this.isHeavy == false)
        {            
            decelerationFactor = this._surfaceTensionDecelerationFactorLight;            
            newVelocity = this.buoyantRigidbody.velocity.magnitude - (this.buoyantRigidbody.velocity.magnitude * (decelerationAmount * decelerationFactor));
        }
        else
        {         
            decelerationFactor = this._surfaceTensionDecelerationFactorHeavy;            
            newVelocity = this.buoyantRigidbody.velocity.magnitude - (this.buoyantRigidbody.velocity.magnitude * (decelerationAmount * decelerationFactor));
        }

        this.buoyantRigidbody.velocity = this.buoyantRigidbody.velocity.normalized * newVelocity;        
    }
}
