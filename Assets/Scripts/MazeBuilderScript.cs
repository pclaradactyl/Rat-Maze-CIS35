﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBuilderScript : MonoBehaviour {

	private GameObject[] walls;
	private Maze myMaze;
	private ElectricityGenerator obsGen;
	public int dimensions;
	public string mazeStr;
	public TextAsset mazeTxt;
	public bool BuildfromTxt;


//	void Start() {
//
//		RandomMazeGenerator gen = new RandomMazeGenerator (dimensions);
//		gen.build();
//		mazeStr = gen.print ();
//		BuildMaze ();
//		SpawnMaze (myMaze );
//	}


	public void BuildMaze () {
		myMaze = new Maze ();
		walls = Resources.LoadAll<GameObject> ("Halls");

		if (BuildfromTxt) {
			myMaze.GenerateFromTxt(mazeTxt);
		}
		else {
			myMaze.GenerateSimple(dimensions, mazeStr);
		}

		SpawnMaze (myMaze);

	}

	public void BuildMaze (int dim, string maze) {
		obsGen = new ElectricityGenerator ();
		myMaze = new Maze ();
		walls = Resources.LoadAll<GameObject> ("Halls");

		myMaze.GenerateSimple(dim, maze);	

		obsGen.Solve (dim, maze);
		SpawnMaze (myMaze);
		if (obsGen.GenerateFloor ())
			SpawnObstacle (obsGen);
	}
		
	void SpawnMaze(Maze myMaze){
		GameObject maze = new GameObject ();
		maze.name = "Maze";
		maze.tag = "maze";
		foreach (Tile tile in myMaze.tiles) {
			GameObject myWall = Instantiate (walls [tile.type]) as GameObject;
			myWall.transform.position = new Vector3 (tile.x * 5, tile.y, tile.z * 5);
			myWall.transform.parent = maze.transform;
		}
	}

	void SpawnObstacle(ElectricityGenerator myObs) {
		GameObject floorTile = Instantiate (Resources.Load<GameObject> ("Electric Floor")) as GameObject;
		floorTile.name = "Electric Floor";
		floorTile.tag = "ElectricFloor";
		Debug.Log ("Floor at (" + myObs.getFloor () [0] + ", " + myObs.getFloor () [1] + ")");
		floorTile.transform.position = new Vector3(myObs.getFloor ()[0], 0, myObs.getFloor ()[1]);
		Debug.Log ("Rotate Floor: " + myObs.getFloor () [2]);
		floorTile.transform.Rotate (0, myObs.getFloor () [2], 0, Space.World);

		GameObject leverTile = Instantiate (Resources.Load<GameObject> ("LeverTile")) as GameObject;
		leverTile.name = "Lever Tile";
		leverTile.transform.parent = floorTile.transform;
		Debug.Log ("Lever at (" + myObs.getLever () [0] + ", " + myObs.getLever () [1] + ")");
		leverTile.transform.position = new Vector3 (myObs.getLever()[0], 0, myObs.getLever()[1]);
		Debug.Log ("Rotate lever " + myObs.getLever () [2]);
		leverTile.transform.Rotate (0, myObs.getLever () [2], 0, Space.World);

		GameObject lever = leverTile.transform.Find ("Lever").gameObject;
		LeverPress leverScript = lever.GetComponent<LeverPress>();
		leverScript.distance = myObs.obstacleDistance;
		leverScript.floor = floorTile.transform.Find ("Electricity").gameObject;
		leverScript.floorSound = lever.GetComponent<AudioSource> ();

		Debug.Log("Distance is "+ leverScript.distance);
	}
}
