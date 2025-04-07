using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public BuoyantObject _buoyantObject;

    public static Vector3 _maxUnsubmergedVelocity = new Vector3(7.0f, 14.0f, 0.0f);
    private Vector3 _maxSubmergedVelocity = new Vector3(15.0f, 5.0f, 0.0f);

    public float _unsubmergedAcceleration = 5.0f;
    public float _submergedAcceleration = 10.0f;

    public Vector3 _moveDirection = Vector3.zero;

    public PlayerControls _playerControls;

    private float _timeSinceLastBuoyChange = 0.0f;
    private float _buoyChangeDelay = 0.0f;

    private Coroutine _delayCoroutine = null;

    private bool _setHeavy = false;

    private void Awake()
    {
        this._buoyantObject = GetComponent<BuoyantObject>();

        this._playerControls = new PlayerControls();

        this._playerControls.PlayerMap.Left.performed += this.MoveLeft;
        this._playerControls.PlayerMap.Left.canceled += this.StopDirection;
        this._playerControls.PlayerMap.Right.performed += this.MoveRight;
        this._playerControls.PlayerMap.Right.canceled += this.StopDirection;
        this._playerControls.PlayerMap.Down.performed += this.GoHeavy;
        this._playerControls.PlayerMap.Down.canceled += this.GoLight;
        this._playerControls.PlayerMap.Restart.performed += this.RestartLevel;
    }

    private void OnEnable()
    {
        this._playerControls.Enable();
    }

    private void OnDisable()
    {
        this._playerControls.Disable();
    }

    private void FixedUpdate()
    {
        if (this._setHeavy == true && this._buoyantObject.isHeavy == false)
        {
            this.ExecuteHeavy();
        }
        else if (this._setHeavy == false && this._buoyantObject.isHeavy == true)        
        {
            this.ExecuteLight();
        }
    
        if (this._buoyantObject.IsFullySubmerged() == false)
        {
            this._buoyantObject.buoyantRigidbody.AddForce(this._moveDirection * this._unsubmergedAcceleration, ForceMode.Acceleration);           
        }
        else
        {
            this._buoyantObject.buoyantRigidbody.AddForce(this._moveDirection * this._submergedAcceleration, ForceMode.Acceleration);
        }        

        this.CapMaxVelocity();

        this._timeSinceLastBuoyChange += Time.fixedDeltaTime;
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

    private void MoveLeft(InputAction.CallbackContext context)
    {
        this._moveDirection = Vector3.left;
    }

    private void MoveRight(InputAction.CallbackContext context)
    {
        this._moveDirection = Vector3.right;
    }

    private void StopDirection(InputAction.CallbackContext context)
    {
        if (this._playerControls.PlayerMap.Left.inProgress == false && this._playerControls.PlayerMap.Right.inProgress == false)
        {
            this._moveDirection = Vector3.zero;
        }
        else if (this._playerControls.PlayerMap.Left.inProgress == true)
        {
            this._moveDirection = Vector3.left;
        }
        else if (this._playerControls.PlayerMap.Right.inProgress == true)
        {
            this._moveDirection = Vector3.right;
        }
    }

    private void GoHeavy(InputAction.CallbackContext context)
    {
        if (this._timeSinceLastBuoyChange >= this._buoyChangeDelay)
        {
            this._setHeavy = true;
        }
        else if (this._delayCoroutine == null)
        {
            this._delayCoroutine = StartCoroutine(this.DelayHeavyCoroutine(context));
        }
    }

    private void GoLight(InputAction.CallbackContext context)
    {
        if (this._timeSinceLastBuoyChange >= this._buoyChangeDelay)
        {
            this._setHeavy = false;
        }
        else if (this._delayCoroutine == null)
        {
            this._delayCoroutine = StartCoroutine(this.DelayLightCoroutine(context));
        }
    }

    private void RestartLevel(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator DelayHeavyCoroutine(InputAction.CallbackContext context)
    {
        float timeDiff = this._buoyChangeDelay - this._timeSinceLastBuoyChange;

        while (timeDiff > 0.0f && this._playerControls.PlayerMap.Down.inProgress == true)
        {
            timeDiff -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (this._playerControls.PlayerMap.Down.inProgress == true)
        {
            this._setHeavy = true;
        }

        this._delayCoroutine = null;
    }

    private IEnumerator DelayLightCoroutine(InputAction.CallbackContext context)
    {
        float timeDiff = this._buoyChangeDelay - this._timeSinceLastBuoyChange;

        while (timeDiff > 0.0f && this._playerControls.PlayerMap.Down.inProgress == false)
        {
            timeDiff -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (this._playerControls.PlayerMap.Down.inProgress == false)
        {
            this._setHeavy = false;
        }

        this._delayCoroutine = null;
    }

    private void ExecuteHeavy()
    {
        this._buoyantObject.ChangeToHeavyObject();
        this._timeSinceLastBuoyChange = 0.0f;
    }

    private void ExecuteLight()
    {
        this._buoyantObject.ChangeToLightObject();
        this._timeSinceLastBuoyChange = 0.0f;
    }
}
