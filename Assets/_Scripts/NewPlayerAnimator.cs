using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerAnimator : MonoBehaviour
{
    private Animator _animator;

    private PlayerController _playerController;

    private Vector3 _lastMoveVector = Vector3.zero;

    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    AudioClip _audioClip;

    private void Awake()
    {
        this._animator = GetComponent<Animator>();
        this._playerController = GetComponent<PlayerController>();
        this._spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
        else if (this._playerController._buoyantObject.IsSubmerged() == true && this._playerController._buoyantObject.isHeavy == false)
        {
            this.ClearFallBools();
            this.ClearSwimBools();           
            this.SetFloatAnimation();             
        }

        if (this._animator.GetBool("PlayerDeployBuoy") == true || this._animator.GetBool("PlayerRetractBuoy") == true)
        {
            this.ClearFallBools();
            this.ClearSwimBools();
            this.ClearFloatBools();
        }

        if (this._playerController._moveDirection == Vector3.left)
        {
            this._spriteRenderer.flipX = false;
        }
        else if (this._playerController._moveDirection == Vector3.right)
        {
            this._spriteRenderer.flipX = true;
        }
        else if (this._lastMoveVector == Vector3.left)
        {
            this._spriteRenderer.flipX = false;
        }
        else if (this._lastMoveVector == Vector3.right)
        {
            this._spriteRenderer.flipX = true;
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
        this._animator.SetBool("PlayerFloat", true);
    }

    private void ClearFloatBools()
    {
        this.ClearBool("PlayerFloat");        
    }

    private void SetSwimAnimation()
    {
        this._animator.SetBool("PlayerSwim", true);       
    }

    private void ClearSwimBools()
    {
        this.ClearBool("PlayerSwim");        
    }

    private void SetFallAnimation()
    {
        if (this._playerController._buoyantObject.isHeavy == false)
        {
            this._animator.SetBool("PlayerFall_Light", true);
        }
        else
        {
            this._animator.SetBool("PlayerFall_Heavy", true);
        }
    }

    private void ClearFallBools()
    {
        this.ClearBool("PlayerFall_Light");
        this.ClearBool("PlayerFall_Heavy");
    }

    private void SetDeployBuoyAnimation(InputAction.CallbackContext context)
    {
        this._animator.SetBool("PlayerDeployBuoy", true);
        this._animator.SetBool("PlayerRetractBuoy", false);

        //AudioChannelSettings channelSettings = new AudioChannelSettings(false, 0.8f, 0.9f, 0.1f, "SFX");

        //AudioManager.instance.Play(this._audioClip, channelSettings);
    }

    private void SetRetractBuoyAnimation(InputAction.CallbackContext context)
    {
        this._animator.SetBool("PlayerRetractBuoy", true);
        this._animator.SetBool("PlayerDeployBuoy", false);
    }

    public void ClearBool(string boolName)
    {
        this._animator.SetBool(boolName, false);
    }
}
