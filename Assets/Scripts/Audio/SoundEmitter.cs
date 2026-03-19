using UnityEngine;

/// <summary>
/// Any class derived from this base class will have an AudioSource attached to it that can be accessed by other
/// classes using the public property called Source.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{

    public AudioSource Source { get { return _Source; } private set { _Source = value; } }
    protected AudioSource _Source;

    protected virtual void Awake()
    {
        TryGetComponent(out _Source);
    }
}
