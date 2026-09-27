using UnityEngine;

public class GerenteUIPrancheta : MonoBehaviour
{
    [Header("Configuração da UI")]
    // Arraste aqui os GameObjects que servem como "checkmarks"
    public GameObject[] marcasDeChecagem;

    // <<< NOVO >>>
    [Header("Controle de Missão")]
    [Tooltip("Quantos itens o jogador precisa coletar para terminar a missão.")]
    public int totalDeItensParaColetar = 6;
    [Tooltip("Arraste aqui o Collider do gatilho do carro que deve ser ativado.")]
    public Collider gatilhoDoCarro;

    [SerializeField]
    private int itensColetados;
    // <<< FIM DO NOVO >>>

    void Start()
    {
        itensColetados = 0; // Garante que a contagem ZERE toda vez que o jogo começa

        // Garante que todos os checkmarks comecem desligados.
        foreach (GameObject marca in marcasDeChecagem)
        {
            if (marca != null)
            {
                marca.SetActive(false);
            }
        }
        
        // <<< NOVO >>>
        // Garante que o gatilho do carro comece desativado
        if (gatilhoDoCarro != null)
        {
            gatilhoDoCarro.enabled = false;
        }
        // <<< FIM DO NOVO >>>
    }

    public void MarcarItemColetado(int itemID)
    {
        if (itemID >= 0 && itemID < marcasDeChecagem.Length)
        {
            // Checa se este item já não foi marcado, para evitar contar duas vezes
            if (!marcasDeChecagem[itemID].activeSelf)
            {
                marcasDeChecagem[itemID].SetActive(true);
                
                // <<< NOVO >>>
                itensColetados++; // Incrementa nosso contador
                VerificarCondicaoDeVitoria(); // Checa se o jogo acabou
                // <<< FIM DO NOVO >>>
            }
        }
        else
        {
            Debug.LogWarning("Tentativa de marcar um item com ID inválido: " + itemID);
        }
    }

    // <<< NOVO MÉTODO >>>
    private void VerificarCondicaoDeVitoria()
    {
        // Checa se o número de itens coletados atingiu o total necessário
        if (itensColetados >= totalDeItensParaColetar)
        {
            Debug.Log("TODOS OS ITENS COLETADOS! A saída do mapa foi liberada.");
            if (gatilhoDoCarro != null)
            {
                gatilhoDoCarro.enabled = true; // Ativa o gatilho do carro!
            }
        }
    }
    // <<< FIM DO NOVO MÉTODO >>>
}