using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MovimentoNPC))]
public class InseguimentoNPC : MonoBehaviour
{
    [Header("Bersaglio")]
    [SerializeField] private Transform giocatore;

    [Header("Inseguimento")]
    [SerializeField] private float raggioRilevamento = 4f;
    [SerializeField] private float distanzaArresto = 0.8f;
    [SerializeField] private float velocita = 3f;

    private Rigidbody2D corpo;
    private Animator animatore;
    private MovimentoNPC movimentoBase;
    private Vector2 direzione;
    private bool staInseguendo;

    private void Awake()
    {
        corpo = GetComponent<Rigidbody2D>();
        animatore = GetComponent<Animator>();
        movimentoBase = GetComponent<MovimentoNPC>();
    }

    private void Update()
    {
        if (giocatore == null) return;

        float distanza = Vector2.Distance(corpo.position, giocatore.position);

        if (!staInseguendo && distanza <= raggioRilevamento)
        {
            staInseguendo = true;
            movimentoBase.enabled = false;
        }

        if (staInseguendo && distanza > raggioRilevamento)
        {
            FermaInseguimento();
            return;
        }

        if (!staInseguendo) return;

        if (distanza > distanzaArresto)
        {
            direzione = ((Vector2)giocatore.position - corpo.position).normalized;
        }
        else
        {
            direzione = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!staInseguendo) return;

        corpo.linearVelocity = direzione * velocita;
        if (direzione.sqrMagnitude > 0.1f)
        {
            animatore.SetFloat("DirezioneX", direzione.x);
            animatore.SetFloat("DirezioneY", direzione.y);
        }
        animatore.SetBool("Cammina", direzione.sqrMagnitude > 0.1f);
    }

    private void FermaInseguimento()
    {
        staInseguendo = false;
        direzione = Vector2.zero;
        corpo.linearVelocity = Vector2.zero;
        animatore.SetBool("Cammina", false);
        movimentoBase.enabled = true;
    }

    private void OnDisable()
    {
        if (staInseguendo)
        {
            FermaInseguimento();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, raggioRilevamento);

    }

}