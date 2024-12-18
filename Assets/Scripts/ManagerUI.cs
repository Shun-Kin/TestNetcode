using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class ManagerUI : MonoBehaviour
{
    public static ManagerUI Instance { private set; get; }

    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button disconnectButton;
    [SerializeField] private TMP_Text serverStatusText;
    [SerializeField] private TMP_Text clientStatusText;
    [SerializeField] private TMP_InputField addressInputField;
    [SerializeField] private TMP_InputField portInputField;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject interactionText;

    private UnityTransport transport;


    private void Awake()
    {
        Instance = this;

        serverButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });

        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });

        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });

        disconnectButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
        });
    }

    private void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        addressInputField.onValueChanged.AddListener((string value) =>
        {
            transport.ConnectionData.Address = value;
        });

        portInputField.onValueChanged.AddListener((string value) =>
        {
            if (ushort.TryParse(value, out ushort result))
            {
                transport.ConnectionData.Port = result;
            }
        });

        NetworkManager.Singleton.OnServerStarted += () => {
            serverStatusText.text = "Server Running";
            clientStatusText.text = "";
        };

        NetworkManager.Singleton.OnServerStopped += (bool hostClient) => {
            serverStatusText.text = "";
        };

        NetworkManager.Singleton.OnClientStarted += () => {
            clientStatusText.text = "Client Connected";
        };

        NetworkManager.Singleton.OnClientStopped += (bool hostClient) => {
            clientStatusText.text = "Disconnected";
        };
    }

    public void InteractionTextShow(bool value)
    {
        interactionText.SetActive(value);
    }

    public string GetName()
    {
        return nameInputField.text;
    }
}
