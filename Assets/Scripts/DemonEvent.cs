using UnityEngine;

public class DemonEvent : MonoBehaviour
{
    public AudioClip screamingAudio;
    public AudioClip jumpAudio;
    public void ScreamingEvent()
    {
        AudioSource.PlayClipAtPoint(screamingAudio, Camera.main.transform.position, 0.4f);
    }
    public void JumpingEvent()
    {
        AudioSource.PlayClipAtPoint(jumpAudio, Camera.main.transform.position, 0.6f);
    }
}
