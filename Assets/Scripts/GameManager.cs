using UnityEngine;
using Unity.Netcode;
using System.Linq;
using JetBrains.Annotations;

public class GameManager : NetworkBehaviour {
  public GameObject[] spawnPoints;
  public GameObject[] players = new GameObject[0];


    private void Start() {
        NetworkManager.Singleton.OnConnectionEvent += (sourceId, data) => {
            ulong clientId = data.ClientId;
            if (data.EventType == ConnectionEvent.ClientConnected) {
                Debug.Log("Client connected: " + clientId);
                NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
                int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count();
                playerObject.transform.position = spawnPoints[playerCount - 1].transform.position;
                playerObject.gameObject.name = "Player " + playerCount;
            }
        };
    }
    public void registerPlayer(GameObject player) {
    Debug.Log("Player registered");
    /*players = players.Append(player).ToArray();
    player.transform.position = spawnPoints[players.Length].transform.position;*/

  }

  public void Update() {

  }

}
