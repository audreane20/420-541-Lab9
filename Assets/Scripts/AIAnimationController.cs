using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class AIAnimationController : MonoBehaviour
{
    public Animator animator { get; private set; }

    private AIController aiController;
    private NavMeshAgent agent;
    private int attackLayer;
    private static readonly int AttackStateHash = Animator.StringToHash("Attack");
    private bool eventHitPending;
    private bool hitRegistered;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        aiController = GetComponent<AIController>();
        agent = GetComponent<NavMeshAgent>();
        attackLayer = animator.GetLayerIndex("UpperBody");
        if (attackLayer < 0)
            attackLayer = 0;
    }

    private void Update()
    {
        float speed = agent != null ? agent.velocity.magnitude : 0f;
        animator.SetFloat("CharacterSpeed", speed);
    }

    private void LateUpdate()
    {
        // Read the hands after the Animator has applied this frame's pose.
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(attackLayer);
        bool isAttacking = state.shortNameHash == AttackStateHash;

        if (!isAttacking && !eventHitPending)
        {
            hitRegistered = false;
            return;
        }

        // Check throughout the striking part of the swing, not just one event frame.
        bool inHitWindow = isAttacking &&
            state.normalizedTime >= 0.2f && state.normalizedTime <= 0.85f;
        bool shouldCheckHit = inHitWindow || eventHitPending;
        eventHitPending = false;

        if (!shouldCheckHit || hitRegistered || aiController == null)
            return;

        // The player can move with a CharacterController between physics steps.
        Physics.SyncTransforms();

        GameObject objectHit;
        if (aiController.CheckHandsCollision(out objectHit, "Player"))
        {
            hitRegistered = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void HitPlayer()
    {
        // Animation events fire during pose evaluation. Defer the query until LateUpdate.
        eventHitPending = true;
    }

    private void OnDisable()
    {
        eventHitPending = false;
        hitRegistered = false;
    }
}
