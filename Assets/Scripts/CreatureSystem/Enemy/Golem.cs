﻿using AIActor_RC;
using CleverCrow.Fluid.BTs.Trees;
using CleverCrow.Fluid.BTs.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class Golem : AIActor
{
    // public Actor attackTarget = null;
    public Actor attackPlayerTarget = null;

    protected override void Start()
    {
        base.Start();

        InitAI();
    }

    protected override void Update()
    {
        base.Update();

        UpdateAttackTarget();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        brain.Tick();
    }

    void InitAI()
    {
        // Create behavior tree
        brain = new BehaviorTreeBuilder(gameObject)
            .Selector()
                .Sequence("Attack Branch")
                    .Condition("Have Target?", () => { return HaveAttackTarget(); })
                    .Selector("Try Attack")
                        .Sequence("Attack Process")
                            .Condition("In Attack Range?", () => { return isInAttackRange(attackPlayerTarget); })
                            .Do("Attack", () => // attack behavior Leaf
                            {
                                DoAttack(attackPlayerTarget);
                                return TaskStatus.Success;
                            })
                            .End()
                        .Do("Pursuit", () => // pursuit behavior Leaf
                        {
                            DoPursuit(attackPlayerTarget);
                            return TaskStatus.Success;
                        })
                        .End()
                    .End()
                .Do("Wander", () => // wander behavior Leaf
                {
                    DoWander();
                    return TaskStatus.Success;
                })
                .Build();
    }

    /// <summary>
    /// wander 
    /// </summary>
    void DoWander()
    {
        if (animator != null) { animator.SetBool("Attack", false); }
        Vector3 acceleration = wanderBehaviors.GetSteering();

        if (steeringBehaviors.IsArrived(wanderBehaviors.targetPosition))
        {
            acceleration = Vector3.zero;
        }

        if (collisionSensor != null)
        {
            Vector3 accelerationDir = acceleration.normalized;
            collisionSensor.GetCollisionFreeDirection(accelerationDir, out accelerationDir);
            accelerationDir *= acceleration.magnitude;
            acceleration = accelerationDir;
        }

        steeringBehaviors.Steer(acceleration);
        steeringBehaviors.LookMoveDirection();
    }

    /// <summary>
    /// Attack
    /// </summary>
    void DoAttack(Actor actor)
    {
        if (actor == null) return;

        if (animator != null) animator.SetBool("Attack", true);

        steeringBehaviors.Steer(Vector3.zero);
        steeringBehaviors.LookAtDirection(attackPlayerTarget.transform.position - transform.position);
    }

    /// <summary>
    /// Pursuit
    /// </summary>
    void DoPursuit(Actor actor)
    {
        if (actor == null) return;

        if (animator != null) animator.SetBool("Attack", false);

        Vector3 acceleration = pursueBehaviors.GetSteering(actor.GetRigidbody());

        if (collisionSensor != null)
        {
            Vector3 accelerationDir = acceleration.normalized;
            collisionSensor.GetCollisionFreeDirection(accelerationDir, out accelerationDir);
            accelerationDir *= acceleration.magnitude;
            acceleration = accelerationDir;
        }

        steeringBehaviors.Steer(acceleration);
        steeringBehaviors.LookMoveDirection();
    }

    /// <summary>
    /// Have attack target?
    /// </summary>
    bool HaveAttackTarget()
    {
        return attackPlayerTarget != null;
    }
    /// <summary>
    /// Is in attack range?
    /// </summary>
    bool isInAttackRange(Actor actor)
    {
        if (actor == null) return false;
        return Vector3.Distance(transform.position, actor.transform.position) < attackRadius;
    }

    /// <summary>
    /// Update attack target
    /// </summary>
    void UpdateAttackTarget()
    {
        if (attackPlayerTarget)
        {
            if (!isInAttackRange(attackPlayerTarget))
            {
                attackPlayerTarget = null;
            }
        }

        if (attackPlayerTarget == null)
        {
            // attackTarget = GetNearestAttackTargetInView();
            attackPlayerTarget = GetNearestAttackPlayerTargetInView();
        }
    }

    /// <summary>
    /// Get the nearest attack target in sight
    /// </summary>
    /*Actor GetNearestAttackTargetInView()
    {
        ActorTypeFilter filter = (actor) => actor is TestAIBehaviorChild;

        List<Actor> actors = GetActorsInView(filter);

        if (actors.Count == 0) return null;

        actors.Sort((actorA, actorB) =>
        {
            float distanceA = Vector3.Distance(transform.position, actorA.transform.position);
            float distanceB = Vector3.Distance(transform.position, actorB.transform.position);
            return distanceA.CompareTo(distanceB);
        });

        return actors[0];
    }*/
    /// <summary>
    /// Get the nearest attack player target in sight
    /// </summary>
    Actor GetNearestAttackPlayerTargetInView()
    {
        ActorTypeFilter filter = (actor) => actor is CharacterActor;

        List<Actor> actors = GetActorsInView(filter);

        if (actors.Count == 0) return null;

        actors.Sort((actorA, actorB) =>
        {
            float distanceA = Vector3.Distance(transform.position, actorA.transform.position);
            float distanceB = Vector3.Distance(transform.position, actorB.transform.position);
            return distanceA.CompareTo(distanceB);
        });

        return actors[0];
    }
}