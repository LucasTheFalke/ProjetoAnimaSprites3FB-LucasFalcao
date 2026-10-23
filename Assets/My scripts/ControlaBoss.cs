using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlaBoss : MonoBehaviour
{
    [Header("Configurações de Disparo")]
    public GameObject tiroBossPrefab;
    public Transform pontoDisparoBoss;
    public float intervaloTiro = 2f;

    [Header("Configurações de Vida")]
    public int vidaMaxima = 10;
    private int vidaAtual;
    private bool isDerrotado;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color corOriginal;

    [Header("Efeito Visual")]
    public Color corDano = Color.red;

    [Header("Interface UI")]
    public TextMeshProUGUI textoVidaBoss;

    void Start()
    {
        vidaAtual = vidaMaxima;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (sr != null) corOriginal = sr.color;
        if (rb != null) { rb.simulated = true; rb.gravityScale = 0f; }

        AtualizarTextoVida();
        if (textoVidaBoss != null) textoVidaBoss.gameObject.SetActive(true);

        StartCoroutine(RotinaDeDisparos());
    }

    IEnumerator RotinaDeDisparos()
    {
        yield return new WaitForSeconds(1f);

        while (!isDerrotado)
        {
            int padrao = Random.Range(0, 3);
            if (padrao == 0) DispararEmLeque();
            else if (padrao == 1) DispararAleatorio();
            else DispararRajadaFocalizada();

            yield return new WaitForSeconds(intervaloTiro);
        }
    }

    public void TomarDano()
    {
        if (isDerrotado) return;

        vidaAtual--;
        AtualizarTextoVida();

        if (sr != null) StartCoroutine(EfeitoPiscar());
        if (vidaAtual <= 0) DerrotarBoss();
    }

    IEnumerator EfeitoPiscar()
    {
        float tempo = 0f;
        while (tempo < 0.4f)
        {
            sr.color = corDano;
            yield return new WaitForSeconds(0.08f);
            sr.color = corOriginal;
            yield return new WaitForSeconds(0.08f);
            tempo += 0.16f;
        }
        sr.color = corOriginal;
    }

    void AtualizarTextoVida()
    {
        if (textoVidaBoss != null)
        {
            textoVidaBoss.text = "BOSS HEALTH " + Mathf.Max(0, vidaAtual);
        }
    }

    void DerrotarBoss()
    {
        isDerrotado = true;
        StopAllCoroutines();

        if (sr != null) sr.color = corOriginal;
        if (textoVidaBoss != null) textoVidaBoss.gameObject.SetActive(false);

        if (rb != null)
        {
            rb.gravityScale = 2f;
            rb.velocity = new Vector2(0f, -2f);
        }

        GameObject jogador = GameObject.FindGameObjectWithTag("Player");
        if (jogador != null)
        {
            ControlaJogadorMouseEsquerdo scriptJogador = jogador.GetComponent<ControlaJogadorMouseEsquerdo>();
            if (scriptJogador != null) scriptJogador.IniciarVitoria();
        }

        Destroy(gameObject, 4f);
    }

    Vector3 ObterPosicaoSpawn() => pontoDisparoBoss != null ? pontoDisparoBoss.position : transform.position;

    void DispararEmLeque()
    {
        if (isDerrotado) return;
        float[] angulos = { -30f, -15f, 0f, 15f, 30f };
        foreach (float angulo in angulos)
        {
            GameObject tiro = Instantiate(tiroBossPrefab, ObterPosicaoSpawn(), Quaternion.identity);
            TiroBoss scriptTiro = tiro.GetComponent<TiroBoss>();
            if (scriptTiro != null) scriptTiro.DefinirDirecao(Quaternion.Euler(0, 0, angulo) * Vector2.left);
        }
    }

    void DispararAleatorio()
    {
        if (isDerrotado) return;
        for (int i = 0; i < 4; i++)
        {
            GameObject tiro = Instantiate(tiroBossPrefab, ObterPosicaoSpawn(), Quaternion.identity);
            TiroBoss scriptTiro = tiro.GetComponent<TiroBoss>();
            if (scriptTiro != null) scriptTiro.DefinirDirecao(Quaternion.Euler(0, 0, Random.Range(-45f, 45f)) * Vector2.left);
        }
    }

    void DispararRajadaFocalizada()
    {
        if (isDerrotado) return;
        GameObject jogador = GameObject.FindGameObjectWithTag("Player");
        if (jogador != null)
        {
            GameObject tiro = Instantiate(tiroBossPrefab, ObterPosicaoSpawn(), Quaternion.identity);
            TiroBoss scriptTiro = tiro.GetComponent<TiroBoss>();
            if (scriptTiro != null) scriptTiro.DefinirDirecao((jogador.transform.position - ObterPosicaoSpawn()).normalized);
        }
    }
}