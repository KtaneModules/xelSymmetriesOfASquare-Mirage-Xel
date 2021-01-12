using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rnd = UnityEngine.Random;
using KModkit;
public class SymmetriesOfASquare : MonoBehaviour {
	public KMSelectable[] square;
	public SpriteRenderer[] chosenSymmetries;
	public Sprite[] symmetries;
	int[][] symmetryOperations = new int[][] {
		new int[] {0,1,2,3},
		new int[] {3,0,1,2},
		new int[] {2,3,0,1},
		new int[] {1,2,3,0},
        new int[] {3,2,1,0},
        new int[] {2,1,0,3},
        new int[] {1,0,3,2},
		new int[] {0,3,2,1}
	};
	int[] squareState = new int[] { 0, 1, 2, 3 };
	int[] chosenSymmetryIndices = new int[3];
	int stage;
	public KMBombModule module;
	public KMAudio sound;
	string[] symmetryNames = new string[] {"Identity", "Rotate 90", "Rotate 180", "Rotate -90", "Reflect Horizontal", "Reflect Right Diagonal", "Reflect Vertical", "Reflect Left Diagonal"};
	string[] chosenSymmetryNames = new string[3];
	int moduleId;
	static int moduleIdCounter = 1;
	bool solved;

    void Awake()
	{
		moduleId = moduleIdCounter++;
		foreach (KMSelectable i in square)
		{
			KMSelectable butterfly = i;
			butterfly.OnInteract += delegate { PressSquare(butterfly); return false;};
		}

	}
	void Start () {
		int[] squareStateTemp = new int[4];
		for (int i = 0; i < 3; i++)  {
			chosenSymmetryIndices[i] = rnd.Range (0, 8);
			chosenSymmetries [i].sprite = symmetries [chosenSymmetryIndices [i]];
			chosenSymmetryNames [i] = symmetryNames [chosenSymmetryIndices [i]];
			squareState.CopyTo(squareStateTemp, 0);
			int index = 0;
			foreach (int j in symmetryOperations[chosenSymmetryIndices[i]])
				{
				squareState [index] = squareStateTemp [j];
				index++;
				}
		}
		Debug.LogFormat("[Symmetries Of A Square #{0}] The symmetries on the module in order are {1}.", moduleId, chosenSymmetryNames.Join(", "));
		Debug.LogFormat("[Symmetries Of A Square #{0}] The order in which the vertices should be pressed are {1}, {2}, {3}, {4}.", moduleId, squareState[0] + 1, squareState[1] + 1, squareState[2] + 1, squareState[3] + 1);
	}


	void PressSquare (KMSelectable butterfly) {

			if (!solved)

			{

				butterfly.AddInteractionPunch();

			Debug.LogFormat("[Symmetries Of A Square #{0}] You pressed {1}.", moduleId, Array.IndexOf(square, butterfly) + 1);

			if (Array.IndexOf(square, butterfly) != squareState[stage])
				{
				module.HandleStrike();
				Debug.LogFormat("[Symmetries Of A Square #{0}] That was incorrect. Strike!", moduleId);
				stage = 0;
				squareState = new int[] { 0, 1, 2, 3 };
				Start();
				}   

				else if (stage == square.Length - 1)
				{
					module.HandlePass();
				Debug.LogFormat("[Symmetries Of A Square #{0}] That was correct. Module solved.", moduleId);
					sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
					solved = true;
				}

				else
				{
					stage++;
				}
	}
	}

	#pragma warning disable 414

	private string TwitchHelpMessage = "use '!{0} 1234' to press the vertices of the square in order.";

	#pragma warning restore 414

	IEnumerator ProcessTwitchCommand(string command)

	{

		command = command.ToLowerInvariant();

		string validcmds = "1234";

		if (command.Contains(' '))

		{

			yield return "sendtochaterror @{0}, invalid command.";

			yield break;

		}

		else

		{

			for (int i = 0; i < command.Length; i++)

			{

				if (!validcmds.Contains(command[i]))

				{

					yield return "sendtochaterror Invalid command.";

					yield break;

				}

			}

			for (int i = 0; i < command.Length; i++)

			{

				for (int j = 0; j < 4; j++)

				{

					if (command[i] == validcmds[j])

					{

						yield return null;

						square[j].OnInteract();

					}

				}

			}

		}

	}

	IEnumerator TwitchHandleForcedSolve()

	{

		foreach (int i in squareState)

		{


					yield return null;

					square[i].OnInteract();

		}
			

		}
		

}

