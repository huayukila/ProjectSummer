using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PaintOnObject
{
    public class Paintable : MonoBehaviour
    {
        public bool debugUV = true;
        public int textureSize = 1024;

        private Renderer rend;
        private RenderTexture maskTexture;
        private RenderTexture copyTexture;

        private int maskTextureID = Shader.PropertyToID("_MaskTex");

        public Renderer GetRenderer() => rend;
        public RenderTexture GetMask() => maskTexture;
        public RenderTexture GetCopy() => copyTexture;

        void Start()
        {
            maskTexture = new RenderTexture(textureSize, textureSize, 0);
            maskTexture.filterMode = FilterMode.Bilinear;

            copyTexture = new RenderTexture(textureSize, textureSize, 0);
            copyTexture.filterMode = FilterMode.Bilinear;
            PaintManager.Instance.ClearRT(copyTexture);

            rend = GetComponent<Renderer>();
            rend.material.SetTexture(maskTextureID, copyTexture);

            GameManager.Instance.mapMaskTexture = copyTexture;

            if (debugUV)
            {
                PaintManager.Instance.InitUVMask(this);
            }
        }

        void OnDisable()
        {
            maskTexture.Release();
            copyTexture.Release();
        }
    }
}