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

	public bool ____________;

	public GameMode mode = GameMode.preGame;
	public WordLevel currLevel;

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
	}
	
}
