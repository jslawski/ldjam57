using System.Collections;
using System.Collections.Generic;
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

    public static Vector3 _maxUnsubmergedVelocity = new Vector3(7.0f, 15.0f, 0.0f);
    private Vector3 _maxSubmergedVelocity = new Vector3(15.0f, 5.0f, 0.0f);

    public float _unsubmergedAcceleration = 5.0f;
    public float _submergedAcceleration = 10.0f;

    private Vector3 _moveDirection = Vector3.zero;

    [SerializeField]
    private AnimationCurve _dragCurve;

    private float _maxDrag = 5.0f;
    
    private void Awake()
    {
        this._buoyantObject = GetComponent<BuoyantObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.ProcessInputs();
        //this.UpdateDrag();
    }

    private void UpdateDrag()
    {
        float tValue = this._buoyantObject.GetSubmergePercentage();
    
        this._buoyantObject.buoyantRigidbody.drag = this._dragCurve.Evaluate(tValue) * this._maxDrag;

        /*
        if (this._buoyantObject.IsFullySubmerged() == true)
        {
            this._buoyantObject.buoyantRigidbody.drag = this._fullySubmergedDrag;
        }
        else if (this._buoyantObject.IsSubmerged() == true)
        {
            this._buoyantObject.buoyantRigidbody.drag = this._fullySubmergedDrag;
        }
        else
        {
            this._buoyantObject.buoyantRigidbody.drag = this._unsubmergedDrag;
        }
        */
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
        else
        {
            if (Mathf.Abs(currentRigidbody.velocity.x) > PlayerController._maxUnsubmergedVelocity.x)
            {
                if (currentVelocity.x > 0)
                {
                    currentRigidbody.velocity = new Vector3(PlayerController._maxUnsubmergedVelocity.x, currentVelocity.y, 0.0f);
                }
                else
                {
                    currentRigidbody.velocity = new Vector3(-PlayerController._maxUnsubmergedVelocity.x, currentVelocity.y, 0.0f);
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
}
