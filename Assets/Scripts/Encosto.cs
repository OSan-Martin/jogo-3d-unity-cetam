using UnityEngine;

public class Encosto : MonoBehaviour
{
    [Header("Objetos Alvo")]
    public Animator animatorAlvo;

    [Header("Configurações da Animação")]
    public string animacaoDeRecuo; // Nome do estado da 1ª parte
    public string animacaoDeVolta;  // Nome do estado da 2ª parte
    public string tagDoCorpo = "Corpo";

    private void OnTriggerEnter(Collider other)
    {
        // Se um objeto com a tag certa entrar...
        if (! other.CompareTag(tagDoCorpo))
        {
            // ...manda o Animator tocar a animação de RECUAR.
            animatorAlvo.Play(animacaoDeRecuo);
        }
    }

    private void OnTriggerExit(Collider other)
    {
            // ...manda o Animator tocar a animação de VOLTAR.
            animatorAlvo.Play(animacaoDeVolta);

    }
}