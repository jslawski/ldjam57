using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private BuoyantObject _buoyantObject;

    [SerializeField]
    private MeshRenderer _renderer;

    [SerializeField]
    private Material _lightMaterial;
    [SerializeField]
    private Material _heavyMaterial;

    public static Vector3 _maxUnsubmergedVelocity = new Vector3(7.0f, 14.0f, 0.0f);
    private Vector3 _maxSubmergedVelocity = new Vector3(15.0f, 5.0f, 0.0f);

    public float _unsubmergedAcceleration = 5.0f;
    public float _submergedAcceleration = 10.0f;

    private Vector3 _moveDirection = Vector3.zero;

    //Momentum conservation
    private Vector3 _lastSubmergedVelocity = Vector3.zero;
    private Vector3 _maxDeceleratingVelocity = Vector3.zero;
    private float _timeToDecelerate = 0.1f;
    private Coroutine _decelerationCoroutine = null;
    private int _currentBreachFrames = 0;
    private int _breachBuffer = 2;

    [SerializeField]
    private AnimationCurve _dragCurve;

    private float _maxDrag = 5.0f;


    private void Awake()
    {
        this._buoyantObject = GetComponent<BuoyantObject>();
        this._maxDeceleratingVelocity = PlayerController._maxUnsubmergedVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        this.ProcessInputs();
    }

    private void UpdateDrag()
    {
        float tValue = this._buoyantObject.GetSubmergePercentage();
    
        this._buoyantObject.buoyantRigidbody.drag = this._dragCurve.Evaluate(tValue) * this._maxDrag;
    }

    private void ProcessInputs()
    {
        this._moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            this._moveDirection = Vector3.left;           
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            this._moveDirection = Vector3.right;                  
        }
        
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            this._renderer.material = this._heavyMaterial;
            this._buoyantObject.ChangeToHeavyObject();
        }
        else
        {
            this._renderer.material = this._lightMaterial;
            this._buoyantObject.ChangeToLightObject();
        }
    }

    private void FixedUpdate()
    {
        if (this._buoyantObject.IsFullySubmerged() == false)
        {
            this._buoyantObject.buoyantRigidbody.AddForce(this._moveDirection * this._unsubmergedAcceleration, ForceMode.Acceleration);           
        }
        else
        {
            this._buoyantObject.buoyantRigidbody.AddForce(this._moveDirection * this._submergedAcceleration, ForceMode.Acceleration);
            this._lastSubmergedVelocity = this._buoyantObject.buoyantRigidbody.velocity;
        }

        this.CapMaxVelocity();
    }

    private void CapMaxVelocity()
    {        
        this.CapMaxVelocityX();
        this.CapMaxVelocityY();
    }

    private void CapMaxVelocityX()
    {
        Rigidbody currentRigidbody = this._buoyantObject.buoyantRigidbody;
        Vector3 currentVelocity = this._buoyantObject.buoyantRigidbody.velocity;

        if (this._buoyantObject.IsFullySubmerged() == true)
        {
            if (Mathf.Abs(currentVelocity.x) > this._maxSubmergedVelocity.x)
            {
                if (currentVelocity.x > 0)
                {
                    currentRigidbody.velocity = new Vector3(this._maxSubmergedVelocity.x, currentVelocity.y, 0.0f);
                }
                else
                {
                    currentRigidbody.velocity = new Vector3(-this._maxSubmergedVelocity.x, currentVelocity.y, 0.0f);
                }
            }
        }
        else if (this._buoyantObject.IsSubmerged() && this._buoyantObject.wasFullySubmergedLastFrame == true)
        {
            if (this._currentBreachFrames > 0)
            {
                this._currentBreachFrames = 0;
            }
        }
        else
        {
            if (Mathf.Abs(currentRigidbody.velocity.x) > this._maxDeceleratingVelocity.x)
            {
                if (this._buoyantObject.breachedThisFrame == true && this._decelerationCoroutine == null)
                {
                    this._maxDeceleratingVelocity = new Vector3(Mathf.Abs(this._lastSubmergedVelocity.x), PlayerController._maxUnsubmergedVelocity.y, 0.0f);
                    this._decelerationCoroutine = StartCoroutine(this.DecelerateFromBreach());
                    this._lastSubmergedVelocity = PlayerController._maxUnsubmergedVelocity;
                }

                if (this._currentBreachFrames >= this._breachBuffer)
                {
                    if (currentVelocity.x > 0)
                    {
                        currentRigidbody.velocity = new Vector3(this._maxDeceleratingVelocity.x, currentVelocity.y, 0.0f);
                    }
                    else
                    {
                        currentRigidbody.velocity = new Vector3(-this._maxDeceleratingVelocity.x, currentVelocity.y, 0.0f);
                    }
                }
            }
        }        
    }
    
    private void CapMaxVelocityY()
    {
        Rigidbody currentRigidbody = this._buoyantObject.buoyantRigidbody;
        Vector3 currentVelocity = this._buoyantObject.buoyantRigidbody.velocity;

        if (this._buoyantObject.IsFullySubmerged() == true)
        {
            if (this._buoyantObject.isHeavy == true && Mathf.Abs(currentVelocity.y) > this._maxSubmergedVelocity.y)
            {
                if (currentVelocity.y > 0)
                {
                    currentRigidbody.velocity = new Vector3(currentVelocity.x, this._maxSubmergedVelocity.y, 0.0f);
                }
                else
                {
                    currentRigidbody.velocity = new Vector3(currentVelocity.x, -this._maxSubmergedVelocity.y, 0.0f);
                }
            }
            else if (Mathf.Abs(currentVelocity.y) > this._maxSubmergedVelocity.y)
            {
                if (Mathf.Abs(currentRigidbody.velocity.y) > PlayerController._maxUnsubmergedVelocity.y)
                {
                    if (currentVelocity.y > 0)
                    {
                        currentRigidbody.velocity = new Vector3(currentVelocity.x, PlayerController._maxUnsubmergedVelocity.y, 0.0f);
                    }
                    else
                    {
                        currentRigidbody.velocity = new Vector3(currentVelocity.x, -PlayerController._maxUnsubmergedVelocity.y, 0.0f);
                    }

                }
            }
        }
        else
        {
            if (Mathf.Abs(currentRigidbody.velocity.y) > PlayerController._maxUnsubmergedVelocity.y)
            {
                if (currentVelocity.y > 0)
                {
                    currentRigidbody.velocity = new Vector3(currentVelocity.x, PlayerController._maxUnsubmergedVelocity.y, 0.0f);
                }
                else
                {
                    currentRigidbody.velocity = new Vector3(currentVelocity.x, -PlayerController._maxUnsubmergedVelocity.y, 0.0f);
                }
            }
        }
    }

    private IEnumerator DecelerateFromBreach()
    {
        Rigidbody currentRigidbody = this._buoyantObject.buoyantRigidbody;
        Vector3 currentVelocity = this._buoyantObject.buoyantRigidbody.velocity;

        float decelerationPerFrame = ((this._maxDeceleratingVelocity.x - PlayerController._maxUnsubmergedVelocity.x) / this._timeToDecelerate) * Time.fixedDeltaTime;

        while (this._buoyantObject.IsFullySubmerged() == false && this._maxDeceleratingVelocity.x > PlayerController._maxUnsubmergedVelocity.x)
        {
            this._currentBreachFrames++;
            
            this._maxDeceleratingVelocity = new Vector3(this._maxDeceleratingVelocity.x - decelerationPerFrame, this._maxDeceleratingVelocity.y, 0.0f);

            yield return new WaitForFixedUpdate();
        }

        this._maxDeceleratingVelocity = PlayerController._maxUnsubmergedVelocity;
        this._decelerationCoroutine = null;
        this._currentBreachFrames = this._breachBuffer;
    }
}
