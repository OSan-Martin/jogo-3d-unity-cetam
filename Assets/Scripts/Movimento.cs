using UnityEngine;

public class Movimento : MonoBehaviour
{
    // ... (Todas as suas variáveis continuam exatamente iguais)
    [Header("Controles de Velocidade")]
    public float velocidadeAgachado = 3f;
    public float velocidadeAndando = 7f;
    public float velocidadeCorrida = 12f;

    [Header("Controles de Stamina")]
    public float maxStamina = 100f;
    public float drenoDeStamina = 20f;
    public float regeneracaoDeStamina = 15f;
    public float delayParaRegenerar = 3f;

    [Header("Configurações de Agachar")]
    public float alturaAgachado = 1f;
    public KeyCode teclaAgachar = KeyCode.LeftControl;
    public float velocidadeDaTransicao = 10f;

    [Header("Suavização da Animação")]
    public float suavizacaoDaAnimacao = 10f;

    [Header("Referências de Áudio")]
    public AudioSource audioSourcePassos;
    public AudioSource audioSourceRespiracao;
    public AudioClip somPassoPadrao;
    public AudioClip somCorrendo;
    [Range(0f, 1f)] public float volumeMinimoRespiracao = 0.0f;
    [Range(0f, 1f)] public float volumeMaximoRespiracao = 0.6f;
    public float intervaloPassoAndando = 0.5f;
    public float intervaloPassoCorrendo = 0.3f;
    public float intervaloPassoAgachado = 0.7f;

    // --- Variáveis Internas ---
    private float velocidadeAtual;
    private float staminaAtual;
    private bool estaPertoDeObstaculo = false;
    private float cronometroDelayRegen;
    private bool estaAgachado;
    private float proximoPasso;
    private AudioClip somPassoAtual;

    // --- Componentes e Cálculos ---
    private Animator anim;
    private CapsuleCollider capsuleCollider;
    private float alturaEmPe;
    private Vector3 centroColliderEmPe;
    private Vector3 ultimaPosicao;
    private float velocidadeSuaveParaAnimacao;

    void Start()
    {
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        
        staminaAtual = maxStamina;
        velocidadeAtual = velocidadeAndando;
        ultimaPosicao = transform.position;
        alturaEmPe = capsuleCollider.height;
        centroColliderEmPe = capsuleCollider.center;

        somPassoAtual = somPassoPadrao;

        if (audioSourceRespiracao != null && somCorrendo != null)
        {
            audioSourceRespiracao.clip = somCorrendo;
            audioSourceRespiracao.loop = true;
            audioSourceRespiracao.Play();
        }
    }

    void Update()
    {
        Vector3 inputMovimento = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        LidarComAgachar();
        LidarComCorridaEStamina(inputMovimento);
        AplicarMovimento(inputMovimento);
        AtualizarAnimacao();
        
        // --- MUDANÇAS AQUI ---
        VerificarSuperficie(); // Novo método pra checar o chão
        LidarComAudioDePassos(inputMovimento);
        LidarComAudioDeRespiracao(); 
    }

    // --- NOVO MÉTODO USANDO RAYCAST ---
    private void VerificarSuperficie()
    {
        // Atira um raio de 1.5m para baixo a partir da posição do personagem
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
        {
            // Tenta pegar o script InfoSuperficie no objeto que o raio atingiu
            if (hit.collider.TryGetComponent<InfoSuperficie>(out InfoSuperficie superficie))
            {
                // Se encontrou, define o som do passo atual para o som daquela superfície
                somPassoAtual = superficie.somDoPasso;
                return; // Sai do método pra não resetar o som logo em seguida
            }
        }

        // Se o raio não bateu em nada com o script, usa o som padrão
        somPassoAtual = somPassoPadrao;
    }

    private void LidarComAudioDeRespiracao()
    {
        if (audioSourceRespiracao == null) return;

        float cansaco = 1f - (staminaAtual / maxStamina);
        audioSourceRespiracao.volume = Mathf.Lerp(volumeMinimoRespiracao, volumeMaximoRespiracao, cansaco);
    }

