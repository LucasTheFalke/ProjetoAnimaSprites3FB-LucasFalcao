using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ControlaJogadorMouseEsquerdo : MonoBehaviour {

    bool comecou, acabou, vitoria;
    Rigidbody2D rb;
    SpriteRenderer sr;
    Color corOriginal;
    bool piscando;

    [Header("Configurações")]
    public int maxToques = 3;
    private int contadorToques;
    public Color corDano = Color.red;
    public Vector2 forcaImpulso = new Vector2(0, 500f);

    [Header("Configurações de Timer")]
    public float tempoRestante = 30f;
    private bool timerAtivo;

    [Header("UI & Referências")]
    public TextMeshProUGUI textoVida;
    public TextMeshProUGUI textoTimer;
    public GameObject canvasGameOver, canvasContinua;
    public GameObject projetilPrefab;
    public Transform pontoDisparo;

    void Start () {  
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (sr != null) corOriginal = sr.color;
        if (canvasGameOver != null) canvasGameOver.SetActive(false);
        if (canvasContinua != null) canvasContinua.SetActive(false);

        AtualizarTextoVida();
        AtualizarTextoTimer();
    }
      
    void Update () {
        if (acabou || vitoria) {
            if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        
        if (timerAtivo && tempoRestante > 0) {
            tempoRestante -= Time.deltaTime;
            AtualizarTextoTimer();

            if (tempoRestante <= 0) {
                tempoRestante = 0;
                AtualizarTextoTimer();
                FinalizarJogo(); 
            }
        }

        if (Input.GetButtonDown("Fire1")) {  
            if (!comecou) {
                comecou = true;
                timerAtivo = true; 
                rb.isKinematic = false;
            }
            rb.velocity = Vector2.zero;
            rb.AddForce(forcaImpulso);    
        }

        if (Input.GetKeyDown(KeyCode.Space)) Disparar();
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (!acabou && !vitoria && col.CompareTag("Inimigo")) ProcessarDano();
    }

    public void ReceberDanoDoBoss() {
        if (!vitoria) ProcessarDano();
    }

    public void IniciarVitoria() {
        if (acabou || vitoria) return;
        vitoria = true;
        timerAtivo = false;
        StartCoroutine(CutsceneVitoria());
    }

    IEnumerator CutsceneVitoria() {
        GeradorInimigos.IndicarVitoria();
        if (rb != null) rb.simulated = false;

        float tempo = 0f;
        while (tempo < 2.5f) {
            transform.Translate(Vector3.right * 6f * Time.deltaTime);
            tempo += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
        if (canvasContinua != null) canvasContinua.SetActive(true);
    }

    void ProcessarDano() {
        contadorToques++;
        AtualizarTextoVida();

        if (sr != null && !piscando) StartCoroutine(EfeitoPiscar());
        if (contadorToques >= maxToques) FinalizarJogo();
    }

    IEnumerator EfeitoPiscar() {
        piscando = true;
        float tempo = 0f;

        while (tempo < 1.5f) {
            sr.color = corDano;
            yield return new WaitForSeconds(0.15f);
            sr.color = corOriginal;
            yield return new WaitForSeconds(0.15f);
            tempo += 0.3f;
        }

        sr.color = corOriginal;
        piscando = false;
    }

    void Disparar() {
        if (projetilPrefab != null) {
            Vector3 pos = pontoDisparo != null ? pontoDisparo.position : transform.position;
            pos.z = 0f;
            Instantiate(projetilPrefab, pos, Quaternion.identity);
        }
    }

    void AtualizarTextoVida() {
        if (textoVida != null) {
            textoVida.text = "AIRPLANE HEALTH " + Mathf.Max(0, maxToques - contadorToques);
        }
    }

    void AtualizarTextoTimer() {
        if (textoTimer != null) {
            textoTimer.text = "TIME: " + Mathf.CeilToInt(tempoRestante);
        }
    }

    void FinalizarJogo() {
        acabou = true;
        timerAtivo = false;

        if (canvasGameOver != null) canvasGameOver.SetActive(true);
        if (rb != null) {
            rb.isKinematic = false;
            rb.gravityScale = 2f;
            rb.velocity = new Vector2(2f, -5f);
        }
    }
}