using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Letter : MonoBehaviour {

	private char _c; //the char shown on this letter
	public TextMesh tMesh; //the TextMesh shows the char
	public Renderer tRend; //the Renderer of 3D text, this will determine whether the char is visible
	public bool big = false; //big letters act differently
	//linear interpolation
	public List<Vector3> pts = null;
	public float timeDuration = 0.5f;
	public float timeStart = -1;
	public string easingCurve = Easing.InOut; // easing from utils.cs


	void Awake() {
		tMesh = GetComponentInChildren<TextMesh> ();
		tRend = tMesh.renderer;
		visible = false; 
	}

	//used to get or set _c and the letter shown by 3D text
	public char c {
		get {
			return(_c);
		}
		set {
			_c = value;
			tMesh.text = _c.ToString();
		}
	}

	//gets or sets _c as a string
	public string str {
		get {
			return(_c.ToString());
		}
		set {
			c = value[0];
		}
	}

	//enables or disables the renderer for 3D text, which causes the char to be visible
	//or invisible respectively 
	public bool visible {
		get {
			return (tRend.enabled);
		}
		set {
			tRend.enabled = value;
		}
	}

	//gets or sets the color of the rounded rectangle 
	public Color color {
		get {
			return (renderer.material.color);
		}
		set {
			renderer.material.color = value;
		}
	}

	//sets the position of the Letter's gameObject 
	public Vector3 pos {
		set {
			//transform.position = value;

			//find a midpoint that is a random distace from the actual
			//midpoint between the current position and the value passed in
			Vector3 mid = (transform.position + value) / 2f;
			//the random distance will be within 1/4 of the magnitude of the
			//line from the actual midpoint
			float mag = (transform.position - value).magnitude;
			mid += Random.insideUnitSphere * mag * 0.25f;
			//create a list<Vector3>() of bezier points
			pts = new List<Vector3> () { transform.position, mid, value};
			//if timestart is at the default -1 then set it 
			if (timeStart == -1)
				timeStart = Time.time;
		}
	}

	//moves immediately to the new position
	public Vector3 position {
		set {
			transform.position = value;
		}
	}

	//interpolation code
	void Update () {
		if (timeStart == -1)
			return;

		//standard linear interpolation code
		float u = (Time.time - timeStart) / timeDuration;
		u = Mathf.Clamp01 (u);
		float u1 = Easing.Ease (u, easingCurve);
		Vector3 v = Utils.Bezier (u1, pts);
		transform.position = v;

		//if the interpolation is done, set timeStart back to -1
		if (u == 1)
			timeStart = -1;
	}
}
