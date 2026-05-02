using UnityEngine;
using Unity.Netcode.Components;
public class ClientNetworkAnimations : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
