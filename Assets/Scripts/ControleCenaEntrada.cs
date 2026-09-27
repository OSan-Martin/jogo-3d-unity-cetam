using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControleCenaEntrada : MonoBehaviour
{
    [Header("Scripts para Desativar")]
    // Arraste aqui TODOS os scripts que dão controle ao jogador
    public MonoBehaviour[] scriptsParaDesativar;

    [Header("Configuração do Fade")]
    public Image telaPretaFade;
    public float duracaoFade = 3.0f;

    [Header("Configuração de Tempo da Cena")]
    // Duração total da cena de entrada. Depois disso, os controles voltam.
    public float duracaoTotalDaCena = 8.0f;


    void Start()
    {
        // Desativa todos os scripts de controle no início
        foreach (MonoBehaviour script in scriptsParaDesativar)
        {
            if (script != null) script.enabled = false;
        }

        // Configura a tela preta e inicia a sequência
        telaPretaFade.gameObject.SetActive(true);
        telaPretaFade.color = new Color(0, 0, 0, 1f);
        StartCoroutine(SequenciaDeEntrada());
    }

    private IEnumerator SequenciaDeEntrada()
    {
        // --- PARTE 1: O FADE-IN ---
        float tempo = 0;
        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, tempo / duracaoFade);
            telaPretaFade.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        telaPretaFade.color = new Color(0, 0, 0, 0f);

        // --- PARTE 2: ESPERAR A CENA ACABAR ---
        // Espera o tempo total definido para a cena
        yield return new WaitForSeconds(duracaoTotalDaCena);

        // --- PARTE 3: DEVOLVER O CONTROLE ---
        foreach (MonoBehaviour script in scriptsParaDesativar)
        {
            if (script != null) script.enabled = true;
        }

        // Desliga a imagem preta para não atrapalhar
        telaPretaFade.gameObject.SetActive(false);
    }
}