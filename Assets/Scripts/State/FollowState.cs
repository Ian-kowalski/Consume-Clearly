using UnityEngine;
using UnityEngine.AI;

namespace State
{
    public class FollowState : MonoBehaviour
    {
        private Transform target;
    
        private NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
    
        void Update()
        {
            agent.SetDestination(target.position);
        }
    }
}
