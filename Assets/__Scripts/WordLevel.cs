using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable] //wordLevels can be viewed in the inspector
public class WordLevel { //wordLevel does NOT extend MonoBehavior
	public int levelNum;
	public int longWordIndex;
	public string word;
	//a dictionary<,> of all the letters in word
	public Dictionary<char, int> charDict;
	//all the words that can be spelled with the letters in charDict
	public List<string> subWords;

	//a static function that counts the instances of chars in a string and 
	//returns a Dictionary<char, int> that contains this information
	static public Dictionary<char, int> MakeCharDict(string w) {
		Dictionary<char, int> dict = new Dictionary<char, int>();
		char c;
		for (int i=0; i<w.Length; i++) {
			c = w[i];
			if(dict.ContainsKey (c)) {
				dict[c]++;
			} else {
				dict.Add (c,1);
			}
		}
		return(dict);
	}

	//this static method checks to see whether the word can be spelled with the 
	//chars in level.charDict
	public static bool CheckWordInLevel(string str, WordLevel level) {
		Dictionary<char, int> counts = new Dictionary<char, int>();
		for(int i=0; i<str.Length; i++) {
			char c = str[i];
			//if the charDict contains char c
			if(level.charDict.ContainsKey(c)) {
				//if counts doesn't already have char c as a key
				if(!counts.ContainsKey(c)) {
					//then add a new key with a value of 1
					counts.Add (c,1);
				} else {
					counts[c]++;
				}
				//if this means that there are more instances of char c in str
				//than are available in level.charDict
				if(counts[c] > level.charDict[c]) {
					//then return false
					return(false);
				}
			} else { 
					//the char c isn't in level.word, so return false
					return(false);
			}
		}
		return(true);
	}

}


