using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GeradorInimigos : MonoBehaviour
{
    [Header("Configurações do Inimigo Comum")]
    public GameObject inimigoPrefab;
    public float intervalo = 3f;
    public float limiteX = 8f;
    public float limiteY = 4f;
    public float velocidade = 3f;
    public float limiteDestruicaoX = -12f;

    [Header("Configurações da Boss Fight")]
    public GameObject bossPrefab;
    public int metaEliminacoes = 10;
    public float posicaoFinalBossX = 5f;
    public float velocidadeBoss = 2f;

    [Header("Interface & UI")]
    public TextMeshProUGUI textoQuotaInimigos;
    public GameObject objetoTextoVidaBoss; 

    private int contadorEliminacoes;
    private bool bossSpawnado;
    private static bool jogoVencido;

    void Start()
    {
        jogoVencido = false;
        if (objetoTextoVidaBoss != null) objetoTextoVidaBoss.SetActive(false);
        
        AtualizarTextoQuota();
        InvokeRepeating(nameof(GerarInimigo), 0f, intervalo);
    }

    void Update()
    {
        if (jogoVencido && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public static void IndicarVitoria()
    {
        jogoVencido = true;
    }

    void GerarInimigo()
    {
        if (bossSpawnado) return;

        Vector2 posicao = new Vector2(limiteX, Random.Range(-limiteY, limiteY));
        GameObject inimigo = Instantiate(inimigoPrefab, posicao, Quaternion.identity);
        StartCoroutine(MoverInimigo(inimigo));
    }

    IEnumerator MoverInimigo(GameObject inimigo)
    {
        while (inimigo != null)
        {
            inimigo.transform.Translate(Vector2.left * velocidade * Time.deltaTime);

            if (inimigo.transform.position.x < limiteDestruicaoX)
            {
                Destroy(inimigo);
                yield break; 
            }
            yield return null; 
        }
    }

    public void RegistrarEliminacao()
    {
        if (bossSpawnado) return;

        contadorEliminacoes++;
        AtualizarTextoQuota();

        if (contadorEliminacoes >= metaEliminacoes)
        {
            IniciarBossFight();
        }
    }

    void AtualizarTextoQuota()
    {
        if (textoQuotaInimigos != null)
        {
            int restantes = Mathf.Max(0, metaEliminacoes - contadorEliminacoes);
            textoQuotaInimigos.text = "ENEMY QUOTA: " + restantes;
        }
    }

    void IniciarBossFight()
    {
        bossSpawnado = true;
        CancelInvoke(nameof(GerarInimigo));

        if (textoQuotaInimigos != null) textoQuotaInimigos.gameObject.SetActive(false);
        if (objetoTextoVidaBoss != null) objetoTextoVidaBoss.SetActive(true);

        if (bossPrefab != null)
        {
            Vector2 posicaoInicial = new Vector2(limiteX + 4f, 0f);
            GameObject boss = Instantiate(bossPrefab, posicaoInicial, Quaternion.identity);

            ControlaBoss scriptBoss = boss.GetComponent<ControlaBoss>();
            if (scriptBoss != null && objetoTextoVidaBoss != null)
            {
                scriptBoss.textoVidaBoss = objetoTextoVidaBoss.GetComponent<TextMeshProUGUI>();
            }

            StartCoroutine(DeslizarBoss(boss));
        }
    }

    IEnumerator DeslizarBoss(GameObject boss)
    {
        while (boss != null && boss.transform.position.x > posicaoFinalBossX)
        {
            boss.transform.Translate(Vector2.left * velocidadeBoss * Time.deltaTime);
            yield return null;
        }
    }
}