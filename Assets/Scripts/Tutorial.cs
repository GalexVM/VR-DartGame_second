using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer; // Referencia al VideoPlayer
    private bool videoFinished = false; // Para saber si el video ha terminado

    public Vector3 moveToPosition; // Posici�n a la que el objeto se mover� al final del video

    void Start()
    {
        // Obtener el VideoPlayer que est� en el mismo objeto
        videoPlayer = GetComponent<VideoPlayer>();

        // Verificar si hay un VideoPlayer en el objeto
        if (videoPlayer != null)
        {
            // A�adir un evento para saber cu�ndo el video ha terminado
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogError("No se encontr� un VideoPlayer en el objeto.");
        }
    }

    void Update()
    {
        // Verifica si el video termin� y realiza las acciones necesarias
        if (videoFinished)
        {
            MuteAndMove();
        }
    }

    // Este m�todo se llama cuando el video termina
    void OnVideoEnd(VideoPlayer vp)
    {
        videoFinished = true; // El video termin�
    }

    // Mutear el video y mover el objeto
    void MuteAndMove()
    {
        if (videoPlayer != null)
        {
            // Muteamos el video
            videoPlayer.SetDirectAudioMute(0, true);

            // Movemos el objeto
            transform.position = moveToPosition;

            // Desactivamos el script para evitar que siga procesando
            this.enabled = false;
        }
    }
}
