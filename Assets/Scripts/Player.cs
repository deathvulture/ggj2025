using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Player : NetworkBehaviour
{

    [SerializeField] GameObject bubble;

    public NetworkVariable<int> bubbleSize = new(1);
    public override void OnNetworkSpawn() {
        Debug.Log("Connected");
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("ahi va");
            SubmitBlowRpc();
        }

        bubble.transform.localScale = new Vector3(bubbleSize.Value, bubbleSize.Value, bubbleSize.Value);
    }

    [Rpc(SendTo.Server)]
    void SubmitBlowRpc(RpcParams rpcParams = default) {
        bubbleSize.Value += 1;
        bubble.transform.localScale = new Vector3(bubbleSize.Value, bubbleSize.Value, bubbleSize.Value);
    }
}

