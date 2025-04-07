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

    private float _surfaceTensionDecelerationFactorLight = 0.5f;
    private float _surfaceTensionDecelerationFactorHeavy = 0.25f;

    private Vector3 _lightGravity = new Vector3(0.0f, -25.0f, 0.0f);
    private Vector3 _heavyGravity = new Vector3(0.0f, -50.0f, 0.0f);

    private bool _wasSubmergedLastFrame = false;
    public bool breachedThisFrame = false;

    [SerializeField]
    private GameObject _splashParticle;

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

        this.buoyantRigidbody.AddForce(this.GetBuoyancyDirection() * this.CalculateBuoyancyForce(), ForceMode.Force);

        if (this.IsSubmerged() == false && this.isHeavy == true)
        {
            Physics.gravity = this._heavyGravity;
        }
        else
        {
            Physics.gravity = this._lightGravity;
        }

        if (this.breachedThisFrame == true)
        {
            this.breachedThisFrame = false;
        }

        if (this._wasSubmergedLastFrame == true && this.IsSubmerged() == false)
        {
            this.breachedThisFrame = true;
            this._wasSubmergedLastFrame = false;            
        }
        else
        {
            this._wasSubmergedLastFrame = this.IsSubmerged();
        }
    }

    private Vector3 GetBuoyancyDirection()
    {
        Vector3 totalBuoyancyDirection = Vector3.zero;

        for (int i = 0; i < this._currentLiquidObjects.Count; i++)
        {

            totalBuoyancyDirection += this._currentLiquidObjects[i].buoyancyDirection;
        }

        return totalBuoyancyDirection;
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

        Vector3 spawnPoint = other.ClosestPoint(this._transform.position);

        this.SpawnEnterSplashParticle(spawnPoint);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Liquid")
        {
            return;
        }

        this._currentLiquidObjects.Remove(other.gameObject.GetComponent<LiquidObject>());

        Vector3 spawnPoint = other.ClosestPoint(this._transform.position);

        this.SpawnExitSplashParticle(spawnPoint);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Liquid")
        {
            return;
        }   
    }

    public void SpawnEnterSplashParticle(Vector3 spawnPoint)
    {
        GameObject splashInstance = Instantiate(this._splashParticle, spawnPoint, new Quaternion());
        splashInstance.transform.up = this.buoyantRigidbody.velocity.normalized;
    }

    public void SpawnExitSplashParticle(Vector3 spawnPoint)
    {
        GameObject splashInstance = Instantiate(this._splashParticle, spawnPoint, new Quaternion());
        splashInstance.transform.up = -this.buoyantRigidbody.velocity.normalized;
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

    public float GetSubmergePercentage()
    {
        float finalSubmergedValue = 0.0f;

        for (int i = 0; i < this._currentLiquidObjects.Count; i++)
        {
            finalSubmergedValue += this._currentLiquidObjects[i].displacementFactor;
        }

        return finalSubmergedValue;
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
