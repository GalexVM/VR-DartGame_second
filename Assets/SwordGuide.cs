using UnityEngine;

public class SwordGuide : MonoBehaviour
{
    public Transform target;          // El objetivo hacia el que la espada debe apuntar
    public Transform cameraTransform; // La referencia a la c�mara
    public Vector3 offset;            // Offset para ajustar la posici�n relativa a la c�mara

    private void LateUpdate()  // Usamos LateUpdate para que el movimiento sea m�s fluido
    {
        // 1. Mover la espada para que est� siempre frente a la c�mara
        if (cameraTransform != null)
        {
            // Posicionamos la espada frente a la c�mara con un offset opcional
            transform.position = cameraTransform.position + cameraTransform.forward * offset.z
                                + cameraTransform.right * offset.x
                                + cameraTransform.up * offset.y;

            // Alineamos la espada con la rotaci�n de la c�mara
            transform.rotation = cameraTransform.rotation;
        }

        // 2. Apuntar la espada hacia el objetivo con los ejes corregidos (si hay un objetivo)
        if (target != null)
        {
            // Apuntar hacia el objetivo
            Vector3 directionToTarget = target.position - transform.position;

            // Ajuste manual de los ejes invertidos de la espada
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(
                directionToTarget.z,  // El eje z de la espada se convierte en x
                directionToTarget.x,  // El eje x se convierte en y
                directionToTarget.y   // El eje y se convierte en z
            ));

            // Aplica la rotaci�n de forma gradual para que sea m�s suave
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
