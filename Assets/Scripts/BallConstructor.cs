﻿using UnityEngine;
using System.Collections;

public class BallConstructor : MonoBehaviour {

    public Camera cam;

    public CircleCollider2D ball;

    private const float ballScale = 0.1f;

    // Use this for initialization
    void Start () {
	    var renderer = (SpriteRenderer)gameObject.GetComponent<SpriteRenderer>();

        var currentScale = renderer.bounds.size.x / cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x;

        renderer.transform.localScale = new Vector2(ballScale / currentScale, ballScale / currentScale);

        ball.radius = renderer.sprite.bounds.size.x / 2;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}