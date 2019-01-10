﻿using UnityEngine;
using System;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class BloomEffect : MonoBehaviour {

    [Range(1, 16)]
    public int iterations = 1;
    public Shader bloomShader;

    RenderTexture[] renderTextures = new RenderTexture[16];
    Material bloom;

	void OnRenderImage (RenderTexture source, RenderTexture destination) {

        if(bloom ==  null)
        {
            bloom = new Material(bloomShader);
            bloom.hideFlags = HideFlags.HideAndDontSave;
        }

        int width = source.width ;
        int height = source.height;
        RenderTextureFormat format = source.format;

        RenderTexture destinationRT = renderTextures[0] = RenderTexture.GetTemporary(width, height, 0, format);
		Graphics.Blit(source, destinationRT);

        RenderTexture currentRT = destinationRT;

        int i = 1;
        for(; i< iterations; i++)
        {
            width = width / 2;
            height = height / 2;

            if(height < 2 || width < 2)
            {
                break;
            }

            destinationRT = renderTextures[i] = RenderTexture.GetTemporary(width, height, 0, format);
            Graphics.Blit(currentRT, destinationRT, bloom);
            //RenderTexture.ReleaseTemporary(currentRT);
            currentRT = destinationRT;
        }

        for(i -= 2; i >= 0; i--)
        {
            destinationRT = renderTextures[i];
            renderTextures[i] = null;
            Graphics.Blit(currentRT, destinationRT, bloom);
            RenderTexture.ReleaseTemporary(currentRT);
            currentRT = destinationRT;
        }

        Graphics.Blit(destinationRT, destination, bloom);

        RenderTexture.ReleaseTemporary(destinationRT);
    }
}