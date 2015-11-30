using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wyrd {
	public string str; //a string representation of the word
	public List<Letter> letters = new List<Letter>();
	public bool found = false; //true if the player has found this word

	//property to set visibility of the 3D text of each letter
	public bool visible {
		get {
			if (letters.Count == 0)
				return(false);
			return(letters [0].visible);
		}
		set {
			foreach (Letter lett in letters) {
				lett.visible = value;
			}
		}
	}

	//a property to set the rounded rectangle color of each Letter
	public Color color {
		get {
			if (letters.Count == 0) return(Color.black);
			return(letters[0].color);
		}
		set {
			foreach (Letter lett in letters) {
				lett.color = value;
			}
		}
	}

	//adds a Letter to letters 
	public void Add (Letter lett) {
		letters.Add (lett);
		str += lett.c.ToString ();
	}
}
