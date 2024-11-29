using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class DartManager : MonoBehaviour
{
    // Lista de objetos que se podrán asignar desde el Inspector
    public List<GameObject> unityObjects;

    // Puerto de escucha
    private int port = 5300;

    // Servidor TCP
    private TcpListener tcpListener;
    private TcpClient tcpClient;
    private NetworkStream networkStream;

    // Bandera para detener el servidor cuando se detenga el juego
    private bool isServerRunning = false;

    void Start()
    {
        // Iniciar el servidor en un hilo separado para no bloquear el hilo principal de Unity
        isServerRunning = true;
        StartServer();
    }

    void OnApplicationQuit()
    {
        // Detener el servidor cuando se cierre la aplicación
        StopServer();
    }

    void Update()
    {
        // Aquí puedes agregar cualquier lógica que dependa de los datos recibidos por TCP
    }

    private void StartServer()
    {
        // Obtener la IP de la máquina local
        string ipAddress = GetLocalIPAddress();
        Debug.Log("Servidor TCP iniciado en IP: " + ipAddress + " y puerto: " + port);

        // Iniciar un hilo para escuchar conexiones TCP
        System.Threading.Thread tcpThread = new System.Threading.Thread(new System.Threading.ThreadStart(ListenForClients));
        tcpThread.IsBackground = true;
        tcpThread.Start();
    }

    private void ListenForClients()
    {
        try
        {
            // Usar la IP obtenida previamente
            string ipAddress = GetLocalIPAddress();
            tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
            tcpListener.Start();
            Debug.Log("Esperando clientes...");

            while (isServerRunning)
            {
                tcpClient = tcpListener.AcceptTcpClient();
                networkStream = tcpClient.GetStream();
                Debug.Log("Cliente conectado.");

                byte[] buffer = new byte[1024];
                int bytesRead;

                // Leer los datos que se envían al servidor
                while ((bytesRead = networkStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Debug.Log("Recibido: " + receivedData);

                    ProcessReceivedData(receivedData);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error en el servidor TCP: " + ex.Message);
        }
    }

    private void StopServer()
    {
        isServerRunning = false;
        if (tcpListener != null)
        {
            tcpListener.Stop();
        }
        if (tcpClient != null)
        {
            tcpClient.Close();
        }
    }

    private int currentObjectIndex = 0; // Índice actual de objetos en la lista

    private void ProcessReceivedData(string data)
    {
        string id = "";
        string pos = "";
        string lin = "";
        string ang = "";

        int ini = -1;
        int fin = -1;
        int numberId;


        //ID
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == 'I')
            {
                ini = i + 4;
            }
            else if (data[i] == 'P')
            {
                fin = i - 4;
            }
        }
        id = data.Substring(ini, fin - ini + 1);


        // Extraer posición (P)
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == 'P')
            {
                ini = i + 11;
            }
            else if (data[i] == 'L')
            {
                fin = i - 4;
            }
        }
        pos = data.Substring(ini, fin - ini + 1);

        // Extraer velocidad lineal (L)
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == 'L')
            {
                ini = i + 17;
            }
            else if (data[i] == 'A')
            {
                fin = i - 4;
            }
        }
        lin = data.Substring(ini, fin - ini + 1);

        // Extraer velocidad angular (A)
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == 'A')
            {
                ini = i + 18;
            }
        }
        fin = data.Length - 2;
        ang = data.Substring(ini, fin - ini + 1);

        // Convertir las cadenas extraídas a vectores
        // Convertir las cadenas extraídas a vectores
        Vector3 posicion = StringToVector3(pos);
        Vector3 Lvelocity = StringToVector3(lin);


        //Vector3 Lvelocity = new Vector3(0.00f, 6.00f, 6.00f);

        Vector3 Avelocity = StringToVector3(ang);

        Debug.Log($"ID actual: {id}");

        numberId = id[id.Length - 1] - '0';

        Debug.Log($"ID actual: {id}");

        Debug.Log($"ID num: {numberId}");        




        if (unityObjects != null && unityObjects.Count > currentObjectIndex)
        {
            var currentObject = unityObjects[numberId];
            //var currentObject = unityObjects[currentObjectIndex];

            if (currentObject != null)
            {
                // Despachar la acción para que se ejecute en el hilo principal
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    // Mueve el objeto a la posición del vector 'posicion'
                    currentObject.transform.position = posicion;

                    // Asegurarse de que el objeto tiene un Rigidbody para aplicar las físicas
                    Rigidbody rb = currentObject.GetComponent<Rigidbody>();

                    if (rb != null)
                    {
                        // Aplica la velocidad lineal al Rigidbody para simular el movimiento
                        //rb.velocity = Lvelocity;

                        // Aplica la velocidad angular al Rigidbody para simular la rotación
                        //rb.angularVelocity = Avelocity;

                        rb.AddForce(Lvelocity, ForceMode.VelocityChange);
                        rb.AddTorque(Avelocity, ForceMode.VelocityChange);
                        Debug.Log($"Impulse applied to {id}: LinearVelocity = {Lvelocity}, AngularVelocity = {Avelocity}");
                    }
                });
            }

            // Incrementar el índice para el próximo objeto
            //currentObjectIndex++;
        }



    }

    private void ApplyImpulseToObject(string objectName, Vector3 linearVelocity, Vector3 angularVelocity)
    {
        // Buscar el objeto en la escena por su nombre
        GameObject obj = GameObject.Find(objectName);

        if (obj == null)
        {
            Debug.LogWarning($"Object with name '{objectName}' not found.");
            return;
        }

        // Verificar si tiene un Rigidbody
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"Object '{objectName}' does not have a Rigidbody. Cannot apply impulse.");
            return;
        }

        // Aplicar la fuerza (impulso)
        rb.AddForce(linearVelocity, ForceMode.VelocityChange);
        rb.AddTorque(angularVelocity, ForceMode.VelocityChange);

        Debug.Log($"Impulse applied to {objectName}: LinearVelocity = {linearVelocity}, AngularVelocity = {angularVelocity}");
    }





    Vector3 StringToVector3(string str)
    {
        // Separar los valores por la coma
        string[] values = str.Split(',');
        if (values.Length != 3)
        {
            Debug.LogError("El formato del string no es válido.");
            return Vector3.zero;
        }

        // Convertir los valores a flotantes y crear el Vector3
        return new Vector3(
            float.Parse(values[0].Trim()),
            float.Parse(values[1].Trim()),
            float.Parse(values[2].Trim())
        );
    }

    private string GetLocalIPAddress()
    {
        // Obtener la IP local de la máquina
        string localIP = "";
        foreach (var addr in Dns.GetHostAddresses(Dns.GetHostName()))
        {
            if (addr.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = addr.ToString();
                break;
            }
        }
        return localIP != "" ? localIP : "127.0.0.1"; // Si no se encuentra, retornar localhost
    }
}
