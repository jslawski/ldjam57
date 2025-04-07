using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;

    private AudioSource[] _bgms;

    private float _timeToFade = 2.0f;

    public int currentlyPlayingIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this._bgms = GetComponents<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < this._bgms.Length; i++)
        {
            this._bgms[i].volume = 0.0f;
            this._bgms[i].Play();
        }

        this._bgms[0].volume = 1.0f;
    }

    public void PlayBGMAtIndex(int index)
    {
        this._bgms[this.currentlyPlayingIndex].DOKill();
        this._bgms[index].DOKill();

        this._bgms[this.currentlyPlayingIndex].DOFade(0.0f, this._timeToFade);
        this._bgms[index].DOFade(1.0f, this._timeToFade);

        this.currentlyPlayingIndex = index;
    }
}
