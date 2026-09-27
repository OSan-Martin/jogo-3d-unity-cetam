using UnityEngine;

public class ColecionarItem : MonoBehaviour
{
    [Header("Conexões")]
    public GerenteUIPrancheta gerenteDaPrancheta;
    
    // <<< NOVO: Referência para o script da Lanterna >>>
    public Lanterna scriptDaLanterna;

    private void OnTriggerEnter(Collider other)
    {
        // Checa se é um item da prancheta
        if (other.CompareTag("ObjetosPerdidos"))
        {
            InfoDoItem infoDoItem = other.GetComponent<InfoDoItem>();

            if (infoDoItem != null && gerenteDaPrancheta != null)
            {
                gerenteDaPrancheta.MarcarItemColetado(infoDoItem.itemID);
            }
            
            Destroy(other.gameObject);
        }
        // <<< NOVO: Checa se é uma Bateria >>>
        if (other.CompareTag("Bateria"))
        {
            // Se a gente tem a referência da lanterna...
            if (scriptDaLanterna != null)
            {
                // ...manda ela se recarregar!
                scriptDaLanterna.RecarregarBateria();
            }

            // E então destrói a bateria
            Destroy(other.gameObject);
        }
    }
}