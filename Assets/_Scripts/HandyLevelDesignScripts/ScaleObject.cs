using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleObject : MonoBehaviour
{
    private Transform _objectTransform;

    [Header("Which scales should the object change between?")]
    [SerializeField]
    private float _scale1 = 0.5f;
    [SerializeField]
    private float _scale2 = 1.5f;

    [Header("How long should it take to get from Scale 1 to Scale 2?")]
    [SerializeField]
    private float _moveTime = 0.5f;

    [Header("How long should the object stay at a target scale before changing again?")]
    [SerializeField]
    private float _waitTime = 2.0f;

    private bool _changeToScale2 = true;

    private Vector3 _scaleVector1;
    private Vector3 _scaleVector2;

    private void Awake()
    {
        this._objectTransform = this.gameObject.GetComponent<Transform>();

        this._scaleVector1 = new Vector3(this._scale1, this._scale1, this._scale1);
        this._scaleVector2 = new Vector3(this._scale2, this._scale2, this._scale2);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        this._scaleVector1 = new Vector3(this._scale1, this._scale1, this._scale1);
        this._scaleVector2 = new Vector3(this._scale2, this._scale2, this._scale2);

        this._objectTransform.localScale = this._scaleVector1;

        StartCoroutine(this.MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(this._waitTime);

            if (this._changeToScale2 == true)
            {
                this._objectTransform.DOScale(this._scaleVector2, this._moveTime).SetEase(Ease.InOutBack);
            }
            else
            {
                this._objectTransform.DOScale(this._scaleVector1, this._moveTime).SetEase(Ease.InOutBack);
            }

            this._changeToScale2 = !this._changeToScale2;

            yield return new WaitForSeconds(this._moveTime);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
