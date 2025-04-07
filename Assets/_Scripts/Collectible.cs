using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField]
    private GameObject _collectParticle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Play Audio
            ScoreManager.IncrementCollectiblesGrabbed();

            GameObject collectParticleInstance = GameObject.Instantiate(this._collectParticle, this.gameObject.transform.position, new Quaternion());
            collectParticleInstance.GetComponent<ParticleSystem>().Play();

            Destroy(this.gameObject);
        }
    }
}
