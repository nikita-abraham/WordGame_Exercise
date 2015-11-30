using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Letter : MonoBehaviour {

	private char _c; //the char shown on this letter
	public TextMesh tMesh; //the TextMesh shows the char
	public Renderer tRend; //the Renderer of 3D text, this will determine whether the char is visible
	public bool big = false; //big letters act differently

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
			transform.position = value;
		}
	}

}
