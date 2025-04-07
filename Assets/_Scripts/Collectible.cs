using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField]
    private GameObject _collectParticle;

    [SerializeField]
    private AudioClip _audioClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            AudioChannelSettings channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 0.5f, "SFX");
            AudioManager.instance.Play(this._audioClip, channelSettings);


            ScoreManager.IncrementCollectiblesGrabbed();

            GameObject collectParticleInstance = GameObject.Instantiate(this._collectParticle, this.gameObject.transform.position, new Quaternion());
            collectParticleInstance.GetComponent<ParticleSystem>().Play();

            Destroy(this.gameObject);
        }
    }
}
