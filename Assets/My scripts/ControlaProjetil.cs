using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlaProjetil : MonoBehaviour
{
    public float velocidade = 15f;

    void Update()
    {
        // Move o projétil para a frente
        transform.Translate(Vector3.right * velocidade * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D outroColisor)
    {
        // 1. TENTA ACERTAR O BOSS PRIMEIRO
        ControlaBoss boss = outroColisor.GetComponent<ControlaBoss>();
        if (boss != null)
        {
            // Causa 1 de dano no Boss (ele mesmo vai contar até 10 e cair)
            boss.TomarDano();

            // Destrói apenas o projétil ao acertar o boss
            Destroy(gameObject);
            return; // Encerra a execução aqui para não cair na lógica de inimigo comum
        }

        // 2. SE NÃO FOR O BOSS, MANTÉM A LÓGICA ANTIGA PARA INIMIGOS NORMAIS
        if (outroColisor.CompareTag("Inimigo"))
        {
            // Encontra o Gerador de Inimigos na cena e avisa que um inimigo foi eliminado
            GeradorInimigos gerador = FindObjectOfType<GeradorInimigos>();
            if (gerador != null)
            {
                gerador.RegistrarEliminacao(); 
            }

            // Pega o Rigidbody2D do inimigo para fazê-lo cair
            Rigidbody2D rbInimigo = outroColisor.GetComponent<Rigidbody2D>();
            
            if (rbInimigo != null)
            {
                rbInimigo.isKinematic = false; // Garante que a física age nele
                rbInimigo.gravityScale = 1f;   // Ativa a gravidade para despencar
                
                // Dá um pequeno empurrãozinho aleatório na queda
                rbInimigo.velocity = new Vector2(Random.Range(-2f, 2f), -3f);  
            }

            // Destrói o projétil (a bala) ao acertar
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        // Destrói o projétil se sair da tela para não pesar o jogo
        Destroy(gameObject);
    }
}