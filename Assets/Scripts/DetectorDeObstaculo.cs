using UnityEngine;

public class DetectorDeObstaculo : MonoBehaviour
{
    private Movimento movimentoScript;

    void Start()
    {
        movimentoScript = GetComponentInParent<Movimento>();
        if (movimentoScript == null)
        {
            Debug.LogError("O script 'Movimento' não foi encontrado no objeto pai!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // PRIMEIRO, a gente checa a etiqueta do intruso.
        if (other.CompareTag("Mao") || other.CompareTag("Player"))
        {
            // Se for a mão, a gente manda ele calar a boca e sair. O 'return' para a execução aqui.
            return; 
        }

        // Se NÃO for a mão, aí sim ele faz a fofoca pro script principal.
        movimentoScript.SetPertoDeObstaculo(true);
    }

    private void OnTriggerExit(Collider other)
    {
        // Mesma lógica na saída, pra evitar bugs.
        if (other.CompareTag("Mao") || other.CompareTag("Player"))
        {
            return;
        }

        movimentoScript.SetPertoDeObstaculo(false);
    }
}