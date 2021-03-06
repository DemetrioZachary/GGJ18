﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetAnimator : MonoBehaviour
{
    [SerializeField]
    private float scale = 0.2f;

    public float speed = 1.0f;

    public float Speed
    {
        get { return speed; }
        set
        {
            speed = value;
            for (int i = 0; i < transform.childCount; ++i)
                transform.GetChild(i).GetComponent<OffsetAnimator>().Speed = value;
        }
    }

    private Vector2 offset = Vector2.zero;

    private MeshRenderer meshRnd = null;

    private void Awake()
    {
        meshRnd = GetComponentInParent<MeshRenderer>();
    }
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {
        offset.x += Time.deltaTime * scale;
        meshRnd.material.mainTextureOffset = offset;
	}
}
