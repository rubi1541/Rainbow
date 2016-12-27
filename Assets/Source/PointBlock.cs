﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointBlock : BlockObject {

	const BlockObject.BlockType type = BlockType.PointBlock;

	// Use this for initialization
	void Start () {
		base.UpdateBlockDirectionCode();
	}

	PointBlock() :base(type){

	}

	// Update is called once per frame
	void Update () {

	}

	public override int GetBlockCode ()
	{
		//기본 형태 0 , 3 , 6 , 9
		Queue<int> Q1 = new Queue<int>(new[] { 1, 0, 0, 0 });
		int blockCode = base.MakeBlockCode (Q1);	
		return blockCode;
	}
}
