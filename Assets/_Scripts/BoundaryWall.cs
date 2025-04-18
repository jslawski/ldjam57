using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryWall : MonoBehaviour
{
    [SerializeField]
    private float _pushDirection = 1.0f;

    [SerializeField]
    private float _horizontalAcceleration = 5.0f;
    [SerializeField]
    private float _verticalAcceleration = 1.0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Rigidbody>().AddForce(new Vector3(this._pushDirection * this._horizontalAcceleration, this._verticalAcceleration, 0.0f), ForceMode.Acceleration);
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 spawnPoint = GetComponent<Collider>().ClosestPoint(other.gameObject.transform.position);
            other.GetComponent<BuoyantObject>().SpawnEnterSplashParticle(spawnPoint);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 spawnPoint = GetComponent<Collider>().ClosestPoint(other.gameObject.transform.position);
            other.GetComponent<BuoyantObject>().SpawnExitSplashParticle(spawnPoint);
        }
    }
    */
}
