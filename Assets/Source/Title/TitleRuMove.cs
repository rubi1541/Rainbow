﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleRuMove : MonoBehaviour {
	public RectTransform[] Color;
	public RectTransform[] Pos;
	public string[] SceneName;

	public RectTransform Player;

	public static int turn = -1;
	// Use this for initializations
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void StartRuMoveRoutine(int turnParam,bool RuMoveNeed){
		turn = turnParam;
		StartCoroutine (temp (turn,RuMoveNeed));
	}

	IEnumerator temp(int turnIndex,bool RuMoveNeed){

		//Debug.Log (turnIndex);
		int loop = turnIndex;	

		for (int i = 1; i < loop; i++) {
			Color [i - 1].GetComponent<Image> ().enabled = true;
			Color aa = Color [loop - 1].GetComponent<Image> ().color;
			aa.a = 1.0f;
			Color [i - 1].GetComponent<Image> ().color = aa;
		}
		Player.localPosition = Pos [turnIndex].localPosition;
		yield return new WaitForSeconds (1.0f);


		if (turnIndex == 8 || RuMoveNeed==false) {
			Debug.Log ("CCCC");
			yield return 0;
		} else {
			yield return StartCoroutine (TurnMove (turnIndex));
		}
	}

	IEnumerator TurnMove(int turnIndex){
		int loop;

		if (turnIndex > 0) {			
			loop = 50;
			Color [turnIndex - 1].GetComponent<Image> ().enabled = true;
			Color temp = Color [turnIndex - 1].GetComponent<Image> ().color;
			for (int i = 0; i < loop; i++) {
				float alpha = Mathf.Lerp (0.0f, 1.0f, (i + 1) / (float)loop);
				temp.a = alpha;
				Color [turnIndex - 1].GetComponent<Image> ().color = temp;
				yield return new WaitForFixedUpdate ();
			}
		}

		//Ru Move
		loop = 30;
		for (int i = 0; i < loop; i++) {
			Vector3 changeVec = Vector3.Lerp (Pos[turnIndex].localPosition, Pos[turnIndex+1].localPosition, (i + 1) / (float)loop);
			Player.localPosition = changeVec;
			yield return new WaitForFixedUpdate ();
		}

		/*
		yield return new WaitForSeconds (0.8f);
		GameObject.Find ("SceneChanger").GetComponent<SceneChanger> ().StartSceneFadeoutEffect ();
		yield return new WaitForSeconds (1.0f);
		SceneManager.LoadScene (SceneName [turnIndex]);
		*/
		yield return 0;
	}

}
