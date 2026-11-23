//using UnityEngine; // Libreria principale di Unity
//using UnityEngine.InputSystem; // Libreria per il nuovo sistema di input

//// RequireComponent indica quali componenti devono essere presenti
//// sul GameObject che utilizza questo script.
////
//// Se uno di questi componenti manca, Unity lo aggiunge
//// automaticamente quando associamo lo script al GameObject.
//[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(SpriteRenderer))]
//public class MovimentoGiocatore : MonoBehaviour
//{
//    // SerializeField permette di modificare una variabile privata
//    // direttamente dall'Inspector di Unity.
//    //
//    // La velocità iniziale del personaggio è 5 unità al secondo.
//    [SerializeField] private float velocita = 5f;

//    // Rigidbody2D gestisce il movimento fisico del personaggio.
//    // Attraverso questo componente possiamo modificare la sua velocità
//    // e interagire con il sistema delle collisioni di Unity.
//    private Rigidbody2D corpo;

//    // Animator gestisce le animazioni del personaggio.
//    //
//    // Attraverso i suoi parametri possiamo decidere
//    // quando passare dall'animazione Idle a Run.
//    private Animator animatore;

//    // SpriteRenderer è il componente che visualizza
//    // lo sprite del personaggio nella scena.
//    //
//    // Lo utilizzeremo anche per capovolgere orizzontalmente
//    // il personaggio quando si muove verso sinistra.
//    private SpriteRenderer spriteRenderer;

//    // InputAction rappresenta un'azione configurata
//    // attraverso il nuovo Input System.
//    //
//    // L'azione "Move" può essere associata a WASD,
//    // frecce direzionali oppure a un controller.
//    private InputAction azioneMovimento;

//    // Vector2 è una struttura che contiene due valori: X e Y.
//    //
//    // Nel nostro caso rappresenta la direzione del movimento:
//    //
//    // (0, 0)  = personaggio fermo
//    // (1, 0)  = destra
//    // (-1, 0) = sinistra
//    // (0, 1)  = alto
//    // (0, -1) = basso
//    //
//    // Possiamo combinare X e Y per muoverci in diagonale.
//    private Vector2 direzione;


//    // -------------------------------------------------------
//    // AWAKE - INIZIALIZZAZIONE DEI COMPONENTI
//    // -------------------------------------------------------

//    // Awake viene chiamato durante l'inizializzazione
//    // del componente, prima del suo primo Update.
//    //
//    // Lo utilizziamo per recuperare i riferimenti
//    // necessari al funzionamento del personaggio.
//    private void Awake()
//    {
//        // GetComponent<T>() cerca un componente del tipo T
//        // sullo stesso GameObject che contiene lo script.
//        //
//        // Recuperiamo il Rigidbody2D del Player.
//        corpo = GetComponent<Rigidbody2D>();

//        // Recuperiamo il componente Animator.
//        //
//        // Questo riferimento ci permetterà di comunicare
//        // con l'Animator Controller e modificare i suoi parametri.
//        animatore = GetComponent<Animator>();

//        // Recuperiamo il componente SpriteRenderer.
//        //
//        // Ci servirà per cambiare l'orientamento
//        // grafico del personaggio.
//        spriteRenderer = GetComponent<SpriteRenderer>();

//        // InputSystem.actions permette di accedere
//        // alle Project-Wide Actions configurate in Unity.
//        //
//        // FindAction cerca l'azione "Move"
//        // all'interno della Action Map "Player".
//        //
//        // Il parametro true indica che deve essere
//        // generato un errore se l'azione non viene trovata.
//        azioneMovimento = InputSystem.actions.FindAction("Player/Move", true);
//    }


//    // -------------------------------------------------------
//    // UPDATE - INPUT E ANIMAZIONI
//    // -------------------------------------------------------

//    // Update viene eseguito una volta per ogni frame.
//    //
//    // Lo utilizziamo per:
//    // 1. Leggere gli input del giocatore.
//    // 2. Aggiornare i parametri dell'Animator.
//    // 3. Capovolgere lo sprite in base alla direzione.
//    private void Update()
//    {
//        // ---------------------------------------------------
//        // 1. LETTURA DEL MOVIMENTO
//        // ---------------------------------------------------

