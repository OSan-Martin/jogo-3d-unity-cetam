using UnityEngine;
using System.Collections;

public class GerenteDeItens : MonoBehaviour
{
    [Header("Animators dos Itens")]
    public Animator animPrancheta;
    public Animator animLanterna;
    public Animator animMachado;

    [Header("Configurações de Atraso")]
    [Tooltip("Tempo de espera ao guardar o machado para trocar de item.")]
    public float tempoDeEspera = 1.2f;

    void Update()
    {
        // Isso aqui continua perfeito
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(IntercalarPrancheta());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(IntercalarLanterna());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(EquiparMachado());
        }
    }

    // --- MÉTODOS PARA EVENTOS DE ANIMAÇÃO (NOVO) ---

    public void EventoEquiparPrancheta()
    {
        // Garante que o machado seja guardado e equipa a prancheta
        animMachado.SetBool("EstaEquipado", false);
        animPrancheta.SetBool("EstaEquipado", true);
    }

    public void EventoEquiparLanterna()
    {
        // Garante que o machado seja guardado e equipa a lanterna
        animMachado.SetBool("EstaEquipado", false);
        animLanterna.SetBool("EstaEquipado", true);
    }

    public void EventoEquiparMachado()
    {
        // Guarda os outros itens e equipa o machado
        animPrancheta.SetBool("EstaEquipado", false);
        animLanterna.SetBool("EstaEquipado", false);
        animMachado.SetBool("EstaEquipado", true);
    }


    // --- CORROTINAS (LÓGICA ANTIGA, MANTIDA) ---

    IEnumerator IntercalarPrancheta()
    {
        // Se o machado estiver na mão...
        if (animMachado.GetBool("EstaEquipado"))
        {
            // ...primeiro guarda ele...
            animMachado.SetBool("EstaEquipado", false);
            // ...e só então espera.
            yield return new WaitForSeconds(tempoDeEspera);
        }

        // Depois da possível espera, faz a lógica da prancheta
        bool estadoAtual = animPrancheta.GetBool("EstaEquipado");
        animPrancheta.SetBool("EstaEquipado", !estadoAtual);
    }

    IEnumerator IntercalarLanterna()
    {
        // Lógica idêntica à da prancheta
        if (animMachado.GetBool("EstaEquipado"))
        {
            animMachado.SetBool("EstaEquipado", false);
            yield return new WaitForSeconds(tempoDeEspera);
        }

        bool estadoAtual = animLanterna.GetBool("EstaEquipado");
        animLanterna.SetBool("EstaEquipado", !estadoAtual);
    }

    IEnumerator EquiparMachado()
    {
        // A lógica do machado é sempre a mesma, não precisa de if
        animPrancheta.SetBool("EstaEquipado", false);
        animLanterna.SetBool("EstaEquipado", false);

        yield return new WaitForSeconds(tempoDeEspera);

        animMachado.SetBool("EstaEquipado", true);
    }
}