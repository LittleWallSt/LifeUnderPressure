using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidBehaviour : BoidUnit
{
    [Range(1, 6)][SerializeField] private float minUpTimer;
    [Range(1, 6)][SerializeField] private float maxUpTimer;
    [SerializeField] private float squidVel = 1.0f;

    private float upTimer = 0.0f;
    private float downTimer = 0.0f;

    private float squidTimer = 0.0f;

    private bool deceleration = false;
    private bool goingUp = true;

    private float maxHeight;
    private float minHeight;

    private Vector3 initialPosition; // Para almacenar la posición inicial

    private void Start()
    {
        averageSpeed = speed;

        // Guardamos la posición inicial del calamar
        initialPosition = transform.position;

        if (assignedBoid != null)
        {
            path = assignedBoid.path;
            currentWaypointIndex = assignedBoid.currWayPointIndex;
        }

        // Establecemos las alturas máximas y mínimas en función de los waypoints
        if (path.GetWaypoint(0).position.y > path.GetWaypoint(1).position.y)
        {
            maxHeight = path.GetWaypoint(0).position.y;
            minHeight = path.GetWaypoint(1).position.y;
        }
        else
        {
            maxHeight = path.GetWaypoint(1).position.y;
            minHeight = path.GetWaypoint(0).position.y;
        }

        Debug.Log("Max height: " + maxHeight + " Min height: " + minHeight);

        upTimer = UnityEngine.Random.Range(minUpTimer, maxUpTimer);
        downTimer = upTimer - UnityEngine.Random.Range(0.5f, 1.3f);
    }

    public override void MoveFish()
    {
        // Si no hay waypoints
        if (path == null || path.Length == 0) return;

        // Control de waypoint actual
        if (assignedBoid.currWayPointIndex > 2)
        {
            assignedBoid.SetNextWaypoint(0);
        }
        Transform targetWaypoint = path.GetWaypoint(assignedBoid.currWayPointIndex);
        directionToWaypoint = Vector3.zero;

        FindNeighbours();
        CalculateAverageSpeed();

        // Lógica de movimiento vertical
        squidTimer += Time.deltaTime;

        if (goingUp)
        {
            Debug.Log("Im going UP");

            // Velocidad de subida
            if (!deceleration)
            {
                squidVel = 1.1f;
                if (squidTimer > upTimer)
                {
                    squidTimer = 0.0f;
                    deceleration = true;
                    squidVel = 1.0f;
                }
            }
            else
            {
                squidVel = -0.5f; // Fase de desaceleración mientras sube
                if (squidTimer > downTimer)
                {
                    squidTimer = 0.0f;
                    squidVel = 1.0f;
                    deceleration = false;
                }
            }
        }
        else
        {
            // Movimiento hacia abajo
            squidVel = -0.7f; // Velocidad constante para bajar
            Debug.Log("Im going DOWN");
        }

        // Movimiento vertical hacia el waypoint (pero solo en el eje Y)
        directionToWaypoint = (targetWaypoint.position - transform.position).normalized;

        // El movimiento será únicamente en el eje Y
        Vector3 moveVector = new Vector3(0, squidVel, 0);

        // Mantenemos la posición original en X y Z
        Vector3 newPosition = new Vector3(initialPosition.x, transform.position.y, initialPosition.z);

        // Aplicamos el movimiento vertical
        newPosition += moveVector * Time.deltaTime;

        // Establecer la nueva posición manteniendo los valores X y Z originales
        transform.position = newPosition;

        // Control de altura para cambiar de dirección
        if (transform.position.y > maxHeight)
        {
            goingUp = false; // Cambiamos la dirección
            if (assignedBoid != null) assignedBoid.SetNextWaypoint();
        }
        else if (transform.position.y < minHeight)
        {
            goingUp = true; // Cambiamos la dirección
            if (assignedBoid != null) assignedBoid.SetNextWaypoint();
        }
    }
}
