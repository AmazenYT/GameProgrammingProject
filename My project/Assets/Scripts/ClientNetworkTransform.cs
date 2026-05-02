using UnityEngine;
using Unity.Netcode.Components;
[DisallowMultipleComponent]
public class Client : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
