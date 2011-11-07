using UnityEngine;
using System.Collections.Generic;

public class LobbyPlayerSetup: MonoBehaviour {

	public float marginX = 340;
	public float marginY = 30;
	public float width = 270;
	public float windowHeight = 320;
	public float height = 60;

	private Rect window;

	public bool showMenu = false;
	public static LobbyPlayerSetup Script;

	// http://forum.unity3d.com/threads/29058-error-with-no-side-effects
	private int innerGUIController = 0;

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * UNITY STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */
	void Awake() {

		Script = this;

		window = new Rect(marginX, marginY, width, windowHeight);
	}

	void OnGUI() {
		if(!showMenu)
			return;

		if(Event.current.type == EventType.Layout) { 

			innerGUIController = 1;
		}
		if(Event.current.type == EventType.Repaint) {

			innerGUIController = 2;
		}
		
		window = GUI.Window(6, window, ShowPlayerSetup, "");
	}

	/* -------------------------------------------------------------------------------------------------------- */
	/*
	 * MENU STUFF
	 */
	/* -------------------------------------------------------------------------------------------------------- */

	/*
	 * @brief		Show the player's name and setup 
	 * @param		nId		Unity´s window reference
	 * @return	void
	 */
	void ShowPlayerSetup(int nId) {

		int nIdx = 0;

		if(innerGUIController == 2) {
			
			innerGUIController = 0;
		
			//GUILayout.BeginVertical();
			{
				foreach(NetworkGame.PlayerInfo entry in NetworkGame.playerList) {

					GUI.Box(new Rect(5, 10 + height * nIdx, width-10, height), entry.playerName);
					nIdx++;
				}
			}
			//GUILayout.EndVertical();
		}

	}

	/*
	 * @brief		Show this menu or not
	 * @param		
	 * @return	void
	 */
	public void ShowThisMenu() {
		
		showMenu = true;
	}
}
