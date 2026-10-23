using UnityEngine;

public class MoveFundo : MonoBehaviour 
{
    private Material fundoMaterial;

    void Start()
    {
        // Pega o material diretamente do Renderer do Quad
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            fundoMaterial = rend.material;
        }
    }

    void Update()
    {
        if (fundoMaterial != null)
        {
            float offset = Time.time * 0.5f;
            
            // Move a textura no eixo X continuamente
            fundoMaterial.mainTextureOffset = new Vector2(offset, 0);
        }
    }
}