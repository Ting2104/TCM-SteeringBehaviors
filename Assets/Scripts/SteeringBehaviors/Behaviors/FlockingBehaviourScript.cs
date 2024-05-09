using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FlockingBehaviourScript : Steering
{
    public GameObject player;
    public GameObject flock;
    private Transform[] targets;
    private float alignDistance = 8f;
    [SerializeField]
    private float threshold = 2f;
    [SerializeField]
    private float decayCoefficient = -25f;
    void Start()
    {
        SteeringBehaviorController[] agents = FindObjectsOfType<SteeringBehaviorController>();
        targets = new Transform[agents.Length - 1];
        int count = 0;

        foreach (SteeringBehaviorController agent in agents)
        {
            if (agent.gameObject != gameObject)
            {
                targets[count++] = agent.transform;
            }
        }
    }

    public override SteeringData GetSteering(SteeringBehaviorController steeringController)
    {
        SteeringData steeringAlig = new SteeringData();
        SteeringData steeringCoh = new SteeringData();
        SteeringData steeringSep = new SteeringData();
        steeringAlig.linear = Vector3.zero;
        steeringCoh.linear = Vector3.zero;
        steeringSep.linear = Vector3.zero;
        Vector3 center = Vector3.zero;
        int countAlig = 0;
        int countCoh = 0;
        Vector2 targetDir = Vector2.zero;
        Vector2 direction = new Vector2();
        float distance = 0;

        foreach (Transform target in targets)
        {
            targetDir = target.position - transform.position;
            distance = targetDir.magnitude;

            steeringAlig.linear += target.GetComponent<Rigidbody2D>().velocity;
            countAlig++;
            direction = target.transform.position - transform.position;

            center.x += target.position.x;
            center.y += target.position.y;
            countCoh++;

            float strength = Mathf.Min(decayCoefficient / (distance * distance), steeringController.maxAcceleration);
            targetDir.Normalize();
            steeringSep.linear += strength * targetDir;
        }

        for(int i = 0; i < targets.Length; i++)
        {
            if ((targets[i].position.x + alignDistance > targets[i + 1].position.x) && (targets[i].position.y + alignDistance > targets[i + 1].position.y) && 
                (targets[i].position.z + alignDistance > targets[i + 1].position.z))
            {
                steeringAlig.linear /= countAlig;
                if (steeringAlig.linear.magnitude > steeringController.maxAcceleration)
                {
                    steeringAlig.linear = steeringAlig.linear.normalized * steeringController.maxAcceleration;
                }
                return steeringAlig;
            }
            else if ((targets[i].position.x > targets[i + 1].position.x) && (targets[i].position.y > targets[i + 1].position.y) &&
                (targets[i].position.z > targets[i + 1].position.z))
            {
                center.x /= countCoh;
                center.y /= countCoh;
                center = new Vector2(center.x, center.y);
                steeringCoh.linear = (center - transform.position).normalized * steeringController.maxAcceleration;

                return steeringCoh;
            }
            else
            {
                if (distance < threshold)
                {
                    float strength = Mathf.Min(decayCoefficient / (distance * distance), steeringController.maxAcceleration);
                    direction.Normalize();
                    steeringSep.linear += strength * direction;
                }
            }
        }

        return new SteeringData()
        {
            linear = (player.transform.position - transform.position).normalized * steeringController.maxAcceleration,
            angular = 0.0f
        };
    }
}