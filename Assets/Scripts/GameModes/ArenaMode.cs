using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaMode : MonoBehaviour
{
	List<GameObject> players;
	List<int> lives;
	List<int> ranks;
	TwoLocalPlayerGameController control;
	bool initialised;
	const int MAX_LIVES = 3;
		// Use this for initialization
		void Start ()
		{
			control = GameObject.FindWithTag("GameController").GetComponent<TwoLocalPlayerGameController>();
			initialised = false;
			lives = new List<int>();
			ranks = new List<int>();

		}
	
		// Update is called once per frame
		void Update ()
		{
			if(!initialised){
				players = control.playerList;
				for(int i = 0; i < players.Count; ++i){
					lives.Add(MAX_LIVES);
					ranks.Add(1);
				}	
				updateRanks();
			}
				

			if(players.Count == 1){
				PlayerInputController p = players[0].GetComponent<PlayerInputController>();
				Debug.Log("Player " + p.numberOfControllerString + " won the Battle!");
			}
				
			for(int i = 0; i<players.Count; ++i){
				Car car = players[i].GetComponent<Car>();
					if(lives[i] > 0 && car.getHealth()<=0.0f){
						lives[i]--;
						PlayerInputController p = players[i].GetComponent<PlayerInputController>();
						if(lives[i] == 0){
							Debug.Log("Player " + p.numberOfControllerString + " eliminated!");
							players.RemoveAt(i);
							lives.RemoveAt(i);
							ranks.RemoveAt(i);
						} else {
							control.reInstanciatePlayer(p.numberOfControllerString);
						}
						updateRanks();
					}
			}
		}

		void updateRanks(){
			int currentLives = MAX_LIVES;
			int rank = 1;
			bool rankChanged = false;
			while(currentLives > 0){
			for(int i = 0; i < players.Count; ++i){
					if(lives[i] == currentLives){
						ranks[i] = rank;
						players[i].GetComponent<PlayerInputController>().hud.rank.text = ""+rank + " place";
						rankChanged = true;
					}
				}
				currentLives--;
				if(rankChanged)
					rank++;

				rankChanged = false;
			}
		}
}

