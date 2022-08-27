using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public float startWaitTime = 3;
    public float timeToRotate = 2;
    public float speedWalk = 6;
    public float speedRun = 9;

    public float viewRadius = 15;
    public float viewAngle = 120;

    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float meshResolution = 1f;

    public Transform[] waypoints;
    int CurrentWayPointIndex;

    Vector3 playerLastPosition = Vector3.zero;
    Vector3 PlayerPosition;

    float waitTime;
    float TimeToRotate;
    bool playerInRange;
    bool playerNear;
    bool isPatrol;
    bool caughtPlayer;

    private Animator anim;
    private PlayerAnimationController enemyAnimationController;
    public PlayerMovement playerMovement;
    private void Start()
    {
        PlayerPosition = Vector3.zero;
        isPatrol = true;
        caughtPlayer = false;
        playerInRange = false;
        waitTime = startWaitTime;
        TimeToRotate = timeToRotate;

        CurrentWayPointIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(waypoints[CurrentWayPointIndex].position);

        anim = GetComponentInChildren<Animator>();
        enemyAnimationController = new PlayerAnimationController(); // 
        enemyAnimationController.SetAnim(anim);
    }

    private void Update()
    {
        EnviromentView();

        if (!isPatrol)
        {
            Chasing();
        }
        else
        {
            Patroling();
        }
        //Debug.Log(playerInRange);
    }

    private void Patroling()
    {
        if (playerNear)
        {
            if(TimeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(playerLastPosition);
            }
            else
            {
                Stop();
                TimeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            playerNear = false;
            playerLastPosition = Vector3.zero;
            enemyAnimationController.SetMovementBlendTree(0.5f);
            navMeshAgent.SetDestination(waypoints[CurrentWayPointIndex].position);
            if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if(waitTime <= 0)
                {
                    NextPoint();
                    Move(speedWalk);
                    waitTime = startWaitTime;
                }
                else
                {
                    enemyAnimationController.SetMovementBlendTree(0f);
                    Stop();
                    waitTime -= Time.deltaTime;
                }
            }
        }
    }

    private void Chasing()
    {
        playerNear = false;
        playerLastPosition = Vector3.zero;
        enemyAnimationController.SetMovementBlendTree(1f);


        if (!caughtPlayer)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(PlayerPosition);
        }
        if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (waitTime <= 0 && !caughtPlayer && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {

                isPatrol = true;
                playerNear = false;
                Move(speedWalk);
                TimeToRotate = timeToRotate;
                waitTime = startWaitTime;
                navMeshAgent.SetDestination(waypoints[CurrentWayPointIndex].position);

            }
            else
            {
                if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 2.5f)
                {
                    Stop();
                    waitTime -= Time.deltaTime;
                }

            }
        }
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
        enemyAnimationController.SetMovementBlendTree(0f);

    }

    public void NextPoint()
    {
        CurrentWayPointIndex = (CurrentWayPointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[CurrentWayPointIndex].position);
    }
    void CaughtPlayer()
    {
        caughtPlayer = true;
    }

    void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(player);
        if (Vector3.Distance(transform.position, player) <= 0.3)
        {
            if (waitTime <= 0)
            {
                playerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(waypoints[CurrentWayPointIndex].position);
                waitTime = startWaitTime;
                TimeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                waitTime -= Time.deltaTime;
            }
        }
    }

    void EnviromentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    this.playerInRange = true;
                    isPatrol = false;
                }
                else
                {
                    this.playerInRange = false;
                }
            }
            if (Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                this.playerInRange = false;
                //burasý niye çalýþmýyor?
                if (playerMovement.isRunning)
                {
                    this.playerInRange = true;
                    isPatrol = false;
                }
                

            }
            if (this.playerInRange)
            {
                PlayerPosition = player.transform.position;
            }

        }
    }
}
