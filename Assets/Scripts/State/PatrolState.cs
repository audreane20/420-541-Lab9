using UnityEngine;
using System.Collections;

public class PatrolState : IState
{
    private AIController aiController;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private Coroutine waitCoroutine;

    public StateType Type => StateType.Patrol;

    public PatrolState(AIController aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        isWaiting = false;
        aiController.Agent.isStopped = false;
        MoveToNextWaypoint();
    }

    public void Execute()
    {
        if (aiController.CanSeePlayer())
        {
            aiController.StateMachine.TransitionToState(StateType.Chase);
            return;
        }

        if (aiController.Waypoints.Length == 0)
            return;

        if (!isWaiting && !aiController.Agent.pathPending &&
            aiController.Agent.remainingDistance <=
            aiController.Agent.stoppingDistance)
        {
            waitCoroutine = aiController.StartCoroutine(WaitAndAnimate());
        }
    }

    public void Exit()
    {
        if (waitCoroutine != null)
        {
            aiController.StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }

        isWaiting = false;
        aiController.Agent.isStopped = false;
    }

    private IEnumerator WaitAndAnimate()
    {
        isWaiting = true;
        aiController.Agent.isStopped = true;

        aiController.aiAnimationController.animator.SetTrigger("doScream");

        yield return new WaitForSeconds(5f);

        aiController.Agent.isStopped = false;
        MoveToNextWaypoint();
        isWaiting = false;
        waitCoroutine = null;
    }

    private void MoveToNextWaypoint()
    {
        if (aiController.Waypoints.Length == 0)
            return;

        aiController.Agent.destination =
            aiController.Waypoints[currentWaypointIndex].position;

        currentWaypointIndex =
            (currentWaypointIndex + 1) % aiController.Waypoints.Length;
    }
}