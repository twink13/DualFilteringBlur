using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupGravity : MonoBehaviour
{
    [Range(0.0001f, 0.01f)]
    public float Scale = 0.01f;
    public Gravity[] Gravitys;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 speedSum = Vector3.zero;
        for (int i = 0; i < Gravitys.Length; i++)
        {
            Gravity gravity = Gravitys[i];

            if (i != Gravitys.Length - 1)
            {
                // 不是最后一个
                Vector3 speed = Random.onUnitSphere * Scale;
                gravity.SetStartSpeed(speed);

                speedSum += speed;
            }
            else
            {
                // 最后一个 平衡业界
                gravity.SetStartSpeed(-speedSum);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        // 清空
        for (int i = 0; i < Gravitys.Length; i++)
        {
            Gravity gravity = Gravitys[i];

            gravity.ClearForce();
        }

        // 每两个物体之间加引力
        for (int i = 0; i < Gravitys.Length; i++)
        {
            Gravity gravity = Gravitys[i];

            for (int j = i + 1; j < Gravitys.Length; j++)
            {
                Gravity gravity2 = Gravitys[j];

                Vector3 dir = gravity2.transform.position - gravity.transform.position;
                Vector3 force = gravity.Mass * gravity2.Mass * dir * Mathf.Pow(Vector3.SqrMagnitude(dir), 2 / 3) * Scale;

                gravity.AddForce(force);
                gravity2.AddForce(-force);
            }
        }

        // 触发update
        for (int i = 0; i < Gravitys.Length; i++)
        {
            Gravity gravity = Gravitys[i];

            gravity.LetUpdate();
        }
    }
}
