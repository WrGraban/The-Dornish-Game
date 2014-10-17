using UnityEngine;
using System.Collections;

public class Blinker : MonoBehaviour 
{
    private float timer = 0f;
    private Color startColor;
    private Color curColor;
	public bool isBlinking = false;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
		if(isBlinking == true)
		{ 
			timer += Time.deltaTime * 5;
			curColor.a = 0.7f + Mathf.Sin(timer) * 0.15f;
		
			this.renderer.material.color = curColor;
		}
	}

	public void Enable()
	{
		startColor = this.renderer.material.color;
		curColor = startColor;
		isBlinking = true;
	}

	public void Disable()
	{
		this.renderer.material.color = startColor;
		isBlinking = false;
		timer = 0f;
	}
}