    private void LidarComAudioDePassos(Vector3 inputMovimento)
    {
        // O resto do método continua exatamente igual
        if (inputMovimento.magnitude < 0.1f) return;
        
        if (proximoPasso > 0)
        {
            proximoPasso -= Time.deltaTime;
        }

        if (proximoPasso <= 0f)
        {
            if (somPassoAtual != null)
            {
                audioSourcePassos.PlayOneShot(somPassoAtual);
            }
            
            if (estaAgachado)
            {
                proximoPasso = intervaloPassoAgachado;
            }
            else if (velocidadeAtual == velocidadeCorrida)
            {
                proximoPasso = intervaloPassoCorrendo;
            }
            else
            {
                proximoPasso = intervaloPassoAndando;
            }
        }
    }

    public void SetPertoDeObstaculo(bool estado)
    {
        estaPertoDeObstaculo = estado;
    }

    // --- APAGUEI OS MÉTODOS OnTriggerEnter e OnTriggerExit DAQUI ---
    // Eles não são mais necessários para o som dos passos.

    // ... (O resto dos seus métodos LidarComAgachar, LidarComCorrida, etc. continuam aqui sem nenhuma alteração)
    private void LidarComAgachar()
    {
        estaAgachado = Input.GetKey(teclaAgachar);

        float alturaAlvo = estaAgachado ? alturaAgachado : alturaEmPe;
        Vector3 centroAlvo = centroColliderEmPe;

        if (estaAgachado)
        {
            velocidadeAtual = velocidadeAgachado;
            
            float alturaDiferenca = alturaEmPe - alturaAgachado;
            centroAlvo = new Vector3(centroColliderEmPe.x, centroColliderEmPe.y + (alturaDiferenca / 2), centroColliderEmPe.z);
        }
        
        capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, alturaAlvo, velocidadeDaTransicao * Time.deltaTime);
        capsuleCollider.center = Vector3.Lerp(capsuleCollider.center, centroAlvo, velocidadeDaTransicao * Time.deltaTime);
    }

    private void LidarComCorridaEStamina(Vector3 inputMovimento)
    {
        if (estaAgachado)
        {
            cronometroDelayRegen -= Time.deltaTime;
            if (cronometroDelayRegen <= 0)
            {
                staminaAtual += regeneracaoDeStamina * Time.deltaTime;
            }
            staminaAtual = Mathf.Clamp(staminaAtual, 0f, maxStamina);
            return; 
        }

        bool tentandoCorrer = Input.GetKey(KeyCode.LeftShift) && inputMovimento.magnitude > 0.1f;

        if (tentandoCorrer && staminaAtual > 0 && !estaPertoDeObstaculo)
        {
            velocidadeAtual = velocidadeCorrida;
            staminaAtual -= drenoDeStamina * Time.deltaTime;
            cronometroDelayRegen = delayParaRegenerar; 
        }
        else
        {
            velocidadeAtual = velocidadeAndando;
            cronometroDelayRegen -= Time.deltaTime;
            if (cronometroDelayRegen <= 0)
            {
                staminaAtual += regeneracaoDeStamina * Time.deltaTime;
            }
        }
        
        staminaAtual = Mathf.Clamp(staminaAtual, 0f, maxStamina);
    }

    private void AplicarMovimento(Vector3 inputMovimento)
    {
        transform.Translate(inputMovimento.normalized * velocidadeAtual * Time.deltaTime);
    }

    private void AtualizarAnimacao()
    {
        float distancia = Vector3.Distance(transform.position, ultimaPosicao);
        float velocidadeRealCalculada = 0f;
        if (Time.deltaTime > 0)
        {
            velocidadeRealCalculada = distancia / Time.deltaTime;
        }
        ultimaPosicao = transform.position;

        velocidadeSuaveParaAnimacao = Mathf.Lerp(velocidadeSuaveParaAnimacao, velocidadeRealCalculada, Time.deltaTime * suavizacaoDaAnimacao);
        
        float cansaco = 1 - (staminaAtual / maxStamina);

        anim.SetFloat("Velocidade", velocidadeSuaveParaAnimacao + 1); 
        anim.SetBool("Correndo", velocidadeAtual == velocidadeCorrida && !estaAgachado);
        anim.SetFloat("Cansaco", cansaco);
    }
}