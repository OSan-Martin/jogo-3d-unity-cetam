using UnityEngine;
using System.Collections;

public class AcionarCenaMachado : MonoBehaviour
{
    [Header("Referências do Jogador")]
    [Tooltip("O Animator que está no seu jogador.")]
    public Animator animadorDoJogador;
    [Tooltip("Arraste aqui o objeto principal do seu jogador.")]
    public Transform jogador;
    [Tooltip("Lista de scripts no jogador que devem ser desativados durante a cena.")]
    public MonoBehaviour[] scriptsParaDesativar;

    [Header("Controle da Cena")]
    [Tooltip("Objeto vazio que marca a posição/rotação final do jogador.")]
    public Transform pontoDeEncaixe;
    [Tooltip("O objeto que será DESTRUÍDO quando a cena começar (ex: o machado no chão).")]
    public GameObject objetoParaDestruir;
    [Tooltip("O objeto que será ATIVADO quando a cena começar (ex: o machado na mão do player).")]
    public GameObject objetoParaAtivar;

    [Header("Animação")]
    [Tooltip("Nome EXATO do gatilho (Trigger) no Animator do jogador a ser disparado.")]
    public string nomeDoGatilhoAnimacao;

    private bool cenaIniciada = false;

    private void OnTriggerEnter(Collider other)
    {
        // 1. Checa se quem entrou foi o Player e se a cena já não começou
        if (other.CompareTag("Player") && !cenaIniciada)
        {
            cenaIniciada = true; // Impede que o trigger dispare de novo
            StartCoroutine(ExecutarCenaPegarMachado());
        }
    }

    IEnumerator ExecutarCenaPegarMachado()
    {
        // 2. Tira o controle do jogador
        foreach (var script in scriptsParaDesativar)
        {
            if(script != null) script.enabled = false;
        }

        // 3. Posiciona o jogador no ponto exato
        if (jogador != null && pontoDeEncaixe != null)
        {
            jogador.position = pontoDeEncaixe.position;
            jogador.rotation = pontoDeEncaixe.rotation;
        }

        // 4. Manipula os objetos de cena

        Destroy(objetoParaDestruir);

        objetoParaAtivar.SetActive(true);

        // 5. Dispara a animação no jogador
        if (animadorDoJogador != null && !string.IsNullOrEmpty(nomeDoGatilhoAnimacao))
        {
            animadorDoJogador.Play(nomeDoGatilhoAnimacao);
        }

        // A corrotina termina aqui, mas a animação continuará rodando.
        // Se precisar reativar os controles do jogador depois da animação,
        // você usaria um Evento de Animação no final dela para chamar uma outra função.
        yield return null; 
    }
}