//        // ReadValue<Vector2>() legge il valore corrente
//        // dell'azione Move dal nuovo Input System.
//        //
//        // Esempi:
//        //
//        // Premiamo D -> direzione = (1, 0)
//        // Premiamo A -> direzione = (-1, 0)
//        // Premiamo W -> direzione = (0, 1)
//        // Non premiamo nulla -> direzione = (0, 0)
//        //
//        // Qui leggiamo soltanto la direzione.
//        // Il movimento fisico verrà applicato in FixedUpdate.
//        direzione = azioneMovimento.ReadValue<Vector2>();


//        // ---------------------------------------------------
//        // 2. AGGIORNAMENTO DEI PARAMETRI ANIMATOR
//        // ---------------------------------------------------

//        // SetFloat() modifica un parametro di tipo Float
//        // presente nell'Animator Controller.
//        //
//        // Il primo argomento è il nome del parametro.
//        // Il secondo argomento è il valore da assegnare.
//        //
//        // MoveX rappresenta il movimento orizzontale:
//        //
//        // -1 = sinistra
//        //  0 = nessun movimento orizzontale
//        //  1 = destra
//        animatore.SetFloat("MoveX", direzione.x);

//        // MoveY rappresenta il movimento verticale:
//        //
//        // -1 = basso
//        //  0 = nessun movimento verticale
//        //  1 = alto
//        animatore.SetFloat("MoveY", direzione.y);


//        // ---------------------------------------------------
//        // 3. CONTROLLO DELLO STATO DI MOVIMENTO
//        // ---------------------------------------------------

//        // Vogliamo sapere se il giocatore è fermo
//        // oppure si sta muovendo.
//        //
//        // sqrMagnitude restituisce la lunghezza
//        // del vettore elevata al quadrato.
//        //
//        // Formula:
//        // sqrMagnitude = (X * X) + (Y * Y)
//        //
//        // Esempi:
//        //
//        // direzione = (0, 0)
//        // sqrMagnitude = 0
//        //
//        // direzione = (1, 0)
//        // sqrMagnitude = 1
//        //
//        // direzione = (0, -1)
//        // sqrMagnitude = 1
//        //
//        // Usiamo una piccola soglia di 0.001
//        // per ignorare movimenti praticamente nulli,
//        // ad esempio piccoli input analogici.
//        //
//        // L'espressione restituisce un booleano:
//        //
//        // true  = il personaggio si sta muovendo
//        // false = il personaggio è fermo
//        bool inMovimento = direzione.sqrMagnitude > 0.001f;

//        // SetBool() modifica un parametro booleano
//        // presente nell'Animator Controller.
//        //
//        // Il parametro "isMoving" controlla
//        // il passaggio tra gli stati Idle e Run.
//        //
//        // true  -> animazione Run
//        // false -> animazione Idle
//        //
//        // Il nome deve corrispondere esattamente
//        // a quello configurato nell'Animator.
//        // Unity distingue maiuscole e minuscole.
//        animatore.SetBool("isMoving", inMovimento);


//        // ---------------------------------------------------
//        // 4. ORIENTAMENTO DELLO SPRITE
//        // ---------------------------------------------------

//        // SpriteRenderer possiede una proprietà chiamata flipX.
//        //
//        // flipX = true:
//        // lo sprite viene capovolto orizzontalmente.
//        //
//        // flipX = false:
//        // lo sprite mantiene l'orientamento originale.
//        //
//        // Supponiamo che lo sprite originale guardi a destra.

//        // Se X è negativo, il giocatore si muove a sinistra.
//        if (direzione.x < 0)
//        {
//            // Capovolgiamo lo sprite sull'asse orizzontale.
//            spriteRenderer.flipX = true;
//        }
//        // Se X è positivo, il giocatore si muove a destra.
//        else if (direzione.x > 0)
//        {
//            // Ripristiniamo l'orientamento originale.
//            spriteRenderer.flipX = false;
//        }

