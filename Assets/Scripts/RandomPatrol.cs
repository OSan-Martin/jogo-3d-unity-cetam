using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RandomPatrol : MonoBehaviour
{
    [Header("Configuração de Spawn Inicial")]
    [Tooltip("Se marcado, o monstro tentará aparecer em um local aleatório perto do jogador no início do jogo.")]
    public bool spawnNearPlayer = true;
    [Tooltip("O raio ao redor do jogador onde o monstro pode aparecer. Também define a distância máxima antes do teletransporte.")]
    public float spawnRadius = 15f;

    [Header("Configuração de Teletransporte Dinâmico")]
    [Tooltip("Se marcado, o monstro se teletransportará para perto do jogador se ele se afastar demais.")]
    public bool enableDynamicTeleport = true;
    [Tooltip("A frequência em segundos para checar se o jogador está muito longe.")]
    public float teleportCheckFrequency = 5f;
    
    [Header("Configuração do Movimento")]
    public float minWalkRadius = 5f;
    public float maxWalkRadius = 20f;
    public float idleTimeMin = 1f;
    public float idleTimeMax = 4f;

    [Header("Detecção do Player")]
    public Transform player;
    public float visionRange = 10f;
    public float visionAngle = 120f;

    [Header("Vinheta HUD")]
    public Image vinhetaImage;
    public float vinhetaSpeed = 0.5f;

    [Header("Distância Crítica para Tensão")]
    public float distanciaPerto = 3f;

    [Header("Vinheta Pulsante")]
    public float pulsateSpeed = 3f;
    public float pulsateAmount = 0.1f;

    [Header("Ataque")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    
    [Header("Sons do Monstro")]
    public AudioSource sfxSource;
    public AudioClip somDeDeteccao; 
    public AudioSource passosSource;
    public AudioClip somDePasso;
    public float frequenciaDosPassos = 0.5f;

    [Header("Componentes Auxiliares")]
    public JumpscareManager jumpscareManager;
    
    private bool canAttack = true;
    private NavMeshAgent agent;
    private Animator anim;
    private bool isChasing = false;
    private bool isAttacking = false;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    
    private float proximoPassoTimer = 0f;
    private bool jaGritou = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        if (anim) anim.applyRootMotion = false;

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        if (spawnNearPlayer && player != null)
        {
            TeleportNearPlayer();
        }

        if (passosSource != null)
        {
            passosSource.playOnAwake = false;
        }

        StartCoroutine(PatrolRoutine());
        
        if (enableDynamicTeleport)
        {
            StartCoroutine(CheckPlayerDistanceRoutine());
        }
    }

    void Update()
    {
        if (!agent || !player || !vinhetaImage) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // Lógica da Vinheta...
        if (distance <= distanciaPerto)
        {
            Color color = vinhetaImage.color;
            float baseAlpha = 1f;
            color.a = baseAlpha + Mathf.Sin(Time.time * pulsateSpeed) * pulsateAmount;
            vinhetaImage.color = color;
        }
        else
        {
            Color color = vinhetaImage.color;
            float targetAlpha = Mathf.Clamp01(1f - (distance - distanciaPerto) / (visionRange - distanciaPerto));
            color.a = targetAlpha;
            vinhetaImage.color = color;
        }

        // Lógica de perseguição e ataque...
        if (IsPlayerInVision())
        {
            if (!isChasing && !isAttacking)
            {
                StopAllCoroutines();
                StartCoroutine(CheckPlayerDistanceRoutine()); // Garante que a checagem continue
                isChasing = true;

                if (!jaGritou)
                {
                    if (sfxSource != null && somDeDeteccao != null)
                    {
                        sfxSource.PlayOneShot(somDeDeteccao);
                    }
                    jaGritou = true;
                }
                
                anim.ResetTrigger("StopRun");
                anim.SetTrigger("StartRun");
            }

            if (distance > attackRange)
            {
                isAttacking = false;
                agent.isStopped = false;
                agent.SetDestination(player.position);
                anim.SetTrigger("StartRun");
                anim.ResetTrigger("StopRun");
            }
            else
            {
                if (!isAttacking && canAttack)
                    StartCoroutine(Attack());
            }
        }
        else
        {
            if (isChasing || isAttacking)
            {
                isChasing = false;
                isAttacking = false;
                jaGritou = false;
                StopAllCoroutines();
                StartCoroutine(PatrolRoutine());
                StartCoroutine(CheckPlayerDistanceRoutine());
            }

            if (agent.velocity.magnitude > 0.1f)
            {
                anim.SetTrigger("StartRun");
                anim.ResetTrigger("StopRun");
            }
            else
            {
                anim.SetTrigger("StopRun");
                anim.ResetTrigger("StartRun");
            }
        }
        
        // Lógica de passos com frequência
        if (!isAttacking && agent.velocity.magnitude > 0.1f)
        {
            if (Time.time >= proximoPassoTimer)
            {
                if (passosSource != null && somDePasso != null)
                {
                    passosSource.PlayOneShot(somDePasso);
                }
                proximoPassoTimer = Time.time + frequenciaDosPassos;
            }
        }
    }
    
    IEnumerator Attack()
    {
        isAttacking = true;
        canAttack = false;
        agent.isStopped = true;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        anim.SetTrigger("Attack");
        if(jumpscareManager != null) { jumpscareManager.TriggerJumpscare(); }
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        isAttacking = false;
        agent.isStopped = false;
    }

    void MoveToRandomPoint()
    {
        float radius = Random.Range(minWalkRadius, maxWalkRadius);
        Vector3 randomDirection = Random.insideUnitSphere * radius + transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas)) 
        { 
            agent.SetDestination(hit.position); 
        }
    }

    // <<< ROTINA DE PATRULHA CORRIGIDA >>>
    IEnumerator PatrolRoutine()
    {
        while (!isChasing)
        {
            // 1. Define um novo destino e começa a se mover IMEDIATAMENTE.
            MoveToRandomPoint();

            // Checa se um caminho válido foi encontrado antes de prosseguir
            if (agent.hasPath)
            {
                agent.isStopped = false;
                anim.SetTrigger("StartRun");
                anim.ResetTrigger("StopRun");

                // 2. Espera até chegar perto do destino.
                while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance && !isChasing)
                {
                    yield return null; // Espera um frame
                }
            }

            // 3. Chegou ao destino, AGORA SIM ele para e espera um tempo.
            // Checa de novo se não está perseguindo, pois o jogador pode ter sido visto durante a caminhada
            if (!isChasing)
            {
                agent.isStopped = true;
                anim.SetTrigger("StopRun");
                anim.ResetTrigger("StartRun");
                float waitTime = Random.Range(idleTimeMin, idleTimeMax);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    void TeleportNearPlayer()
    {
        Vector3 randomPoint = player.position + Random.insideUnitSphere * spawnRadius;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, spawnRadius, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            StopAllCoroutines();
            StartCoroutine(PatrolRoutine());
            StartCoroutine(CheckPlayerDistanceRoutine());
            Debug.Log("Monstro teletransportado para perto do jogador em: " + hit.position);
        }
        else
        {
            Debug.LogWarning("Falha ao encontrar um ponto de teletransporte válido perto do jogador.");
        }
    }

    IEnumerator CheckPlayerDistanceRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(teleportCheckFrequency);

            if (!isChasing && !isAttacking && player != null)
            {
                float distance = Vector3.Distance(transform.position, player.position);

                if (distance > spawnRadius)
                {
                    Debug.Log("Jogador está muito longe (" + distance + "m). Teletransportando monstro...");
                    TeleportNearPlayer();
                }
            }
        }
    }

    bool IsPlayerInVision()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= visionRange)
        {
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            if (angle < visionAngle * 0.5f)
            {
                if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, visionRange))
                {
                    if (hit.transform == player) return true;
                }
            }
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Vector3 fovLine1 = Quaternion.AngleAxis(visionAngle * 0.5f, Vector3.up) * transform.forward * visionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-visionAngle * 0.5f, Vector3.up) * transform.forward * visionRange;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
    }

    public void ResetPosition()
    {
        StopAllCoroutines();
        isAttacking = false;
        isChasing = false;
        canAttack = true;
        jaGritou = false;
        agent.isStopped = true;
        agent.ResetPath();
        agent.Warp(initialPosition);
        transform.rotation = initialRotation;
        StartCoroutine(PatrolRoutine());
        StartCoroutine(CheckPlayerDistanceRoutine());
    }
}
