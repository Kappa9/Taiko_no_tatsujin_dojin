﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
	public Animator[] interact_Control;
	public GameObject[] interact_Hit;

	float[] hitTime;

    void OnEnable()
    {
		hitTime = new float[4];
	}

    void Update()
    {
		OnControl();
    }

	void OnControl()
	{
		if (Input.GetButtonDown("DonL1"))
		{
			hitTime[0] = Time.time;
			if (hitTime[0] - hitTime[1] <= 0.03f) interact_Control[2].Play("Active");
			interact_Control[0].Play("Active");
			interact_Control[1].Play("Active");
		}
		if (Input.GetButtonDown("DonR1"))
		{
			hitTime[1] = Time.time;
			if (hitTime[1] - hitTime[0] <= 0.03f) interact_Control[1].Play("Active");
			interact_Control[0].Play("Active");
			interact_Control[2].Play("Active");
		}
		if (Input.GetButtonDown("KaL1"))
		{
			hitTime[2] = Time.time;
			if (hitTime[2] - hitTime[3] <= 0.03f) interact_Control[5].Play("Active");
			interact_Control[3].Play("Active");
			interact_Control[4].Play("Active");
		}
		if (Input.GetButtonDown("KaR1"))
		{
			hitTime[3] = Time.time;
			if (hitTime[3] - hitTime[2] <= 0.03f) interact_Control[4].Play("Active");
			interact_Control[3].Play("Active");
			interact_Control[5].Play("Active");
		}
	}

}