//        // Perché non utilizziamo un else finale?
//        //
//        // Quando direzione.x è uguale a 0,
//        // il giocatore è fermo orizzontalmente
//        // oppure si sta muovendo solo in verticale.
//        //
//        // In questo caso non modifichiamo flipX.
//        //
//        // Il personaggio continuerà a guardare
//        // nell'ultima direzione orizzontale utilizzata.
//        //
//        // Inoltre flipX modifica soltanto la grafica:
//        // non cambia la scala del GameObject
//        // e non capovolge i suoi Collider.
//    }


//    // -------------------------------------------------------
//    // FIXEDUPDATE - MOVIMENTO FISICO
//    // -------------------------------------------------------

//    // FixedUpdate viene eseguito a intervalli di tempo fissi.
//    //
//    // Per impostazione predefinita, generalmente ogni 0.02 secondi,
//    // cioè 50 aggiornamenti fisici al secondo.
//    //
//    // È utilizzato per interagire con il sistema fisico.
//    private void FixedUpdate()
//    {
//        // linearVelocity è una proprietà del Rigidbody2D.
//        //
//        // Rappresenta la velocità lineare del corpo
//        // sugli assi X e Y, espressa in unità al secondo.
//        //
//        // Per calcolare la velocità effettiva moltiplichiamo
//        // il vettore direzione per la variabile velocita.
//        //
//        // ESEMPIO 1 - Movimento verso destra:
//        //
//        // direzione = (1, 0)
//        // velocita = 5
//        //
//        // (1, 0) * 5 = (5, 0)
//        //
//        // Il personaggio si muove a destra.
//        //
//        // ESEMPIO 2 - Movimento verso sinistra:
//        //
//        // direzione = (-1, 0)
//        // velocita = 5
//        //
//        // (-1, 0) * 5 = (-5, 0)
//        //
//        // Il personaggio si muove a sinistra.
//        //
//        // ESEMPIO 3 - Personaggio fermo:
//        //
//        // direzione = (0, 0)
//        // velocita = 5
//        //
//        // (0, 0) * 5 = (0, 0)
//        //
//        // La velocità diventa zero
//        // e il personaggio si ferma.
//        //
//        // Non moltiplichiamo per Time.deltaTime:
//        // linearVelocity rappresenta già una velocità
//        // espressa in unità al secondo.
//        corpo.linearVelocity = direzione * velocita;
//    }
//}

using UnityEngine; // Libreria principale di Unity.
using UnityEngine.InputSystem; // Libreria del nuovo Input System.

