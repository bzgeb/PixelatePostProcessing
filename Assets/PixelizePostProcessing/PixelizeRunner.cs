using System;
using UnityEngine;

public class PixelizeRunner : MonoBehaviour
{
    public ComputeShader PixelizeComputeShader;

    int _screenWidth;
    int _screenHeight;
    RenderTexture _renderTexture;
    
    void Start()
    {
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;
        
        CreateRenderTexture();
    }

    void CreateRenderTexture()
    {
         _renderTexture = new RenderTexture(_screenWidth, _screenHeight, 24);
         _renderTexture.filterMode = FilterMode.Point;
         _renderTexture.enableRandomWrite = true;
         _renderTexture.Create();       
    }

    void Update()
    {
        if (Screen.width != _screenWidth || Screen.height != _screenHeight)
            CreateRenderTexture();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, _renderTexture);
        
        var mainKernel = PixelizeComputeShader.FindKernel("CSMain");
        PixelizeComputeShader.SetFloat("_ResultWidth", _renderTexture.width);
        PixelizeComputeShader.SetFloat("_ResultHeight", _renderTexture.height);
        PixelizeComputeShader.SetTexture(mainKernel, "Result", _renderTexture);
        PixelizeComputeShader.Dispatch(mainKernel, _renderTexture.width / 3, _renderTexture.height / 3, 1);
        
        Graphics.Blit(_renderTexture, dest);
    }
}