using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private Transform _objectTransform;

    [SerializeField]
    [Range(-2.0f, 2.0f)]
    private float rotationSpeed = 1.0f;

    private void Awake()
    {
        this._objectTransform = this.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this._objectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, this._objectTransform.rotation.eulerAngles.z - rotationSpeed);
    }
}
