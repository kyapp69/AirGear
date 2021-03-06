﻿using UnityEngine;
using System.Collections;

public class PlayerCam : MonoBehaviour {
	
	public Transform player;
	Player _player;
	public Vector2 rotateSpeed, verticalLimit;
	CharacterController cc;
	Vector2 inputVector;
	Vector3 offset;
	Quaternion camRotation;
	
	// Use this for initialization
	void Awake () 
	{
		cc = GetComponent<CharacterController>();
		_player = player.gameObject.GetComponent<Player>();
		offset = player.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		CameraControl();
	}
	
	float ClampAngle(float angle, float min, float max)
	{
		if(angle < -360)
			angle +=360;
		if(angle > 360)
			angle -= 360;
		return Mathf.Clamp(angle,min,max);
	}
	
	void OnGUI()
	{
		if(!Screen.lockCursor)
		{
			if(GUI.Button(new Rect(0,0,Screen.width/12,Screen.width/25),"Lock Cursor"))
				Screen.lockCursor = true;
		}
		GUI.Label(new Rect(Screen.width-100,0,500,25),_player.moveDirection.ToString());
		GUI.Label(new Rect(Screen.width-100,18,500,25),_player.ySpeed.ToString());
		GUI.Label(new Rect(Screen.width-100,36,500,25),_player.currentSpeed.ToString());
		GUI.Label(new Rect(Screen.width-100,54,500,25),_player.currentState.ToString());
		GUI.Label(new Rect(Screen.width-100,72,500,25),rotateSpeed.ToString());
		GUI.Label(new Rect(Screen.width-100,90,500,25),_player.transform.position.ToString());
		GUI.Label(new Rect(Screen.width-100,108,500,25),_player.hookOffset.ToString());
		if(_player.wallHit.transform)GUI.Label(new Rect(Screen.width-100,126,500,25),_player.wallHit.transform.position.ToString());
		rotateSpeed.x = GUI.VerticalSlider(new Rect(Screen.width*9.7f/10,200,10,400),rotateSpeed.x,30,200);
		rotateSpeed.y = GUI.VerticalSlider(new Rect(Screen.width*9.8f/10,200,10,400),rotateSpeed.y,30,200);
	}
	
	void CameraControl()
	{
		if(Mathf.Abs(Input.GetAxis("Camera X")) > 0.01)
		inputVector.x += Input.GetAxis("Camera X")*rotateSpeed.x*Time.deltaTime;
		if(Mathf.Abs(Input.GetAxis("Camera Y")) > 0.01)
		inputVector.y += Input.GetAxis("Camera Y")*rotateSpeed.y*Time.deltaTime;
		
		//Keep vertical rotation within bounds specified in the inspector
		inputVector.y = ClampAngle(inputVector.y,verticalLimit.x,verticalLimit.y);
		
		//Set rotation variable to match the current input
		Quaternion rotation = Quaternion.Euler(inputVector.y,inputVector.x,0);
		
		//Set the position relative to the player and based on rotation
		Vector3 position = player.position - rotation*offset;
	
		//Actually perform the changes
		transform.rotation = rotation;
		
		//Pathfinding goes here:
		//Vector3 moveDirection = position - transform.position;
		//cc.Move(moveDirection);
		transform.position = position;
		
		//Rotate the player so they're facing where the camera is facing
		if(_player.currentState != Player.playerState.wallriding)
			_player.rotationCube.transform.rotation = Quaternion.Lerp(_player.rotationCube.transform.rotation, Quaternion.Euler(0,inputVector.x,0),0.7f);//should only do this if the player instances speed is greater than 0
	}
}
