using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

  [SerializeField] GameObject bubble;
  public GameManager gameManager;
  public Button blowBtn;
  public RectTransform canvasRT;
  public RectTransform blowButtonRT;


  public NetworkVariable<int> bubbleSize = new(1);
  public override void OnNetworkSpawn() {
    gameManager.registerPlayer(gameObject);

  }

  public void Awake() {
    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    blowBtn = GameObject.Find("BlowButton").GetComponent<Button>();
    canvasRT = GameObject.Find("GameCanvas").GetComponent<RectTransform>();
    blowButtonRT = blowBtn.GetComponent<RectTransform>();
    blowBtn.onClick.AddListener(() => {
      if (IsOwner) {
        SubmitBlowRpc();
        MoveButtonToRandomPosition();
      }
    });
  }
  public void Update() {
    if (Input.GetKeyDown(KeyCode.Space)) {
      if (IsOwner) {
        Debug.Log(bubbleSize.Value);
        SubmitBlowRpc();
      }
    }
    float newSize = 1f + (bubbleSize.Value / 25f);
    bubble.transform.localScale = new Vector3(newSize, newSize, newSize);
  }

  [Rpc(SendTo.Server)]
  void SubmitBlowRpc(RpcParams rpcParams = default) {
    bubbleSize.Value += 1;
    float newSize = 1f + (bubbleSize.Value / 25f);
    bubble.transform.localPosition = bubble.transform.localPosition + new Vector3(0, 0.07f, 0);
    bubble.transform.localScale = new Vector3(newSize, newSize, newSize);
  }

  public void MoveButtonToRandomPosition() {
    // Get the dimensions of the button
    float btnWidth = blowBtn.GetComponent<RectTransform>().rect.width;
    float btnHeight = blowBtn.GetComponent<RectTransform>().rect.width;

    // Get the dimensions of the canvas
    float canvasWidth = canvasRT.rect.width;
    float canvasHeight = canvasRT.rect.height;

    // Calculate the bounds for the button
    float xMin = (canvasWidth * -0.5f) + btnWidth / 2;
    float xMax = (canvasWidth * 0.5f) - btnWidth / 2;
    float yMin = (canvasHeight * -0.5f) + btnHeight / 2;
    float yMax = (canvasHeight * 0.5f) - btnHeight / 2;

    // Generate random position within bounds
    Debug.Log("xMin: " + xMin + " xMax: " + xMax + " yMin: " + yMin + " yMax: " + yMax);
    float randomX = Random.Range(xMin, xMax);
    float randomY = Random.Range(yMin, yMax);

    // Set the button position
    blowButtonRT.localPosition = new Vector3(randomX, randomY, 0);

  }
}

