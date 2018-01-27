using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetAnimator : MonoBehaviour
{
    [SerializeField]
    private float scale = 0.2f;

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
