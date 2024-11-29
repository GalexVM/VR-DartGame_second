using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net.Sockets;
using System.Text;
using System.Net; // Para obtener la IP

public class SensorDataReceiver : MonoBehaviour
{
    public TextMeshProUGUI data_text;
    public TMP_InputField inputField; // Campo para introducir texto

    // Lista de luces que se moverán
    public List<Light> directionalLights;
    public List<GameObject> cylinders;
    public GameObject oculusInteractionSamplerRig;

    // Variables para controlar el movimiento
    private int currentLightIndex = 0;
    private int currentCylinderIndex = 0;
    private bool controllingLights = true; // Controlar luces o cilindros
    private float moveAmount = 1f; // Movimiento más lento
    private float rotationAmount = 45f; // Rotación más lenta

    private string serverAddress; // Dirección IP del servidor
    private int serverPort = 5001; // Puerto del servidor



    private void Start()
    {
        serverAddress = "192.168.3.139";
        Debug.Log($"Dirección IP detectada automáticamente: {serverAddress}");
    }

    private string GetLocalIPv4()
    {
        string localIP = "127.0.0.1"; // Valor predeterminado en caso de fallo
        try
        {
            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) // Filtra IPv4
                {
                    localIP = ip.ToString();
                    break;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al obtener la IP local: {e.Message}");
        }
        return localIP;
    }




    private void Update()
    {
        // Cambiar entre controlar luces y cilindros
        if (Input.GetKeyDown(KeyCode.U))
        {
            SendKeyPress("U");
            controllingLights = !controllingLights;
            Debug.Log(controllingLights ? "Controlando luces" : "Controlando cilindros");

            // Teletransportar el primer objeto de la nueva lista activa a (0, 3, 0)
            if (controllingLights)
            {
                TeleportFirstObject(directionalLights);
                currentLightIndex = 0; // Reinicia el índice
            }
            else
            {
                TeleportFirstObject(cylinders);
                currentCylinderIndex = 0; // Reinicia el índice
            }
        }

        // Control de movimiento
        if (controllingLights)
        {
            HandleMovement(directionalLights, ref currentLightIndex);
        }
        else
        {
            HandleMovement(cylinders, ref currentCylinderIndex);
        }

        // Enviar texto con la tecla Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string userInput = inputField.text; // Obtener el texto ingresado
            ProcessText(userInput); // Procesar o enviar el texto
            inputField.text = ""; // Limpiar el campo
        }
    }

    private void ProcessText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("El texto ingresado está vacío.");
            return;
        }

        Debug.Log($"Texto recibido: {text}");

        // Actualizar el texto en pantalla si es necesario
        if (data_text != null)
        {
            data_text.text = $"Texto recibido: {text}";
        }

        // Aquí podrías agregar lógica para enviar este texto a otro script, procesarlo, etc.
    }

    private void TeleportFirstObject<T>(List<T> objects) where T : class
    {
        if (objects == null || objects.Count == 0)
        {
            Debug.LogWarning($"No se ha asignado ningún objeto a la lista {typeof(T).Name}s.");
            return;
        }

        Transform firstTransform = null;

        if (objects[0] is Component component)
        {
            firstTransform = component.transform;
        }
        else if (objects[0] is GameObject gameObject)
        {
            firstTransform = gameObject.transform;
        }

        if (firstTransform != null)
        {
            firstTransform.position = new Vector3(0, 3, 0);
            Debug.Log($"El primer objeto de la lista {typeof(T).Name} ha sido teletransportado a (0, 3, 0).");
        }
    }

    private void HandleMovement<T>(List<T> objects, ref int currentIndex) where T : class
    {
        if (objects == null || objects.Count == 0)
        {
            Debug.LogWarning($"No se ha asignado ningún objeto a la lista {typeof(T).Name}s.");
            return;
        }

        Transform currentTransform = null;

        if (objects[currentIndex] is Component component)
        {
            currentTransform = component.transform;
        }
        else if (objects[currentIndex] is GameObject gameObject)
        {
            currentTransform = gameObject.transform;
        }

        if (currentTransform == null)
        {
            Debug.LogError("El objeto actual no tiene un Transform válido.");
            return;
        }

        // Movimiento de posición
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentTransform.position += Vector3.forward * moveAmount;
            SendKeyPress("W");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            currentTransform.position += Vector3.back * moveAmount;
            SendKeyPress("S");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentTransform.position += Vector3.left * moveAmount;
            SendKeyPress("A");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentTransform.position += Vector3.right * moveAmount;
            SendKeyPress("D");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentTransform.position += Vector3.up * moveAmount;
            SendKeyPress("T");
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            currentTransform.position += Vector3.down * moveAmount;
            SendKeyPress("G");
        }

        // Rotación
        if (Input.GetKeyDown(KeyCode.N))
        {
            currentTransform.Rotate(Vector3.right * rotationAmount);
            SendKeyPress("N");
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            currentTransform.Rotate(Vector3.up * rotationAmount);
            SendKeyPress("M");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            oculusInteractionSamplerRig.transform.position = new Vector3(20, 20, 20);
            SendKeyPress("E");
        }

        // Cambiar al siguiente objeto
        if (Input.GetKeyDown(KeyCode.O))
        {
            SendKeyPress("O");
            currentIndex = (currentIndex + 1) % objects.Count;

            Transform newTransform = null;

            if (objects[currentIndex] is Component newComponent)
            {
                newTransform = newComponent.transform;
            }
            else if (objects[currentIndex] is GameObject newGameObject)
            {
                newTransform = newGameObject.transform;
            }

            if (newTransform != null)
            {
                newTransform.position = new Vector3(0, 3, 0);
                Debug.Log($"Cambiando al siguiente {typeof(T).Name}: {currentIndex} y teletransportando a (0, 3, 0)");
            }
        }
    }

    private void SendKeyPress(string key)
    {
        // Crear el mensaje con el formato deseado
        string formattedMessage = $"..T{key}";

        // Conectar al servidor y enviar el mensaje
        try
        {
            using (TcpClient client = new TcpClient(serverAddress, serverPort))
            using (NetworkStream stream = client.GetStream())
            {
                byte[] message = Encoding.ASCII.GetBytes(formattedMessage);
                stream.Write(message, 0, message.Length);
                Debug.Log($"Mensaje enviado: {formattedMessage}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al enviar el mensaje: {e.Message}");
        }
    }

}
