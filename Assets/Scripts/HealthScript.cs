using UnityEngine;

public class HealthScript : IResetable
{
    // Start is called before the first frame update
    [SerializeField] float health = 30.0f;
    [SerializeField] float resetTime = 20.0f;

    private bool isActive = true;
    private BoxCollider bc;
    private SpriteRenderer sr;
    private Timer timer;

    private void Awake()
    {
        bc = GetComponent<BoxCollider>();
        sr = GetComponentInChildren<SpriteRenderer>();
        timer = gameObject.AddComponent<Timer>();
    }


    private void OnTriggerEnter(Collider other)
    {
        PlayerController controller = other.gameObject.GetComponent<PlayerController>();
        if (controller != null) {
            controller.Heal(health);
            isActive = false;
            timer.SetTimer(resetTime, Reset);
            CheckActive();
        }
    }

    public override void Reset()
    {
        timer.StopTimer();
        isActive = true;
        CheckActive();
    }


    private void CheckActive()
    {
        bc.enabled = isActive;
        sr.enabled = isActive;
    }
}
