using UnityEngine;

public class TiroBoss : MonoBehaviour
{
    public float velocidade = 10f;
    private Vector2 direcao;

    public void DefinirDirecao(Vector2 novaDirecao)
    {
        direcao = novaDirecao.normalized;
    }

    void Update()
    {
        transform.Translate(direcao * velocidade * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D outroColisor)
    {
        if (outroColisor.CompareTag("Player") || outroColisor.gameObject.name.Contains("Jogador"))
        {
            ControlaJogadorMouseEsquerdo jogador = outroColisor.GetComponent<ControlaJogadorMouseEsquerdo>();
            
            if (jogador != null)
            {
                jogador.ReceberDanoDoBoss();
            }

            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}