using UnityEngine;

public class PixelateRunner : MonoBehaviour
{
    public ComputeShader PixelateComputeShader;
    [Range(2, 40)] public int BlockSize = 3;

    int _screenWidth;
    int _screenHeight;
    RenderTexture _renderTexture;

    void Start()
    {
        CreateRenderTexture();
    }

    void CreateRenderTexture()
    {
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;
        
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

        var mainKernel = PixelateComputeShader.FindKernel("Pixelate");
        PixelateComputeShader.SetInt("_BlockSize", BlockSize);
        PixelateComputeShader.SetInt("_ResultWidth", _renderTexture.width);
        PixelateComputeShader.SetInt("_ResultHeight", _renderTexture.height);
        PixelateComputeShader.SetTexture(mainKernel, "Result", _renderTexture);
        PixelateComputeShader.Dispatch(mainKernel,
            Mathf.CeilToInt(_renderTexture.width / (float)BlockSize / 8f),
            Mathf.CeilToInt(_renderTexture.height / (float)BlockSize / 8f),
            1);

        Graphics.Blit(_renderTexture, dest);
    }
}