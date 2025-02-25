using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightThrower : MonoBehaviour
{

	public StickyLight lightPrefab;
	public Transform spawnPoint;
	Rigidbody rb;

	Astronaft controller;
	Terraformer terraformer;

	void Start()
	{
		controller = GetComponent<Astronaft>();
		rb = GetComponent<Rigidbody>();
		terraformer = FindObjectOfType<Terraformer>();
	}


	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			var l = Instantiate(lightPrefab, spawnPoint.position, spawnPoint.rotation);
			l.Init(rb.linearVelocity, controller.gravity, terraformer);
		}
	}
}
