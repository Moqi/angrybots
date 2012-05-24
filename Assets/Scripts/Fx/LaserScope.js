/*
Copyright (c) 2012, Run With Robots
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the roar.io library nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY RUN WITH ROBOTS ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL MICHAEL ANDERSON BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

/*
This source file is a modified version of the original AngryBots tech demo
source file AngryBots/Assets/Scripts/Fx/LaserScope.cs, downloaded via
http://u3d.as/content/unity-technologies/angry-bots/2aL
*/
#pragma strict 

@script RequireComponent (PerFrameRaycast)

public var scrollSpeed : float = 0.5;
public var pulseSpeed : float = 1.5;

public var noiseSize : float = 1.0;

public var maxWidth : float = 0.5;
public var minWidth : float = 0.2;

public var pointer : GameObject = null;

private var lRenderer : LineRenderer;
private var aniTime : float = 0.0;
private var aniDir : float = 1.0;

private var raycast : PerFrameRaycast;

function Start() {
	lRenderer = gameObject.GetComponent (LineRenderer) as LineRenderer;	
	aniTime = 0.0;
	
	// Change some animation values here and there
	ChoseNewAnimationTargetCoroutine();
	
	raycast = GetComponent.<PerFrameRaycast> ();
}

function ChoseNewAnimationTargetCoroutine () {
	while (true) {
		aniDir = aniDir * 0.9 + Random.Range (0.5, 1.5) * 0.1;
		yield;
		minWidth = minWidth * 0.8 + Random.Range (0.1, 1.0) * 0.2;
		yield WaitForSeconds (1.0 + Random.value * 2.0 - 1.0);	
	}	
}

function Update () {

	var equipManager : EquipmentManager = gameObject.Find("RoarDemo").GetComponent(EquipmentManager);
	if(!equipManager.IsEquipped('laser_sight')) {
		this.lRenderer.enabled = false;
		if (pointer)
			pointer.renderer.enabled = false;	
		return;
	} else {
		this.lRenderer.enabled = true;
	}
	renderer.material.mainTextureOffset.x += Time.deltaTime * aniDir * scrollSpeed;
	renderer.material.SetTextureOffset ("_NoiseTex", Vector2 (-Time.time * aniDir * scrollSpeed, 0.0));

	var aniFactor : float = Mathf.PingPong (Time.time * pulseSpeed, 1.0);
	aniFactor = Mathf.Max (minWidth, aniFactor) * maxWidth;
	lRenderer.SetWidth (aniFactor, aniFactor);
	
	// Cast a ray to find out the end point of the laser
	var hitInfo : RaycastHit = raycast.GetHitInfo ();
	if (hitInfo.transform) {
		lRenderer.SetPosition (1, (hitInfo.distance * Vector3.forward));
		renderer.material.mainTextureScale.x = 0.1 * (hitInfo.distance);
		renderer.material.SetTextureScale ("_NoiseTex", Vector2 (0.1 * hitInfo.distance * noiseSize, noiseSize));		
		
		// Use point and normal to align a nice & rough hit plane
		if (pointer) {
			pointer.renderer.enabled = true;
			pointer.transform.position = hitInfo.point + (transform.position - hitInfo.point) * 0.01;
			pointer.transform.rotation = Quaternion.LookRotation (hitInfo.normal, transform.up);
			pointer.transform.eulerAngles.x = 90.0;
		}
	}
	else {
		if (pointer)
			pointer.renderer.enabled = false;		
		var maxDist : float = 200.0;
		lRenderer.SetPosition (1, (maxDist * Vector3.forward));
		renderer.material.mainTextureScale.x = 0.1 * (maxDist);		
		renderer.material.SetTextureScale ("_NoiseTex", Vector2 (0.1 * (maxDist) * noiseSize, noiseSize));		
	}
}
