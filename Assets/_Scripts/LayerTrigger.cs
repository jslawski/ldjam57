using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerTrigger : MonoBehaviour
{
    private Animator _chapterAnimator;

    [SerializeField]
    private string _animationTriggerName;
    
    [SerializeField]
    private int _bgmIndex = 0;

    private void Awake()
    {
        this._chapterAnimator = GameObject.Find("ChapterCanvas").GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && BGMManager.instance.currentlyPlayingIndex != _bgmIndex)
        {
            this._chapterAnimator.SetTrigger(this._animationTriggerName);            
            BGMManager.instance.PlayBGMAtIndex(this._bgmIndex);
        }
    }
}
