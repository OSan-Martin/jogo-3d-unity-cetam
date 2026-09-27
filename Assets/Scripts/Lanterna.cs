using UnityEngine;
using System.Collections;

public class Lanterna : MonoBehaviour
{
    [Header("Componentes")]
    public Light luzDaLanterna;

    [Header("Referências de Áudio")]
    public AudioSource sfxSource;
    public AudioSource humSource;
    public AudioSource falhaSource;
    public AudioClip somDeClique;
    public AudioClip somDoHum;
    public AudioClip somDeFalha;
    public AudioClip somDeRecarga;

    [Header("Configurações da Bateria")]
    public float bateriaMax = 100f;
    public float consumoPorSegundo = 1f;
    public float bateriaAtual;
    
    [Header("Configurações de Intensidade")]
    public float intensidadeMax = 2f;
    public float intensidadeMin = 0.7f;
    
    [Header("Limites da Bateria (em porcentagem 0-1)")]
    [Range(0f, 1f)]
    public float porcentagemParaEnfraquecer = 0.5f;
    [Range(0f, 1f)]
    public float porcentagemBateriaCritica = 0.2f;
    [Range(0f, 1f)]
    public float porcentagemParaPiscar = 0.05f;

    [Header("Configurações de Detecção")]
    public float alcanceDaDeteccao = 15f;
    public float raioDaDeteccao = 0.5f;
    public Color corDoBrilho = Color.yellow; 

    private bool usuarioQuerLanternaLigada = false;
    private float tempoProximoPiscar = 0f;
    private Renderer itemSendoObservadoRenderer;

    void Start()
    {
        if (luzDaLanterna == null || sfxSource == null || humSource == null || falhaSource == null) 
        {
            Debug.LogError("Componentes não atribuídos no script da Lanterna!", this.gameObject);
            this.enabled = false;
            return;
        }
        luzDaLanterna.enabled = false;
        bateriaAtual = bateriaMax;
        humSource.clip = somDoHum;
        humSource.loop = true;
    }

    void Update()
    {
        LidarComInput();
        AtualizarEstadoLanterna();
        LidarComDeteccao();
    }

    // --- MÉTODOS PÚBLICOS (Chamados por outros scripts ou eventos) ---

    public void RecarregarBateria()
    {
        bateriaAtual = bateriaMax;
        if (somDeRecarga != null)
        {
            sfxSource.PlayOneShot(somDeRecarga);
        }
    }

    // Função para ser chamada pela animação de ENTRADA
    public void LigarLanterna()
    {
        usuarioQuerLanternaLigada = true;
        if (somDeClique != null)
        {
            sfxSource.PlayOneShot(somDeClique);
        }
    }

    // Função para ser chamada pela animação de SAÍDA
    public void DesligarLanterna()
    {
        usuarioQuerLanternaLigada = false;
        if (somDeClique != null)
        {
            sfxSource.PlayOneShot(somDeClique);
        }
    }

    // --- MÉTODOS PRIVADOS (Lógica interna da Lanterna) ---

    private void LidarComInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            usuarioQuerLanternaLigada = !usuarioQuerLanternaLigada;
            if (somDeClique != null)
            {
                sfxSource.PlayOneShot(somDeClique);
            }
        }
    }

    private void AtualizarEstadoLanterna()
    {
        if (!usuarioQuerLanternaLigada || bateriaAtual <= 0)
        {
            luzDaLanterna.enabled = false;
            if (humSource.isPlaying) { humSource.Stop(); }
            return;
        }

        luzDaLanterna.enabled = true;
        if (!humSource.isPlaying) { humSource.Play(); }
        
        bateriaAtual -= consumoPorSegundo * Time.deltaTime;
        bateriaAtual = Mathf.Max(bateriaAtual, 0f);
        float porcentagemAtual = bateriaAtual / bateriaMax;

        AtualizarIntensidade(porcentagemAtual);
        LidarComPisca(porcentagemAtual);
    }
    
    private void AtualizarIntensidade(float porcentagem)
    {
        float t = Mathf.InverseLerp(porcentagemBateriaCritica, porcentagemParaEnfraquecer, porcentagem);
        t = Mathf.Clamp01(t);
        luzDaLanterna.intensity = Mathf.Lerp(intensidadeMin, intensidadeMax, t);
    }

    private void LidarComPisca(float porcentagem)
    {
        if (porcentagem <= porcentagemParaPiscar)
        {
            if (Time.time >= tempoProximoPiscar)
            {
                luzDaLanterna.enabled = !luzDaLanterna.enabled;
                tempoProximoPiscar = Time.time + Random.Range(0.05f, 0.3f);
                falhaSource.PlayOneShot(somDeFalha);
            }
        }
    }

    private void LidarComDeteccao()
    {
        if (!luzDaLanterna.enabled)
        {
            if (itemSendoObservadoRenderer != null)
            {
                DesativarBrilho(itemSendoObservadoRenderer);
                itemSendoObservadoRenderer = null;
            }
            return;
        }

        RaycastHit hit;
        if (Physics.SphereCast(luzDaLanterna.transform.position, raioDaDeteccao, luzDaLanterna.transform.forward, out hit, alcanceDaDeteccao))
        {
            if (hit.collider.CompareTag("ObjetosPerdidos"))
            {
                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend != null && rend != itemSendoObservadoRenderer)
                {
                    if (itemSendoObservadoRenderer != null) { DesativarBrilho(itemSendoObservadoRenderer); }
                    AtivarBrilho(rend);
                    itemSendoObservadoRenderer = rend;
                }
                return;
            }
        }
        
        if (itemSendoObservadoRenderer != null)
        {
            DesativarBrilho(itemSendoObservadoRenderer);
            itemSendoObservadoRenderer = null;
        }
    }

    private void AtivarBrilho(Renderer rend)
    {
        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", corDoBrilho);
    }

    private void DesativarBrilho(Renderer rend)
    {
        rend.material.SetColor("_EmissionColor", Color.black);
    }
}