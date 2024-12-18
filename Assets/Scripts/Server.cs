using UnityEngine;
using Unity.Netcode;

public class Server : NetworkBehaviour
{
    public static Server Instance { private set; get; }

    [SerializeField] float pushForce;


    private void Awake()
    {
        Instance = this;
    }

    [Rpc(SendTo.Server)]
    public void PushRpc(ulong networkObjectId, Vector3 forceDirection)
    {
        Rigidbody networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].GetComponent<Rigidbody>();
        networkObject.AddForce(forceDirection * pushForce, ForceMode.Impulse);
    }

    [Rpc(SendTo.Server)]
    public void CaptureObjectRpc(ulong ownerClientId, ulong networkObjectId)
    {
        NetworkObject targetPlayer = NetworkManager.Singleton.SpawnManager.SpawnedObjects[ownerClientId];
        NetworkObject objectToAttach = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        
        if (objectToAttach.TrySetParent(targetPlayer))
        {
            Rigidbody attachRigidbody = objectToAttach.GetComponent<Rigidbody>();
            attachRigidbody.isKinematic = true;
            attachRigidbody.interpolation = RigidbodyInterpolation.None;
            objectToAttach.transform.localPosition = new Vector3(0, 1.7f, 0);
        }
    }


    [Rpc(SendTo.Server)]
    public void ReleaseObjectRpc(ulong networkObjectId)
    {
        NetworkObject releasedObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];

        if (releasedObject.TryRemoveParent())
        {
            releasedObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            releasedObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
