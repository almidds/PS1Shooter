using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessingCamera : MonoBehaviour{

    private Camera cam;

    [SerializeField]
    Material fogMat;

    [SerializeField]
    private Color fogColor;

    [SerializeField, Range(0.0f, 1.0f)]
    private float fogDensity;

    [SerializeField, Range(0.0f, 100.0f)]
    private float fogOffset;

    void Start() {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
    }

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        fogMat.SetVector("_FogColor", fogColor);
        fogMat.SetFloat("_FogDensity", fogDensity);
        fogMat.SetFloat("_FogOffset", fogOffset);
        Graphics.Blit(src, dest, fogMat);
    }

}
