using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whacka.Render
{
    public struct BlurSettings
    {
        public Material UseMaterial;
        public Shader UseShader;
        public string UseShaderByName;

    }

    public static class DualFilteringBlurUtils
    {
        public static void Blur(Texture source, RenderTexture destination, int downSampleTimes, BlurSettings settings = new BlurSettings())
        {
            // gen material
            Material mat = settings.UseMaterial;

            // blur
            Texture before;
            RenderTexture after;

            before = source;
            for (int i = 1; i <= downSampleTimes; i++)
            {
                after = RenderTexture.GetTemporary(source.width << i, source.height << i, 0);
            }
        }
    }
}
