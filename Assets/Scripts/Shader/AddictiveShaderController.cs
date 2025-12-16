using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlasticGui;
using TMPro;
using UnityEngine.UI;

public class AddictiveShaderController : MonoBehaviour
{
    [SerializeField] private Material AddictiveShader;
    [SerializeField] private RawImage objectRenderer;
    
    //[SerializeField] private float defaultIntensity = 3.0f;
    //[SerializeField] private TextMeshProUGUI intensityValue;

    private Color color;
    private float intensity;
    void Start()
    {
        AddictiveShader = objectRenderer.GetComponent<RawImage>().material;
    }

    // public void TurnOffAddictiveShader()
    // {
    //     AddictiveShader.DisableKeyword("_ShaderIntensity");
    // }
    //
    // public void TurnOnAddictiveShader()
    // {
    //     AddictiveShader.EnableKeyword("_ShaderIntensity");
    // }

    public void AddictionInstensity(float value)
    {
        AddictiveShader.SetColor("_ShaderIntensity", color * value);
    }

    private void OnDestroy()
    {
        Destroy(AddictiveShader);
    }
    void Update()
    {
        
    }
}