// RequireComponent obbliga il GameObject ad avere
// un Rigidbody2D e un Animator.
// Se mancano, Unity li aggiunge automaticamente.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class MovimentoGiocatore : MonoBehaviour
{
    // Velocità di movimento del personaggio.
    // SerializeField permette di modificarla dall'Inspector.
    [SerializeField] private float velocita = 5f;

    // Riferimento al Rigidbody2D del personaggio.
    // Viene utilizzato per applicare il movimento fisico.
    private Rigidbody2D corpo;

    // Riferimento all'Animator.
    // Gestisce le animazioni Idle e Walk attraverso i Blend Tree.
    private Animator animatore;

    // Azione Move configurata nel nuovo Input System.
    // Permette di leggere WASD, frecce direzionali o controller.
    private InputAction azioneMovimento;

    // Vector2 contiene due coordinate: X e Y.
    //
    // (0, 0)   = fermo
    // (0, 1)   = alto
    // (0, -1)  = basso
    // (-1, 0)  = sinistra
    // (1, 0)   = destra
    //
    // Combinando X e Y possiamo muoverci anche in diagonale.
    private Vector2 direzione;


    // -------------------------------------------------------
    // AWAKE - INIZIALIZZAZIONE
    // -------------------------------------------------------

    // Awake viene eseguito durante l'inizializzazione
    // del componente, prima del suo primo Update.
    private void Awake()
    {
        // Recuperiamo il Rigidbody2D del GameObject.
        corpo = GetComponent<Rigidbody2D>();

        // Recuperiamo l'Animator del GameObject.
        animatore = GetComponent<Animator>();

        // Cerchiamo l'azione Move nella Action Map Player.
        // true genera un errore se l'azione non esiste.
        azioneMovimento = InputSystem.actions.FindAction("Player/Move", true);
    }


    // -------------------------------------------------------
    // UPDATE - INPUT E ANIMAZIONI
    // -------------------------------------------------------

    // Update viene eseguito una volta per ogni frame.
    // Lo utilizziamo per leggere gli input
    // e aggiornare i parametri dell'Animator.
    private void Update()
    {
        // Leggiamo la direzione dal nuovo Input System.
        // Il risultato è un Vector2 con coordinate X e Y.
        direzione = azioneMovimento.ReadValue<Vector2>();

        // Limitiamo la lunghezza massima del vettore a 1.
        //
        // Senza questa operazione, premendo due tasti
        // contemporaneamente (es. W + D), il personaggio
        // potrebbe muoversi più velocemente in diagonale.
        //
        // ClampMagnitude mantiene la direzione originale,
        // ma impedisce che la lunghezza superi 1.
        direzione = Vector2.ClampMagnitude(direzione, 1f);


        // ---------------------------------------------------
        // CONTROLLO DEL MOVIMENTO
        // ---------------------------------------------------

        // sqrMagnitude restituisce la lunghezza
        // del vettore elevata al quadrato.
        //
        // Formula: X * X + Y * Y
        //
        // Se il valore è praticamente zero,
        // significa che il personaggio è fermo.
        //
        // Utilizziamo una piccola soglia per ignorare
        // eventuali movimenti minimi di un controller.
        bool inMovimento = direzione.sqrMagnitude > 0.001f;


        // ---------------------------------------------------
        // AGGIORNAMENTO DEL BLEND TREE
        // ---------------------------------------------------

        // Aggiorniamo i parametri direzionali
        // soltanto quando il personaggio si muove.
        if (inMovimento)
        {
            // normalized restituisce un vettore
            // con lunghezza uguale a 1.
            //
            // Esempi:
            //
            // Destra:       (1, 0)
            // Sinistra:     (-1, 0)
            // Alto:         (0, 1)
            // Basso:        (0, -1)
            // Alto-destra:  (0.707, 0.707)
            //
            // Questo permette di mantenere una direzione
            // coerente per il Blend Tree.
            Vector2 orientamento = direzione.normalized;

            // MoveX controlla l'orientamento orizzontale.
            //
            // -1 = sinistra
            //  0 = nessuna componente orizzontale
            //  1 = destra
            animatore.SetFloat("MoveX", orientamento.x);

            // MoveY controlla l'orientamento verticale.
            //
            // -1 = basso
            //  0 = nessuna componente verticale
            //  1 = alto
            animatore.SetFloat("MoveY", orientamento.y);
        }

        // Quando il personaggio si ferma,
        // MoveX e MoveY non vengono aggiornati.
        //
        // In questo modo il personaggio mantiene
        // l'ultima direzione verso cui stava guardando.


        // ---------------------------------------------------
        // PASSAGGIO TRA IDLE E WALK
        // ---------------------------------------------------

        // isMoving è un parametro Bool dell'Animator.
        //
        // true  = il personaggio si muove -> Walk
        // false = il personaggio è fermo  -> Idle
        //
        // Le transizioni tra gli stati vengono gestite
        // automaticamente dall'Animator Controller.
        animatore.SetBool("isMoving", inMovimento);
    }


    // -------------------------------------------------------
    // FIXEDUPDATE - MOVIMENTO FISICO
    // -------------------------------------------------------

    // FixedUpdate viene eseguito a intervalli di tempo fissi.
    // È utilizzato per aggiornare il sistema fisico di Unity.
    private void FixedUpdate()
    {
        // linearVelocity rappresenta la velocità
        // del Rigidbody2D sugli assi X e Y.
        //
        // Moltiplichiamo la direzione per la velocità.
        //
        // Esempio:
        //
        // direzione = (1, 0)
        // velocita = 5
        //
        // Risultato = (5, 0)
        // Il personaggio si muove verso destra.
        //
        // Non utilizziamo Time.deltaTime perché
        // linearVelocity è già espressa in unità al secondo.
        corpo.linearVelocity = direzione * velocita;
    }
}