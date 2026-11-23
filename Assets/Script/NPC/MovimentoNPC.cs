using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class MovimentoNPC : MonoBehaviour
{
    [Header("Comportamento")]

    // ModalitaNPC è un enum: permette di scegliere tra un insieme
    // predefinito di comportamenti (Fermo, Pattuglia, Casuale).
    // Nell'Inspector viene visualizzato come un menu a tendina.
    [SerializeField] private ModalitaNPC modalita = ModalitaNPC.Fermo;

    [SerializeField] private float velocita = 2.5f;
    [SerializeField] private Vector2 direzioneIniziale = Vector2.down;

    [Header("Vagabondaggio")]
    [SerializeField] private Transform[] puntiPasseggiata;

    [Header("Pattugliamento")]

    // Transform[] contiene i riferimenti ai punti del percorso.
    // Ogni Transform ci permette di leggere la posizione del punto.
    [SerializeField] private Transform[] puntiPattuglia;

    [Header("Movimento casuale")]

    // Raggio dell'area entro cui generare le destinazioni casuali.
    [SerializeField] private float raggioMovimento = 3f;

    // Raggio del cerchio utilizzato per rilevare gli ostacoli.
    [SerializeField] private float raggioControlloOstacoli = 0.25f;

    // LayerMask permette di scegliere quali Layer devono essere
    // considerati durante il controllo delle collisioni.
    [SerializeField] private LayerMask ostacoli;

    [Header("Pause")]

    // Intervallo utilizzato per generare pause di durata casuale.
    [SerializeField] private float pausaMinima = 1f;
    [SerializeField] private float pausaMassima = 3f;

    [Header("Precisione")]

    // Tolleranza per considerare raggiunta una destinazione.
    // Non serve arrivare esattamente alle stesse coordinate.
    [SerializeField] private float distanzaArrivo = 0.10f;

    private Rigidbody2D corpo;
    private Animator animatore;

    // Indice del prossimo punto da raggiungere nell'array.
    private int indicePunto = 0;

    // Memorizza la posizione iniziale dell'NPC.
    // Sarà il centro dell'area di movimento casuale.
    private Vector2 centroMovimento;

    private Vector2 destinazioneCasuale;

    // Indica se è già stata trovata una destinazione utilizzabile.
    private bool destinazioneValida = false;

    // Contatore del tempo rimanente prima del prossimo movimento.
    private float tempoPausa = 0f;

    // Conserva l'ultima direzione per mantenere l'orientamento
    // corretto quando l'NPC passa all'animazione Idle.
    private Vector2 direzioneGuardata;

    private void Awake()
    {
        corpo = GetComponent<Rigidbody2D>();
        animatore = GetComponent<Animator>();
    }

    private void Start()
    {
        centroMovimento = corpo.position;

        // sqrMagnitude restituisce la lunghezza del vettore al quadrato.
        // È utile per verificare se il vettore è quasi nullo
        // senza dover calcolare la radice quadrata.
        if (direzioneIniziale.sqrMagnitude > 0.1f)
        {
            // normalized restituisce un vettore di lunghezza 1,
            // mantenendo la stessa direzione.
            direzioneGuardata = direzioneIniziale.normalized;
        }
        else
        {
            direzioneGuardata = Vector2.down;
        }

        AggiornaAnimazione(Vector2.zero, false);
    }

    private void FixedUpdate()
    {
        if (modalita == ModalitaNPC.Fermo)
        {
            FermaNPC();
            return;
        }

        if (tempoPausa > 0f)
        {
            // fixedDeltaTime rappresenta l'intervallo temporale
            // tra gli aggiornamenti fisici di Unity.
            // Sottraendolo riduciamo progressivamente il timer.
            tempoPausa -= Time.fixedDeltaTime;

            FermaNPC();
            return;
        }

        // Lo switch esegue il comportamento corrispondente
        // al valore dell'enum selezionato nell'Inspector.
        switch (modalita)
        {
            case ModalitaNPC.Pattuglia:
                GestisciPattuglia();
                break;

            case ModalitaNPC.Casuale:
                GestisciMovimentoCasuale();
                break;

            case ModalitaNPC.Viandante:
                GestisciMovimentoViandante();
                break;
        }
    }

    private void GestisciPattuglia()
    {
        if (puntiPattuglia == null || puntiPattuglia.Length == 0)
        {
            FermaNPC();
            return;
        }

        if (puntiPattuglia[indicePunto] == null)
        {
            // L'operatore % (modulo) restituisce il resto della divisione.
            // Permette di tornare all'indice 0 dopo l'ultimo elemento.
            // Esempio con 3 punti: 0 -> 1 -> 2 -> 0.
            indicePunto = (indicePunto + 1) % puntiPattuglia.Length;

            FermaNPC();
            return;
        }

        // Recuperiamo la posizione del punto corrente.
        // Il Transform contiene le coordinate del GameObject.
        Vector2 destinazione = puntiPattuglia[indicePunto].position;

        // MuoviVerso restituisce true quando raggiungiamo il punto.
        if (MuoviVerso(destinazione))
        {
            // Selezioniamo il punto successivo del percorso.
            indicePunto = (indicePunto + 1) % puntiPattuglia.Length;

            // Prima del prossimo movimento l'NPC farà una pausa.
            AvviaPausa();
        }
    }

    private void GestisciMovimentoCasuale()
    {
        // Una nuova destinazione viene generata soltanto
        // quando non ne abbiamo già una valida.
        if (!destinazioneValida)
        {
            ScegliDestinazioneCasuale();

            if (!destinazioneValida)
            {
                // Se non troviamo un punto raggiungibile,
                // aspettiamo un secondo prima di riprovare.
                tempoPausa = 1f;

                FermaNPC();
                return;
            }
        }

        if (MuoviVerso(destinazioneCasuale))
        {
            // La destinazione è stata raggiunta.
            // La invalidiamo per generarne una nuova
            // dopo la pausa.
            destinazioneValida = false;

            AvviaPausa();
        }
    }

    private void ScegliDestinazioneCasuale()
    {
        // Proviamo fino a 10 volte a trovare una destinazione
        // che non richieda di attraversare un ostacolo.
        for (int tentativo = 0; tentativo < 10; tentativo++)
        {
            // Random.insideUnitCircle genera un punto casuale
            // all'interno di un cerchio di raggio 1.
            //
            // Moltiplicandolo per raggioMovimento allarghiamo l'area.
            // Sommandolo a centroMovimento la posizioniamo
            // intorno alla posizione iniziale dell'NPC.
            Vector2 punto = centroMovimento +
                            Random.insideUnitCircle * raggioMovimento;

            // Calcoliamo il vettore che collega l'NPC al punto.
            Vector2 spostamento = punto - corpo.position;

            // magnitude restituisce la lunghezza del vettore,
            // cioè la distanza tra le due posizioni.
            // Scartiamo i punti troppo vicini.
            if (spostamento.magnitude < 0.05f)
            {
                continue;
            }

            // CircleCast simula lo spostamento di un cerchio
            // lungo una direzione, verificando eventuali collisioni.
            //
            // Parametri:
            // 1. Posizione iniziale del controllo.
            // 2. Raggio del cerchio.
            // 3. Direzione del controllo.
            // 4. Distanza massima da controllare.
            // 5. Layer degli ostacoli da rilevare.
            //
            // Restituisce un RaycastHit2D con le informazioni
            // sull'eventuale ostacolo incontrato.
            RaycastHit2D ostacolo = Physics2D.CircleCast(
                corpo.position,
                raggioControlloOstacoli,
                spostamento.normalized,
                spostamento.magnitude,
                ostacoli
            );

            // Se collider non è null, è stato rilevato un ostacolo.
            // continue interrompe questa iterazione del ciclo
            // e passa al tentativo successivo.
            if (ostacolo.collider != null)
            {
                continue;
            }

            // Il percorso diretto è libero dagli ostacoli controllati.
            // Memorizziamo la destinazione.
            destinazioneCasuale = punto;
            destinazioneValida = true;

            return;
        }

        // Dopo 10 tentativi non abbiamo trovato una destinazione.
        destinazioneValida = false;
    }

    private void GestisciMovimentoViandante()
    {
        if (puntiPasseggiata == null || puntiPasseggiata.Length == 0)
        {
            FermaNPC();
            return;
        }

        if (puntiPasseggiata[indicePunto] == null)
        {
            // L'operatore % (modulo) restituisce il resto della divisione.
            // Permette di tornare all'indice 0 dopo l'ultimo elemento.
            // Esempio con 3 punti: 0 -> 1 -> 2 -> 0.
            indicePunto = (indicePunto + 1) % puntiPasseggiata.Length;

            FermaNPC();
            return;
        }

        const int NR_SALTA_PUNTI = 7;

        // Recuperiamo la posizione del punto corrente.
        // Il Transform contiene le coordinate del GameObject.
        Vector2 destinazione = puntiPasseggiata[indicePunto].position;

        // MuoviVerso restituisce true quando raggiungiamo il punto.
        if (MuoviVerso(destinazione))
        {
            switch (indicePunto)
            {
                case < 2 or > 2:
                    // Selezioniamo il punto successivo del percorso.
                    indicePunto = (indicePunto + 1) % puntiPasseggiata.Length;

                    if (indicePunto == 10)
                    {
                        indicePunto = 12;
                    }

                    break;
                case 2:
                    if (Random.Range(0, 2) == 0)
                    {
                        //Primo percorso a sinistra: Punto_4
                        indicePunto = (indicePunto + 1) % puntiPasseggiata.Length;
                    }
                    else
                    {
                        //Primo percorso a destra
                        indicePunto = (indicePunto + 1) % puntiPasseggiata.Length + NR_SALTA_PUNTI;
                    }

                    // Prima del prossimo movimento l'NPC farà una pausa.
                    AvviaPausa();
                    break;
            }
        }
    }

    private bool MuoviVerso(Vector2 destinazione)
    {
        // Sottraendo due posizioni otteniamo il vettore spostamento.
        Vector2 spostamento = destinazione - corpo.position;

        // La lunghezza dello spostamento è la distanza da percorrere.
        float distanza = spostamento.magnitude;

        // Utilizziamo una tolleranza per stabilire se siamo arrivati.
        if (distanza <= distanzaArrivo)
        {
            FermaNPC();
            return true;
        }

        // Normalizziamo la direzione per mantenere una velocità
        // uniforme anche nei movimenti diagonali.
        Vector2 direzione = spostamento.normalized;

        // Mathf.Min restituisce il valore più piccolo tra due numeri.
        //
        // Utilizziamo la velocità normale quando siamo lontani.
        // Se siamo molto vicini alla destinazione, la riduciamo
        // per evitare di superare il punto nell'ultimo aggiornamento.
        float velocitaEffettiva = Mathf.Min(
            velocita,
            distanza / Time.fixedDeltaTime
        );

        // Impostiamo la velocità del Rigidbody2D
        // nella direzione della destinazione.
        corpo.linearVelocity = direzione * velocitaEffettiva;

        AggiornaAnimazione(direzione, true);

        // false indica che la destinazione non è ancora raggiunta.
        return false;
    }

    private void FermaNPC()
    {
        corpo.linearVelocity = Vector2.zero;

        AggiornaAnimazione(Vector2.zero, false);
    }

    private void AvviaPausa()
    {
        // Random.Range(float, float) genera un numero casuale
        // compreso tra i due valori indicati.
        // Qui determina quanti secondi resterà fermo l'NPC.
        tempoPausa = Random.Range(pausaMinima, pausaMassima);

        FermaNPC();
    }

    private void AggiornaAnimazione(Vector2 direzione, bool cammina)
    {
        // Aggiorniamo la direzione soltanto quando l'NPC cammina.
        // Quando si ferma, manteniamo l'ultima direzione salvata.
        if (cammina && direzione.sqrMagnitude > 0.001f)
        {
            direzioneGuardata = direzione.normalized;
        }

        animatore.SetFloat("DirezioneX", direzioneGuardata.x);
        animatore.SetFloat("DirezioneY", direzioneGuardata.y);
        animatore.SetBool("Cammina", cammina);
    }

    // OnDrawGizmosSelected viene chiamato dall'Editor di Unity
    // per disegnare elementi visivi quando il GameObject è selezionato.
    // Serve per il debug e non modifica il comportamento fisico.
    private void OnDrawGizmosSelected()
    {
        if (modalita != ModalitaNPC.Casuale)
        {
            return;
        }

        // Impostiamo il colore delle linee disegnate.
        Gizmos.color = Color.yellow;

        // Application.isPlaying verifica se il gioco è in esecuzione.
        //
        // Durante il gioco usiamo il centro salvato all'avvio.
        // Nell'Editor usiamo la posizione attuale del GameObject.
        //
        // L'operatore ternario ? : permette di scegliere
        // tra due valori sulla base di una condizione.
        Vector3 centro = Application.isPlaying
            ? new Vector3(
                centroMovimento.x,
                centroMovimento.y,
                transform.position.z
              )
            : transform.position;

        // Disegna una sfera composta da linee.
        // Vista dall'alto nella scena 2D, permette di visualizzare
        // l'area circolare entro cui l'NPC può scegliere i punti.
        Gizmos.DrawWireSphere(centro, raggioMovimento);
    }
}