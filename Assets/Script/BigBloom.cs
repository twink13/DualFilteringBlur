using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class BigBloom : MonoBehaviour
{
    public int DownSampleTimes = 3;
    public Material DownSampleMaterial;

    public RenderTexture DebugRT;

    private RenderTexture[] _RTs;

    // Start is called before the first frame update
    void Start()
    {
        _RTs = new RenderTexture[DownSampleTimes];

        for (int i = 0; i < _RTs.Length; i++)
        {
            RenderTexture rt = new RenderTexture(Screen.width >> (i + 1), Screen.height >> (i + 1), 0, DefaultFormat.HDR);
            rt.filterMode = FilterMode.Bilinear;
            rt.name = "BigBloomDownSample" + (i + 1);
            _RTs[i] = rt;
        }
        DebugRT = _RTs[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture before = source;
        RenderTexture after = source;
        for (int i = 0; i < _RTs.Length; i++)
        {
            RenderTexture rt = _RTs[i];
            after = rt;

            //Graphics.SetRenderTarget(rt);
            if (i == 0)
            {
                Graphics.Blit(before, after, DownSampleMaterial, 0);
            }
            else
            {
                Graphics.Blit(before, after, DownSampleMaterial, 1);
            }

            before = after;
        }


        for (int i = _RTs.Length - 2; i >= 0; i--)
        {
            RenderTexture rt = _RTs[i];
            after = rt;

            //Graphics.SetRenderTarget(rt);
            Graphics.Blit(before, after, DownSampleMaterial, 2);

            before = after;
        }

        Graphics.Blit(source, destination);
        Graphics.Blit(after, destination, DownSampleMaterial, 3);
    }
}
