using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private BuoyantObject _buoyantObject;

    [SerializeField]
    private MeshRenderer _renderer;

    [SerializeField]
    private Material _lightMaterial;
    [SerializeField]
    private Material _heavyMaterial;

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            this._renderer.material = this._heavyMaterial;
            this._buoyantObject.ChangeToHeavyObject();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {            
            this._renderer.material = this._lightMaterial;            
            this._buoyantObject.ChangeToLightObject();
        }
    }
}
