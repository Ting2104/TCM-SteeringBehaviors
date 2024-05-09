using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EvadeBehavior : Steering
{
    public Transform target;
    public override SteeringData GetSteering(SteeringBehaviorController steeringController)
    {
        if (target == null)
            return new SteeringData();

        else if (Vector2.Distance(transform.position, transform.position * 3) < Vector2.Distance(target.position, transform.position))
        {
            return new SteeringData()
            {
                linear = -((target.position - transform.position).normalized * steeringController.maxAcceleration),
                angular = 0.0f
            };
        }

        return new SteeringData()
        {
            linear = -((target.position - transform.position).normalized * steeringController.maxAcceleration),
            angular = 0.0f
        };
    }
}
