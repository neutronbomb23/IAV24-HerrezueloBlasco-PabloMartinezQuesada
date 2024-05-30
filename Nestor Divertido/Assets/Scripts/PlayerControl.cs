using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Rigidbody myRigidbody;
    NavMeshAgent agent;
    public Vector3 desiredPositionByAI;
    public float rotationSpeed = 5f; // Velocidad de rotaci�n

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        desiredPositionByAI = this.transform.position;
    }

    void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
        MoveAgentTo(desiredPositionByAI);
    }

    public void LookAt(Transform target)
    {
        // Calcula la direcci�n hacia el objetivo
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Aseg�rate de que solo estamos rotando en el plano horizontal

        // Si la direcci�n es diferente de Vector3.zero, calcula la rotaci�n
        if (direction != Vector3.zero)
        {
            // Calcula la rotaci�n objetivo
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // Interpola la rotaci�n actual hacia la rotaci�n objetivo
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void MoveAgentTo(Vector3 position)
    {
        agent.destination = position;
    }

    public void MoveAgent()
    {
        agent.destination = desiredPositionByAI;
    }
}
