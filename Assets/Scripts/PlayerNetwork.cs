using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private TMP_Text nameText;

    private NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private Rigidbody rigidbodyCached;
    private Transform transformCached;
    private Vector3 moveDirection;
    private Transform interactableObject;
    private Transform cameraTransform;


    private void Awake()
    {
        transformCached = transform;
        rigidbodyCached = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
        playerName.OnValueChanged += OnPlayerNameChanged;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerName.Value = ManagerUI.Instance.GetName();
        }

        nameText.text = playerName.Value.ToString();
    }

    private void OnPlayerNameChanged(FixedString128Bytes oldName, FixedString128Bytes newName)
    {
        nameText.text = newName.ToString();
    }

    private void Update()
    {
        if (!IsOwner) return;

        cameraTransform.SetPositionAndRotation(cameraHolder.position, cameraHolder.rotation);

        moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDirection.z = 1f;
        if (Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
        if (Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDirection.x = 1f;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidbodyCached.AddForce(transformCached.up * jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.Q) && interactableObject != null && interactableObject.parent == null)
        {
            Vector3 forceDirection = (interactableObject.position - transformCached.position).normalized;
            Server.Instance.PushRpc(interactableObject.GetComponent<NetworkObject>().NetworkObjectId, forceDirection);
        }

        if (Input.GetKeyDown(KeyCode.E) && interactableObject != null)
        {
            if (interactableObject.parent == null)
            {
                Server.Instance.CaptureObjectRpc(NetworkObjectId, interactableObject.GetComponent<NetworkObject>().NetworkObjectId);
                ManagerUI.Instance.InteractionTextShow(false);
            }
            else
            {
                Server.Instance.ReleaseObjectRpc(interactableObject.GetComponent<NetworkObject>().NetworkObjectId);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 targetVelocity = new Vector3(moveDirection.x, 0, moveDirection.z) * moveSpeed;
        targetVelocity = transformCached.TransformDirection(targetVelocity);
        Vector3 velocityChange = targetVelocity - rigidbodyCached.linearVelocity;
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);
        rigidbodyCached.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        if (other.gameObject.CompareTag("Pushable"))
        {
            interactableObject = other.transform;
            ManagerUI.Instance.InteractionTextShow(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;

        if (other.gameObject.CompareTag("Pushable"))
        {
            interactableObject = null;
            ManagerUI.Instance.InteractionTextShow(false);
        }
    }
}
