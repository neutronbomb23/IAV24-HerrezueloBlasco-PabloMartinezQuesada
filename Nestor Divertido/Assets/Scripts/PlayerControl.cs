using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {
    Vector3 velocity;
    Rigidbody myRigidbody;
    NavMeshAgent agent;
    public Vector3 desiredPositionByAI;

    void Awake() {
        myRigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        desiredPositionByAI = this.transform.position;
    }

    void FixedUpdate() {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
        MoveAgentTo(desiredPositionByAI);
    }

    public void Move(Vector3 _velocity) {
        this.velocity = _velocity;
    }

    public void LookAt(Vector3 lookPoint) {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }

    public void MoveAgentTo(Vector3 position) {
        agent.destination = position;
    }

    public void MoveAgent() {
        agent.destination = desiredPositionByAI;
    }
}
