using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
	Animator animator;
	Astronaft controller;

	float speedPercent;

	void Start()
	{
		animator = GetComponentInChildren<Animator>();
		controller = GetComponent<Astronaft>();
	}


	void Update()
	{
		var state = controller.currentMoveState;

		float targetSpeedPercent = 0;
		if (state == Astronaft.MoveState.Walk)
		{
			targetSpeedPercent = 0.5f;
		}
		else if (state == Astronaft.MoveState.Run)
		{
			targetSpeedPercent = 1;
		}
		else if (state == Astronaft.MoveState.Swim)
		{
			targetSpeedPercent = 0.5f;
		}

		speedPercent = Mathf.Lerp(speedPercent, targetSpeedPercent, Time.deltaTime * 3);

		animator.SetFloat("Speed Percent", speedPercent);
		animator.SetBool("Air", !controller.grounded);
	}
}
