using UnityEngine;

public class ThrowableWithOVRGrabbable : OVRGrabbable
{
    private Rigidbody _rigidbody;

    // Sobrescribe el m�todo Start() de OVRGrabbable
    protected override void Start()
    {
        // Llamamos al m�todo Start() de la clase base para mantener su funcionalidad
        base.Start();

        // Obtener el Rigidbody del objeto
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Sobrescribe el m�todo GrabEnd() para aplicar la velocidad al soltar el objeto
    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        // Llamar a la implementaci�n original de GrabEnd() para liberar el objeto
        base.GrabEnd(linearVelocity, angularVelocity);

        // Aplicar la velocidad lineal y angular proporcionadas al soltar el objeto
        if (_rigidbody != null)
        {
            _rigidbody.velocity = linearVelocity;  // Aplicar la velocidad lineal de la mano
            _rigidbody.angularVelocity = angularVelocity;  // Aplicar la velocidad angular de la mano
        }
    }
}
