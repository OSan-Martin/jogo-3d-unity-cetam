using UnityEngine;

public class MachadoAtaque : MonoBehaviour
{
    [Header("Objetos Alvo")]
    public Animator animatorAlvo;
    public string animacao;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animatorAlvo.Play(animacao);
        }
    }

    // --- LÓGICA DE COLISÃO ADICIONADA AQUI ---
    private void OnTriggerEnter(Collider other)
    {
        // 1. Checa se o objeto tem a tag que a gente quer.
        if (other.CompareTag("PortaDerrubar"))
        {
            // 2. Tenta pegar o script "AlvoAnimado" no objeto atingido.
            AlvoAnimado alvo = other.GetComponent<AlvoAnimado>();

            // 3. Verifica se o script existe (pra não dar erro se você esquecer de colocar).
            if (alvo != null)
            {
                // 4. Se tudo estiver certo, usa as informações do alvo para tocar a animação.
                alvo.animatorDoAlvo.Play(alvo.nomeDaAnimacao);
            }
        }
    }
}