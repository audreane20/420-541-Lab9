using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class AIAnimationController : MonoBehaviour
{
    public Animator animator { get; private set; }

    private AIController aiController;
    private NavMeshAgent agent;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        aiController = GetComponent<AIController>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        float speed = agent != null ? agent.velocity.magnitude : 0f;
        animator.SetFloat("CharacterSpeed", speed);
    }

    private void HitPlayer()
    {
        GameObject objectHit;

        if (aiController.CheckHandsCollision(out objectHit, "Player"))
        {
            // Enable this after setting up the animations:
            SceneManager.LoadScene(0);
        }
    }
}
