using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{
    public Transform reference;
    public Vector2 referenceBound;
    public Vector2 parallexBound;

    private void LateUpdate()
    {
        //var x = Mathf.Lerp(-parallexBound.x, parallexBound.x, (reference.transform.position.x / referenceBound.x));
        //var y = Mathf.Lerp(-parallexBound.y, parallexBound.y, (reference.transform.position.y / referenceBound.y));

        var x = parallexBound.x * (reference.transform.position.x / referenceBound.x);
        var y = parallexBound.y * (reference.transform.position.y / referenceBound.y);
        transform.position = new Vector3(x, y, 0f);
    }
}
