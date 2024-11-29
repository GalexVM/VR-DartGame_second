using UnityEngine;

public class DartCollision : MonoBehaviour
{
    [Header("Configuraci�n del Detector")]
    [Tooltip("Tag del objeto con el que se desea detectar la colisi�n")]
    public string targetTag = "Target";

    private Rigidbody rb;

    void Start()
    {
        // Aseg�rate de que el objeto tenga un Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("El objeto no tiene un componente Rigidbody.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verifica si el objeto con el que colision� tiene el Tag espec�fico
        if (collision.gameObject.CompareTag(targetTag))
        {
            // Configura el Rigidbody como kinem�tico
            rb.isKinematic = true;

            // Haz que el dardo sea hijo del objeto objetivo
            transform.SetParent(collision.transform);

            // Opcional: Ajusta la posici�n o rotaci�n del dardo si es necesario
            Debug.Log($"El dardo se ha pegado al objetivo: {collision.gameObject.name}");
        }
    }
}
