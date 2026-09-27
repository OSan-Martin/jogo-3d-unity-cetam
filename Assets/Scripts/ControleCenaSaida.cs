using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ControleCenaSaida : MonoBehaviour
{
    [Header("Controle do Jogador e Cena")]
    [Tooltip("Arraste aqui o objeto principal do seu jogador.")]
    public Transform jogador;

    [Header("Posicionamento da Cena")]
    [Tooltip("Objeto vazio que marca a posição/rotação final do jogador na cena.")]
    public Transform pontoFinalDoPlayer;

    [Header("Componentes da Cena")]
    public AudioSource audioSource;
    public Animator animadorDaCena;

    [Header("Assets da Cena")]
    public AudioClip somPortaCarro;
    public AudioClip somFinal;
    public string nomeDaAnimacaoFinal;
    public string nomeDaCenaMenu;

    [Header("Configurações de Tempo")]
    public float duracaoCenaFinal = 10.0f;

    private bool cenaIniciada = false;

    // <<< NOVO MÉTODO UPDATE >>>
    void Update()
    {
        // Se a cena final começou, a gente assume o controle total da posição
        if (cenaIniciada && jogador != null && pontoFinalDoPlayer != null)
        {
            // A MARRETA: Força a posição e rotação a cada frame.
            jogador.position = pontoFinalDoPlayer.position;
            jogador.rotation = pontoFinalDoPlayer.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !cenaIniciada)
        {
            cenaIniciada = true;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(ExecutarSequenciaFinal());
        }
    }
    
    IEnumerator ExecutarSequenciaFinal()
    {

        // O teleporte inicial ainda é bom para garantir a posição antes do primeiro frame
        if (pontoFinalDoPlayer != null && jogador != null)
        {
            jogador.position = pontoFinalDoPlayer.position;
            jogador.rotation = pontoFinalDoPlayer.rotation;
        }

        // Toca som da porta e espera no escuro
        if (audioSource != null && somPortaCarro != null)
        {
            audioSource.PlayOneShot(somPortaCarro);
        }

        // Inicia a animação e o som final
        if (animadorDaCena != null && !string.IsNullOrEmpty(nomeDaAnimacaoFinal))
        {
            animadorDaCena.Play(nomeDaAnimacaoFinal);
        }
        if (audioSource != null && somFinal != null)
        {
            audioSource.PlayOneShot(somFinal);
        }

        // Espera a cena acabar
        yield return new WaitForSeconds(duracaoCenaFinal);

        // Volta para o menu
        SceneManager.LoadScene(nomeDaCenaMenu);
    }
}