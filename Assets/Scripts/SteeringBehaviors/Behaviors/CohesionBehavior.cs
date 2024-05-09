using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CohesionBehavior : Steering
{
    private Transform[] targets;

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
        SteeringData steering = new SteeringData();
        Vector3 center = Vector2.zero;
        int count = 0;

        //Calcula el centro de las abejas
        foreach (Transform target in targets)
        {
            Vector2 direction = target.transform.position - transform.position;
            center.x += target.position.x;
            center.y += target.position.y;
            count++;
        }
        center.x /= count;
        center.y /= count;
        center = new Vector2(center.x, center.y);

        //Van hacía al centro
        return new SteeringData()
        {
            linear = (center - transform.position).normalized * steeringController.maxAcceleration,
            angular = 0.0f
        };
    }
}
