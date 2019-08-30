using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    [Range(0.01f, 2f)]
    public float Mass = 1f;

    private Vector3 _Force;
    private Vector3 _Speed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void LetUpdate()
    {
        _Speed += _Force;
        this.transform.position += _Speed;
    }

    public void SetStartSpeed(Vector3 speed)
    {
        _Speed = speed;
    }

    public void AddForce(Vector3 add)
    {
        _Force += add;
    }

    public void ClearForce()
    {
        _Force = Vector3.zero;
    }
}
