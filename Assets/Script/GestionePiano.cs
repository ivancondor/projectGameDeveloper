using UnityEngine;

public class GestionePiano : MonoBehaviour
{

    [SerializeField, Range(0, 1)] private int pianoIniziale = 0;

    private SpriteRenderer spriteRenderer;

    private int layerPlayerP0;
    private int layerPlayerP1;

    public int PianoCorrente { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        layerPlayerP0 = LayerMask.NameToLayer("Player_P0");
        layerPlayerP1 = LayerMask.NameToLayer("Player_P1");

        if (spriteRenderer == null || layerPlayerP0 == -1 || layerPlayerP1 == -1)
        {
            Debug.LogError("GestionePiano: manca lo SpriteRenderer oppure una Physics Layer Player_P0/Player_P1.", this);
            enabled = false;
            return;
        }

        CambiaPiano(pianoIniziale);

    }

    public void CambiaPiano(int nuovoPiano)
    {
        if (nuovoPiano != 0 && nuovoPiano != 1)
        {
            return;
        }

        gameObject.layer = nuovoPiano == 0 ? layerPlayerP0 : layerPlayerP1;

        spriteRenderer.sortingLayerName = nuovoPiano == 0 ? "Personaggio_P0" : "Personaggio_P1";

        PianoCorrente = nuovoPiano;

        Debug.Log("Giocatore: raggiunto piano " + PianoCorrente, this);
    }
}