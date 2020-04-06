using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class BigBloom : MonoBehaviour
{
    public int DownSampleTimes = 3;
    public Material DownSampleMaterial;

    public RenderTexture DebugRT;

    private RenderTexture _SourceCopy;
    private RenderTexture[] _DownSampleRTs;
    private RenderTexture[] _UpSampleRTs;

    // Start is called before the first frame update
    void Start()
    {
        _DownSampleRTs = new RenderTexture[DownSampleTimes];

        for (int i = 0; i < _DownSampleRTs.Length; i++)
        {
            RenderTexture rt = new RenderTexture(Screen.width >> (i + 1), Screen.height >> (i + 1), 0, DefaultFormat.HDR);
            rt.filterMode = FilterMode.Point;
            rt.name = "BigBloomDownSample" + (i + 1);
            _DownSampleRTs[i] = rt;
        }
        DebugRT = _DownSampleRTs[0];

        // up
        _UpSampleRTs = new RenderTexture[DownSampleTimes];

        for (int i = 0; i < _UpSampleRTs.Length; i++)
        {
            RenderTexture rt = new RenderTexture(Screen.width >> (i + 1), Screen.height >> (i + 1), 0, DefaultFormat.HDR);
            rt.filterMode = FilterMode.Bilinear;
            rt.name = "BigBloomUpSample" + (i + 1);
            _UpSampleRTs[i] = rt;
        }

        // source copy
        _SourceCopy = new RenderTexture(Screen.width, Screen.height, 0, DefaultFormat.HDR);
        _SourceCopy.filterMode = FilterMode.Point;
        _SourceCopy.name = "SourceCopy";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, _SourceCopy, DownSampleMaterial, 0);

        RenderTexture before = _SourceCopy;
        RenderTexture after = source;
        for (int i = 0; i < _DownSampleRTs.Length; i++)
        {
            RenderTexture rt = _DownSampleRTs[i];
            after = rt;

            //Graphics.SetRenderTarget(rt);
            Graphics.Blit(before, after, DownSampleMaterial, 1);

            before = after;
        }


        for (int i = _UpSampleRTs.Length - 2; i >= 0; i--)
        {
            RenderTexture rt = _UpSampleRTs[i];
            after = rt;

            //Graphics.SetRenderTarget(rt);
            Graphics.Blit(before, after, DownSampleMaterial, 2);

            before = after;
        }

        Graphics.Blit(source, destination);
        Graphics.Blit(after, destination, DownSampleMaterial, 3);
    }
}
