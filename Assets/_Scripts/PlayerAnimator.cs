using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{    
    private Animator _animator;

    private PlayerController _playerController;

    private Vector3 _lastMoveVector = Vector3.zero;

    private void Awake()
    {
        this._animator = GetComponent<Animator>();
        this._playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        this._playerController._playerControls.PlayerMap.Down.performed += this.SetRetractBuoyAnimation;
        this._playerController._playerControls.PlayerMap.Down.canceled += this.SetDeployBuoyAnimation;
    }

    private void Update()
    {
        if (this._playerController._buoyantObject.IsSubmerged() == false)
        {
            this.ClearSwimBools();
            this.ClearFloatBools();

            this.SetFallAnimation();
        }
        else if (this._playerController._buoyantObject.IsFullySubmerged() == true)
        {
            this.ClearFallBools();
            this.ClearFloatBools();

            if (this._playerController._buoyantObject.isHeavy == true)
            {
                this.SetFallAnimation();
            }
            else
            {
                this.SetSwimAnimation();
                
            }
        }
        else if (this._playerController._buoyantObject.IsSubmerged() == true)
        {
            this.ClearFallBools();
            this.ClearSwimBools();
            this.SetFloatAnimation();
        }
    }

    private void LateUpdate()
    {
        if (this._playerController._moveDirection != Vector3.zero)
        {
            this._lastMoveVector = this._playerController._moveDirection;
        }
    }

    private void SetFloatAnimation()
    {
        if (this._playerController._moveDirection == Vector3.left)
        {
            this._animator.SetBool("FloatLeft", true);
        }
        else if (this._playerController._moveDirection == Vector3.right)
        {
            this._animator.SetBool("FloatRight", true);
        }
        else if (this._playerController._buoyantObject.buoyantRigidbody.velocity.x < 0.0f)
        {
            this._animator.SetBool("FloatLeft", true);
        }
        else
        {
            this._animator.SetBool("FloatRight", true);
        }
    }

    private void ClearFloatBools()
    {
        this.ClearBool("FloatLeft");
        this.ClearBool("FloatRight");
    }

    private void SetSwimAnimation()
    {
        if (this._playerController._moveDirection == Vector3.left || this._lastMoveVector == Vector3.left)
        {
            this._animator.SetBool("SwimLeft", true);
        }
        else if (this._playerController._moveDirection == Vector3.right || this._lastMoveVector == Vector3.right)
        {
            this._animator.SetBool("SwimRight", true);
        }
    }

    private void ClearSwimBools()
    {
        this.ClearBool("SwimLeft");
        this.ClearBool("SwimRight");
    }

    private void SetFallAnimation()
    {
        if (this._playerController._moveDirection == Vector3.left || this._lastMoveVector == Vector3.left)
        {
            if (this._playerController._buoyantObject.isHeavy == false)
            {
                this._animator.SetBool("FallLeft_Light", true);
            }
            else
            {
                this._animator.SetBool("FallLeft_Heavy", true);
            }
        }
        else if (this._playerController._moveDirection == Vector3.right || this._lastMoveVector == Vector3.right)
        {
            if (this._playerController._buoyantObject.isHeavy == false)
            {
                this._animator.SetBool("FallRight_Light", true);
            }
            else
            {
                this._animator.SetBool("FallRight_Heavy", true);
            }
        }
    }

    private void ClearFallBools()
    {
        this.ClearBool("FallLeft_Light");
        this.ClearBool("FallLeft_Heavy");
        this.ClearBool("FallRight_Light");
        this.ClearBool("FallRight_Heavy");
    }

    private void SetDeployBuoyAnimation(InputAction.CallbackContext context)
    {
        if (this._playerController._moveDirection == Vector3.left || this._lastMoveVector == Vector3.left)
        {
            this._animator.SetBool("DeployBuoyLeft", true);
        }
        else if (this._playerController._moveDirection == Vector3.right || this._lastMoveVector == Vector3.right)
        {
            this._animator.SetBool("DeployBuoyRight", true);
        }
    }

    private void SetRetractBuoyAnimation(InputAction.CallbackContext context)
    {
        if (this._playerController._moveDirection == Vector3.left || this._lastMoveVector == Vector3.left)
        {
            this._animator.SetBool("RetractBuoyLeft", true);
        }
        else if (this._playerController._moveDirection == Vector3.right || this._lastMoveVector == Vector3.right)
        {
            this._animator.SetBool("RetractBuoyRight", true);
        }
    }

    public void ClearBool(string boolName)
    {
        this._animator.SetBool(boolName, false);
    }
}
