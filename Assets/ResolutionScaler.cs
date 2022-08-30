#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ResolutionScaler : MonoBehaviour
{
    [Range(2, 16)] public float Scale = 2;

    private Camera cameraComponent;
    private RenderTexture texture;

    private void Start()
    {
        CreateTexture();
    }

    private void CreateTexture()
    {
        int width = Mathf.RoundToInt(Screen.width / Scale);
        int height = Mathf.RoundToInt(Screen.height / Scale);
       // GetComponent<Camera>().orthographicSize = Mathf.RoundToInt(Screen.width / Scale);
           texture = new RenderTexture(width, height, 24, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Default);
        // texture.antiAliasing = ;

        cameraComponent = GetComponent<Camera>();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (EditorApplication.isPlaying) return;
        CreateTexture();
    }
#endif

    private void OnPreRender()
    {
        cameraComponent.targetTexture = texture;
    }

    private void OnPostRender()
    {
        cameraComponent.targetTexture = null;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
     //   src.filterMode = FilterMode.Bilinear;

        Graphics.Blit(src, dest);
    }
}