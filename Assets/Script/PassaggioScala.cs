using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider2D))]
public class PassaggioScala : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private int pianoDestinazione;

    private void Awake()
    {
        if (!GetComponent<BoxCollider2D>().isTrigger)
        {
            Debug.LogError("PassaggioScala: attiva Is Trigger sul BoxCollider2D.", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GestionePiano giocatore = collision.GetComponentInParent<GestionePiano>();

        if (giocatore == null || !giocatore.enabled)
        {
            return;
        }

        if (giocatore.PianoCorrente == pianoDestinazione)
        {
            return;
        }

        giocatore.CambiaPiano(pianoDestinazione);
    }

}