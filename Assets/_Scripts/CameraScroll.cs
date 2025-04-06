using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _target;

    private Transform _cameraHolderTransform;

    private float _verticalViewportThreshold = 0.3f;
    private float _horizontalViewportThreshold = 0.0f;

    private float _initialZPosition;

    // Start is called before the first frame update
    void Awake()
    {
        this._cameraHolderTransform = GetComponent<Transform>();

        this._initialZPosition = this._cameraHolderTransform.position.z;
    }

    private bool IsPlayerPastHorizontalThreshold(float playerViewportXPosition)
    {
        return (playerViewportXPosition >= (1.0f - this._horizontalViewportThreshold)) ||
            (playerViewportXPosition <= this._horizontalViewportThreshold);
    }

    private bool IsPlayerPastVerticalThreshold(float playerViewportYPosition)
    {
        return (playerViewportYPosition >= (1.0f - this._verticalViewportThreshold)) ||
            (playerViewportYPosition <= this._verticalViewportThreshold);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this._target == null)
        {
            return;
        }

        Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint(this._target.position);

        if (this.IsPlayerPastHorizontalThreshold(targetViewportPosition.x))
        {
            //this.UpdateCameraHorizontalPosition();
        }

        if (this.IsPlayerPastVerticalThreshold(targetViewportPosition.y))
        {
            this.UpdateCameraVerticalPosition();
        }
    }

    private void UpdateCameraHorizontalPosition()
    {
        Vector3 worldSpaceCenteredPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, this._verticalViewportThreshold, this._initialZPosition));

        Vector3 shiftVector = new Vector3(this._target.position.x - worldSpaceCenteredPosition.x, 0.0f, 0.0f);

        this._cameraHolderTransform.Translate(shiftVector.normalized * Mathf.Abs(this._target.velocity.x) * Time.fixedDeltaTime);
    }
    private  void UpdateCameraVerticalPosition() 
    {
        Vector3 worldSpaceCenteredPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, this._verticalViewportThreshold, this._initialZPosition));

        Vector3 shiftVector = new Vector3(0.0f, this._target.position.y - worldSpaceCenteredPosition.y, 0.0f);

        this._cameraHolderTransform.Translate(shiftVector.normalized * Mathf.Abs(this._target.velocity.y) * Time.fixedDeltaTime);
    }
}
