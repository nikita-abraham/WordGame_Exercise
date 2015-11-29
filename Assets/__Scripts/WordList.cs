using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WordList : MonoBehaviour {
	public static WordList S;

	public TextAsset wordListText;
	public int numToParseBeforeYield = 10000;
	public int wordLengthMin = 3;
	public int wordLengthMax = 7;

	public bool ________________;

	public int currLine = 0;
	public int totalLines;
	public int longWordCount;
	public int wordCount;

	private string[] lines;
	private List<string> longWords;
	private List<string> words;

	void Awake() {
		S = this; 
	}

	void Start() {
		//split the text of wordListText on line feeds, which creates a large
		//populated string[] with all the words from the list
		lines = wordListText.text.Split ('\n');
		totalLines = lines.Length;

		//this starts the coroutine ParseLines(). Couroutines can be paused in 
		//the middle to allow other code to execute
		StartCoroutine (ParseLines ());
	}

	//all coroutines have IEnumerator as their return type
	public IEnumerator ParseLines() {
		string word;
		//in it the Lists to hold the longest words and all valid words
		longWords = new List<string> ();
		words = new List<string> ();

		for (currLine = 0; currLine < totalLines; currLine++) {
			word = lines [currLine];

			//if the word is as long as wordLengthMax
			if (word.Length == wordLengthMax) {
				//then store it in longwords
				longWords.Add (word);
			}
			//if it's between wordLengthMin and wordLengthMax in length
			if (word.Length >= wordLengthMin && word.Length <= wordLengthMax) {
				//then add it to the list of all valid words
				words.Add (word);
			}

			//determine whether the coroutine should yield
			//this uses a modulus function to yield every 10,000th record
			//(or whatever you have numToParseBeforeYield set to)
			if (currLine % numToParseBeforeYield == 0) {
				//count the wors in each list ot show that the parsing is 
				//progressing
				longWordCount = longWords.Count;
				wordCount = words.Count;
				//this yields execution until the next frame
				yield return null;

				//the yield will cause the execution of ths method to wait 
				//here while other code executes and then continue from this point
			}
		}
	}

	//these methods allow other classes to access the private List<string>
	public List<string> GetWords() {
		return(words);
	}
	public string GetWord(int ndx) {
		return (words [ndx]);
	}
	public List<string> GetLongWords() {
		return(longWords);
	}
	public string GetLongWord(int ndx) {
		return(longWords [ndx]);
	}
		   

}
