﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;



public class BlockManager : MonoBehaviour {

	Player player;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player").GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private BlockCode[,] GetBlockMap(){
		BlockCode[,] arr = new BlockCode[7, 7];
		//arr initialize
		for (int i = 0; i < arr.GetLength (0); i++) {
			for (int j = 0; j < arr.GetLength (1); j++) {
				arr[i,j] = new BlockCode();
			}
		}

		//get blockCode
		Transform map = GameObject.Find ("Map").transform;
		BlockObject[] blockObjects = map.GetComponentsInChildren<BlockObject> ();
		foreach(BlockObject child in blockObjects){
			Vector3 localPos = child.transform.position;
			int x = Mathf.RoundToInt (localPos.x);
			int y = Mathf.RoundToInt (localPos.y);
			try{
				arr [x, y] = child.GetBlockCode();			

			}catch(IndexOutOfRangeException e){
				Debug.Log ("Error " + e.Message+"  "+x+"  "+y);
			}
		}

		return arr;
	}


	private int[,] FindMovableMapAlg(BlockCode[,] blockMap,Vector3 PlayerPos){

		//initialize;
		int[,] movableMap = new int[blockMap.GetLength(0),blockMap.GetLength(1)];
		Array.Clear (movableMap, 0, movableMap.Length);

		Queue<KeyValuePair<int,int>> Q1 = new Queue<KeyValuePair<int,int>>();
		 
		int playerXpos = Mathf.RoundToInt (PlayerPos.x);
		int playerYpos = Mathf.RoundToInt (PlayerPos.y);
		Q1.Enqueue(new KeyValuePair<int,int>(playerXpos , playerYpos));
		movableMap [playerXpos, playerYpos] = 1;

		while (Q1.Count != 0) {
			KeyValuePair<int,int> temp = Q1.Dequeue ();
			for (int i = 0; i < 4; i++) {
				int key = temp.Key;
				int value = temp.Value;
				if (blockMap [temp.Key, temp.Value].GetCodeByIndex (i) == '1') {
					switch (i) {
					case 0:
						value += 1;
						break;
					case 1:
						key += 1;
						break;
					case 2:
						value -= 1;
						break;
					case 3:
						key -= 1;
						break;
					}

					KeyValuePair<int,int> newKey = new KeyValuePair<int, int>(key,value);

					//newkeyPos in BlockMap? and 
					if (newKey.Key >=0 && newKey.Value >=0 &&
						newKey.Key <blockMap.GetLength (0) && newKey.Value <blockMap.GetLength (1)){
						if (movableMap [newKey.Key, newKey.Value] == 1)
							continue;

						if (blockMap [newKey.Key, newKey.Value].GetCodeByIndex ((i + 2) % 4) == '1') {
							movableMap [newKey.Key, newKey.Value] = 1;
							Q1.Enqueue (newKey);
						}
					}
				}

			}
		}
	

		return movableMap;
	}


	private struct RoadInfo{
		public int isExist;
		public int count;
		public int direction;
		public bool visited;
		public BlockCode blockcode;
	};


	private RoadInfo[,] MakeMoveGuideMap(int[,] movableMap , Vector3 GoalPos,BlockCode[,] codeMap){
		RoadInfo[,] moveGuideMap = new RoadInfo[movableMap.GetLength (0), movableMap.GetLength (1)];

		//initialize
		for (int i = 0; i < movableMap.GetLength (0); i++) {
			for (int j = 0; j < movableMap.GetLength (1); j++) {
				moveGuideMap [i,j].count = 1000;
				moveGuideMap [i,j].isExist = movableMap [i, j];
				moveGuideMap [i,j].direction = -1;
				moveGuideMap [i,j].visited = false;
				moveGuideMap [i, j].blockcode = codeMap [i, j];
			}
		}

		int goalXpos = Mathf.RoundToInt (GoalPos.x);
		int goalYpos = Mathf.RoundToInt (GoalPos.y);

		if (IsAccessible (goalXpos, goalYpos, moveGuideMap) == true) 
		{
			moveGuideMap [goalXpos, goalYpos].count = 0;
			moveGuideMap [goalXpos, goalYpos].direction = 0;
			FindRoadAlg (goalXpos, goalYpos, moveGuideMap);
		}

		return moveGuideMap;

	}

	//
	private void FindRoadAlg(int startX,int startY,RoadInfo[,] infoMap){
		infoMap[startX,startY].visited = true;

		// 12 3 6 9
		int[] aa = { 0, +1, 0, -1 };
		int[] bb = { +1, 0, -1, 0 };	
	
		for (int i = 0; i < 4; i++)
		{			
			if (IsAccessible (startX + aa [i], startY + bb [i], infoMap)) 
			{
				if ( infoMap [startX,startY].blockcode.GetCodeByIndex (i) == '1' &&
					infoMap [startX + aa [i], startY + bb [i]].blockcode.GetCodeByIndex ((i + 2) % 4) == '1') 
				{
					if (infoMap [startX, startY].count + 1 <= ((infoMap [startX + aa [i], startY + bb [i]]).count)) 
					{
						(infoMap [startX + aa [i], startY + bb [i]]).count = infoMap [startX, startY].count + 1;
						(infoMap [startX + aa [i], startY + bb [i]]).direction = i + 1;
						FindRoadAlg (startX + aa [i], startY + bb [i], infoMap);
					}
				}
			}
		}

		infoMap[startX,startY].visited = false;
		return;
	}


	private bool IsAccessible(int xPos,int yPos,RoadInfo[,] infoMap){
		if (xPos < 0 || xPos > infoMap.GetLength(0)-1)
			return false;
		if (yPos < 0 || yPos > infoMap.GetLength(1)-1)
			return false;
		if (infoMap[xPos,yPos].isExist == 0)
			return false;
		if (infoMap[xPos,yPos].visited == true)
			return false;
		

		return true;
	}

	//백트래킹 맵 
	public void GetMoveGuideMap(Vector3 clickedBlockPos){
		GameObject.Find ("Player");

		BlockCode[,] blockMap = GetBlockMap ();	


		int[,] movableMap = FindMovableMapAlg (blockMap, player.transform.position);	
		/*
		for (int i = 0; i < 7; i++) {
			String temp = "";
			for (int j = 0; j < 7; j++) {
				temp += movableMap [i,j] + "\t";
			}
			Debug.Log (temp);
		}*/

		RoadInfo[,] moveGuideMap = MakeMoveGuideMap (movableMap, clickedBlockPos,blockMap);



		int[,] resultGuideMap = new int[moveGuideMap.GetLength (0), moveGuideMap.GetLength (1)];

		for (int i = 0; i < 7; i++) {
			for (int j = 0; j < 7; j++) {				
				resultGuideMap [i, j] = moveGuideMap [i, j].direction;
			}
		}
	
		if (player != null)
			player.MoveGuideMapUpdate (resultGuideMap);
		//return 
	}

	public void PrintTemp(){
		Debug.Log ("AAA");
	}
}
