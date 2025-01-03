using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string interactionPrompt = "Tekan Spasi untuk berinteraksi";
    public bool isInteractable = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowInteractionPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HideInteractionPrompt();
        }
    }

    private void Update()
    {
        if (isInteractable && Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }
    }

    void ShowInteractionPrompt()
    {
        // Tampilkan pesan interaksi atau UI prompt
        Debug.Log(interactionPrompt);
    }

    void HideInteractionPrompt()
    {
        // Sembunyikan pesan interaksi atau UI prompt
        Debug.Log("Tutup pesan interaksi");
    }

    void Interact()
    {
        // Logika interaksi objek
        Debug.Log("Objek diinteraksi!");
    }
}