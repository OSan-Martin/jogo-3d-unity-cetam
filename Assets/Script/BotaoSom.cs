using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class BotaoSom : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip somHover;   // Som quando passa o mouse
    public AudioClip somClick;   // Som quando clica

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Quando o mouse passa por cima
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (somHover != null)
            audioSource.PlayOneShot(somHover);
    }

    // Quando clica no botão
    public void OnPointerClick(PointerEventData eventData)
    {
        if (somClick != null)
            audioSource.PlayOneShot(somClick);
    }
}
