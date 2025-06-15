using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class AgentMovement : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _animator.applyRootMotion = true;
    }

    void Update()
    {
        //if (_animator.GetCurrentAnimatorStateInfo(0).speed != 0.4f)
        //{
        //    return;
        //}
        Vector3 worldDeltaPosition = _agent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        //bool shouldMove = velocity.magnitude > 0.5f && _agent.remainingDistance > _agent.radius;

        // Update animation parameters
        //_animator.SetBool("move", shouldMove);
        _animator.SetFloat("velx", velocity.x);
        _animator.SetFloat("vely", velocity.y);

        //if (worldDeltaPosition.magnitude > _agent.radius)
        //    transform.position = _agent.nextPosition - 0.9f * worldDeltaPosition;

        //transform.LookAt(_agent.steeringTarget + transform.forward);//.lookAtTargetPosition = _agent.steeringTarget + transform.forward;
    }

    void OnAnimatorMove()
    {

        // Update position to agent position
        //Vector3 position = new Vector3(_agent.nextPosition.x, _agent.nextPosition.y - _animator.rootPosition.y, _agent.nextPosition.z);
        //Vector3 position = new Vector3(_agent.nextPosition.x, -_animator.rootPosition.y, _agent.nextPosition.z);
        transform.position = _agent.nextPosition;

        //if (_animator.GetCurrentAnimatorStateInfo(0).speed == 0.4f)
        //{
        //    //Vector3 position = _animator.rootPosition - _animator.transform.position;
        //    //Debug.Log("Pos: " + position.x + ", "  +  position.y + ", " + position.z);
        //    //position.y = _agent.nextPosition.y;
        //    //transform.position = _animator.rootPosition;
        //    //transform.position = position;
        //    //transform.position = (_agent.nextPosition + position);
        //    //transform.LookAt(_agent.steeringTarget + transform.forward);
        //}
    }
}
