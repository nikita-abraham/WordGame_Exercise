using UnityEngine;
using System.Collections;
using System.Collections.Generic; // List<> and Dictionary<>
using System.Linq; 

public enum GameMode {
	preGame, //before the game starts
	loading, //the word list is loading and being parsed
	makeLevel, //the individual WordLevel is being created
	levelPrep, //the level visuals are instantiated
	inLevel //level is in progress
}

public class WordGame : MonoBehaviour {
	static public WordGame S;

	public GameObject prefabLetter;
	public Rect wordArea = new Rect (-24, 19, 48, 28);
	public float letterSize = 1.5f;
	public bool showAllWyrds = true;
	public float bigLetterSize = 4f;
	public Color bigColorDim = new Color (0.8f, 0.8f, 0.8f);
	public Color bigColorSelected = Color.white;
	public Vector3 bigLetterCenter = new Vector3(0, -16, 0);
	
	public bool ____________;

	public GameMode mode = GameMode.preGame;
	public WordLevel currLevel;
	public List<Wyrd> wyrds;
	public List<Letter> bigLetters;
	public List<Letter> bigLettersActive;

	void Awake() {
		S = this;
	}

	void Start() {
		mode = GameMode.loading;
		WordList.S.Init ();
	}

	//called by the SendMessage() command from WordList
	public void WordListParseComplete() {
		mode = GameMode.makeLevel;
		//make a level and assign it to currLevel, the current WordLevel
		currLevel = MakeWordLevel ();
	}

	//with the default value of -1, this method will generate a level from 
	//a random word
	public WordLevel MakeWordLevel(int levelNum= -1) {
		WordLevel level = new WordLevel ();
		if (levelNum == -1) {
			//pick a random level
			level.longWordIndex = Random.Range (0, WordList.S.longWordCount);
		} else {
			//added later
		}
		level.levelNum = levelNum;
		level.word = WordList.S.GetLongWord (level.longWordIndex);
		level.charDict = WordLevel.MakeCharDict (level.word);

		//call a coroutine to check all the words in the WordList and see 
		//whether each word can be spelled by the chars in level.charDict
		StartCoroutine (FindSubWordsCoroutine (level));

		//this returns the level before the coroutine finished, so 
		//SubWordSearchComplete() is called when the coroutine is done
		return (level);
	}

	//a coroutine that finds words that can be spelled in this level
	public IEnumerator FindSubWordsCoroutine(WordLevel level) {
		level.subWords = new List<string> ();
		string str;

		List<string> words = WordList.S.GetWords ();

		//iterate through all the words in the WordList
		for (int i = 0; i<WordList.S.wordCount; i++) {
			str = words [i];
			//check whetehr each one can be spelled using level.charDict
			if (WordLevel.CheckWordInLevel (str, level)) {
				level.subWords.Add (str);
			}
			//yield if we've parsed a lot of words this frame
			if (i % WordList.S.numToParseBeforeYield == 0) {
				//yield until the next frame
				yield return null;
			}
		}
		//List<String>.Sort() sorts alphabetically by default
		level.subWords.Sort ();
		//now sort by length to have words grouped by number of letters
		level.subWords = SortWordsByLength (level.subWords).ToList ();

		//the coroutine is complete, so call SubWordSearchComplete()
		SubWordSearchComplete ();
	}

	public static IEnumerable<string> SortWordsByLength(IEnumerable<string> e) {
		var sorted = from s in e
			orderby s.Length ascending
				select s;
		return sorted;
	}

	public void SubWordSearchComplete() {
		mode = GameMode.levelPrep;
		Layout (); //call the Layout() function after SubWordSearchComplete
	}

	void Layout() {
		//place the letters for each subword of currLevel on screen
		wyrds = new List<Wyrd> ();

		//declare a lot of variables that will be used in this method
		GameObject go;
		Letter lett;
		string word;
		Vector3 pos;
		float left = 0;
		float columnWidth = 3;
		char c;
		Color col;
		Wyrd wyrd;

		//determine how many rows of letters will fit on screen
		int numRows = Mathf.RoundToInt (wordArea.height / letterSize);

		//make a Wyrd of each level.subWord
		for (int i=0; i<currLevel.subWords.Count; i++) {
			wyrd = new Wyrd ();
			word = currLevel.subWords [i];

			//if the word is longer than columnWidth, expand it
			columnWidth = Mathf.Max (columnWidth, word.Length);

			//instantiate a prefabLetter for each letter of the word
			for (int j=0; j<word.Length; j++) {
				c = word [j]; //grab the jth char of the word
				go = Instantiate (prefabLetter) as GameObject;
				lett = go.GetComponent<Letter> ();
				lett.c = c; //set the c of the Letter
				//position the Letter
				pos = new Vector3 (wordArea.x + left + j * letterSize, wordArea.y, 0);
				//the modulus here makes multiple columns line up
				pos.y -= (i % numRows) * letterSize;
				lett.pos = pos;
				go.transform.localScale = Vector3.one * letterSize;
				wyrd.Add (lett);
			}

			if (showAllWyrds)
				wyrd.visible = true; //this line is for testing
			wyrds.Add (wyrd);

			//if we've gotten to the numRows(th) row, start a new column
			if (i % numRows == numRows - 1) {
				left += (columnWidth + 0.5f) * letterSize;
			}
		}

		//place the big letters
		//initialize the List<>s for big letters
		bigLetters = new List<Letter> ();
		bigLettersActive = new List<Letter> ();

		//create a big letter for each letter in the target word
		for (int i = 0; i<currLevel.word.Length; i++) {
			//this is similar to the process for a normal letter
			c = currLevel.word [i];
			go = Instantiate (prefabLetter) as GameObject;
			lett = go.GetComponent<Letter> ();
			lett.c = c;
			go.transform.localScale = Vector3.one * bigLetterSize;

			//set the initial position of the big letters below screen
			pos = new Vector3 (0, -100, 0);
			lett.pos = pos;

			col = bigColorDim;
			lett.color = col;
			lett.visible = true;
			lett.big = true;
			bigLetters.Add (lett);
		}
		//shuffle the big Letters
		bigLetters = ShuffleLetters (bigLetters);
		//arrange them on screen
		ArrangeBigLetters ();

		//set the mode to be in-game
		mode = GameMode.inLevel;
	}

	//this shuffles the list randomly and returns the result
	List<Letter> ShuffleLetters(List<Letter> letts) {
		List<Letter> newL = new List<Letter> ();
		int ndx;
		while (letts.Count > 0) {
			ndx = Random.Range (0, letts.Count);
			newL.Add (letts [ndx]);
			letts.RemoveAt (ndx);
		}
		return(newL);
	}

	//this arranges the big Letters on screen
	void ArrangeBigLetters() {
		//the halfwidth allows the big Letters to be centered
		float halfWidth = ((float)bigLetters.Count) / 2f - 0.5f;
		Vector3 pos;
		for (int i = 0; i<bigLetters.Count; i++) {
			pos = bigLetterCenter;
			pos.x += (i - halfWidth) * bigLetterSize;
			bigLetters [i].pos = pos;
		}
		//bigLettersActive
		halfWidth = ((float)bigLettersActive.Count) / 2f - 0.5f;
		for (int i=0; i<bigLettersActive.Count; i++) {
			pos = bigLetterCenter; 
			pos.x += (i - halfWidth) * bigLetterSize;
			pos.y += bigLetterSize * 1.25f;
			bigLettersActive [i].pos = pos;
		}
	}	
}
