using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using rnd = UnityEngine.Random;

public class SymmetriesOfASquare : MonoBehaviour {

	public KMSelectable[] square;
	public SpriteRenderer[] chosenSymmetries;
	public Material[] symmetryMats;
	public Sprite[] symmetries;
	int[] squareState = new int[] { -1, -1, -1, -1 };
	int[] chosenSymmetryIndices = new int[3];
	int stage;
	public KMBombModule module;
	public KMAudio sound;
	string[] symmetryNames = new string[] {"Identity", "Reflect Horizontal", "Reflect Right Diagonal", "Reflect Vertical", "Reflect Left Diagonal", "Rotate 90", "Rotate 180", "Rotate -90"};
	string[] chosenSymmetryNames = new string[3];
	int moduleId;
	static int moduleIdCounter = 1;
	bool solved;

    void Awake()
	{
		moduleId = moduleIdCounter++;
		foreach (KMSelectable i in square)
		{
			KMSelectable vert = i;
			vert.OnInteract += delegate { PressSquare(vert); return false;};
		}
	}

	void Start () {
		int[] numbers = new int[] { 0, 1, 2, 3 };
		for (int i = 0; i < 3; i++)  {
			chosenSymmetryIndices[i] = rnd.Range(0, 8);
			chosenSymmetries[i].sprite = symmetries[chosenSymmetryIndices[i]];
			chosenSymmetries[i].material = symmetryMats[chosenSymmetryIndices[i]];
			chosenSymmetryNames[i] = symmetryNames[chosenSymmetryIndices[i]];
			int temp1, temp2;
			switch (chosenSymmetryIndices[i])
            {
				case 1:
					temp1 = numbers[0];
					temp2 = numbers[1];
					numbers[0] = numbers[3];
					numbers[1] = numbers[2];
					numbers[3] = temp1;
					numbers[2] = temp2;
					break;
				case 2:
					temp1 = numbers[0];
					numbers[0] = numbers[2];
					numbers[2] = temp1;
					break;
				case 3:
					temp1 = numbers[0];
					temp2 = numbers[3];
					numbers[0] = numbers[1];
					numbers[3] = numbers[2];
					numbers[1] = temp1;
					numbers[2] = temp2;
					break;
				case 4:
					temp1 = numbers[1];
					numbers[1] = numbers[3];
					numbers[3] = temp1;
					break;
				case 5:
					for (int j = 0; j < 4; j++)
					{
						numbers[j]++;
						if (numbers[j] > 3)
							numbers[j] -= 4;
					}
					break;
				case 6:
					for (int j = 0; j < 4; j++)
					{
						numbers[j] -= 2;
						if (numbers[j] < 0)
							numbers[j] += 4;
					}
					break;
				case 7:
					for (int j = 0; j < 4; j++)
					{
						numbers[j]--;
						if (numbers[j] < 0)
							numbers[j] += 4;
					}
					break;
				default:
					break;
            }
		}
		for (int i = 0; i < 4; i++)
			squareState[i] = Array.IndexOf(numbers, i);
		Debug.LogFormat("[Symmetries Of A Square #{0}] The symmetries on the module in order are {1}.", moduleId, chosenSymmetryNames.Join(", "));
		Debug.LogFormat("[Symmetries Of A Square #{0}] The order in which the vertices should be pressed are {1}, {2}, {3}, {4}.", moduleId, squareState[0] + 1, squareState[1] + 1, squareState[2] + 1, squareState[3] + 1);
	}


	void PressSquare (KMSelectable vertex) {
		if (!solved)
		{
			vertex.AddInteractionPunch();
			Debug.LogFormat("[Symmetries Of A Square #{0}] You pressed {1}.", moduleId, Array.IndexOf(square, vertex) + 1);
			if (Array.IndexOf(square, vertex) != squareState[stage])
			{
				module.HandleStrike();
				Debug.LogFormat("[Symmetries Of A Square #{0}] That was incorrect. Strike!", moduleId);
				stage = 0;
				squareState = new int[] { -1, -1, -1, -1 };
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
	private string TwitchHelpMessage = "Use '!{0} 1234' to press the vertices of the square in order.";
	#pragma warning restore 414

	IEnumerator ProcessTwitchCommand(string command)
	{
		command = command.ToLowerInvariant();
		string validcmds = "1234";
		if (command.Contains(' '))
		{
			yield return "sendtochaterror Invalid command.";
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
						square[j].OnInteract();
						yield return new WaitForSeconds(0.1f);
					}
				}
			}
		}
	}

	IEnumerator TwitchHandleForcedSolve()
	{
		int start = stage;
		for (int i = start; i < 4; i++)
		{
			square[squareState[i]].OnInteract();
			yield return new WaitForSeconds(0.1f);
		}	
	}		
}