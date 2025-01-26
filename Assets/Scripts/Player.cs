using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Player : NetworkBehaviour {

    [SerializeField] GameObject bubble;
    public GameManager gameManager;
    public Button blowBtn;
    public RectTransform canvasRT;
    public RectTransform blowButtonRT;
    public Sprite[] bubbleSprites;
    public NetworkAnimator playerAnimator;

    private NetworkVariable<bool> ended = new(false);

    public NetworkVariable<int> bubbleSize = new(1);
    public override void OnNetworkSpawn() {
        gameManager.registerPlayer(gameObject);

    }

    public void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void Start() {
        playerAnimator = GetComponent<NetworkAnimator>();
        if (IsOwner) {
            blowBtn = GameObject.Find("BlowButton").GetComponent<Button>();
            canvasRT = GameObject.Find("GameCanvas").GetComponent<RectTransform>();
            blowButtonRT = blowBtn.GetComponent<RectTransform>();
            blowBtn.onClick.AddListener(() => {

                    Pump();
                    MoveButtonToRandomPosition();
                    playerAnimator.SetTrigger("Pump");
                    if (bubbleSize.Value >= 100) {
                        blowBtn.gameObject.SetActive(false);
                        EndPlayerGameRpc();
                    }
            });
        }

    }
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (IsOwner) {
                Debug.Log(bubbleSize.Value);
                playerAnimator.SetTrigger("Pump");
                Pump();
            }
        }
        float newSize = 1f + (bubbleSize.Value / 25f);
        bubble.transform.localScale = new Vector3(newSize, newSize, newSize);
        UpdateBubbleSprite();
        if (ended.Value && IsOwner) {
            Debug.Log("se acabo");
            BubbleExit();
        }
    }

    public void UpdateBubbleSprite () {
        
        if (bubbleSize.Value < 25) {
            bubble.GetComponent<SpriteRenderer>().sprite = bubbleSprites[0];
        } else if (bubbleSize.Value < 50) {
            bubble.GetComponent<SpriteRenderer>().sprite = bubbleSprites[1];
        } else if (bubbleSize.Value < 70) {
            bubble.GetComponent<SpriteRenderer>().sprite = bubbleSprites[2];
        } else if (bubbleSize.Value < 85) {
            bubble.GetComponent<SpriteRenderer>().sprite = bubbleSprites[3];
        } else {
            bubble.GetComponent<SpriteRenderer>().sprite = bubbleSprites[4];
        }
    }

    [Rpc(SendTo.Server)]
    void EndPlayerGameRpc(RpcParams rpcParams = default) {
        ended.Value = true;
    }
    [Rpc(SendTo.Server)]
    void SubmitBlowRpc(RpcParams rpcParams = default) {
        bubbleSize.Value += 1;
        float newSize = 1f + (bubbleSize.Value / 25f);
        bubble.transform.localScale = new Vector3(newSize, newSize, newSize);
        bubble.transform.localPosition = bubble.transform.localPosition + new Vector3(0, 0.06f, 0);

    }

    [Rpc(SendTo.Server)]
    void SubmitExitBubbleRpc(RpcParams rpcParams = default) {
        Vector3 newPos = Vector3.Lerp(bubble.transform.localPosition, bubble.transform.localPosition + Vector3.up, 2f * Time.deltaTime);
        bubble.transform.localPosition = newPos;
    }

    public void Pump() {
        SubmitBlowRpc();
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

    public void BubbleExit () {
        SubmitExitBubbleRpc();
    }
}

