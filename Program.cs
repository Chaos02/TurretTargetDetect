using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
	partial class Program : MyGridProgram {
		// This file contains your actual script.
		//
		// You can either keep all your code here, or you can create separate
		// code files to make your program easier to navigate while coding.
		//
		// In order to add a new utility class, right-click on your project, 
		// select 'New' then 'Add Item...'. Now find the 'Space Engineers'
		// category under 'Visual C# Items' on the left hand side, and select
		// 'Utility Class' in the main area. Name it in the box below, and
		// press OK. This utility class will be merged in with your code when
		// deploying your final script.
		//
		// You can also simply create a new utility class manually, you don't
		// have to use the template if you don't want to. Just do so the first
		// time to see what a utility class looks like.
		// 
		// Go to:
		// https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
		//
		// to learn more about ingame scripts.

		#region mdk preserve
		// Define if script should try find a group of turrets with <Keyword>.
		readonly bool UseTurretGroup = true;

		// Define keyword that script should look for in turret/group names.
		readonly string Keyword = "[TTD]";

		// Also turns on everything when any of the turrets are manually controlled by a player.
		readonly bool WhenUnderControl = true;

		/////////////////////DO NOT MODIFY BELOW!!!//////////////////////

		/////////////////////DO NOT MODIFY BELOW!!!//////////////////////

		/////////////////////DO NOT MODIFY BELOW!!!//////////////////////



		#endregion

		List<IMyTerminalBlock> SwitchGroup = new List<IMyTerminalBlock>();
		List<IMyLargeTurretBase> TurretGroup = new List<IMyLargeTurretBase>();
		List<IMyTurretControlBlock> CustomTurretGroup = new List<IMyTurretControlBlock>();
		bool SoundIsPlaying = false;


		//Global
		readonly string[] StatusSpinner = new string[4] { "[_]", "[\\]", "[I]", "[/]" };
		string StatusHeader;
		string StatusMessage;
		ushort StatusSpinnerC;


		public Program() {
			// The constructor, called only once every session and
			// always before any other method is called. Use it to
			// initialize your script. 
			//     
			// The constructor is optional and can be removed if not
			// needed.
			// 
			// It's recommended to set Runtime.UpdateFrequency 
			// here, which will allow your script to run itself without a 
			// timer block.

			Runtime.UpdateFrequency = UpdateFrequency.Update10 | UpdateFrequency.Update100;

			GetTargets();

		}

		public void Save() {
			// Called when the program needs to save its state. Use
			// this method to save your state to the Storage field
			// or some other means. 
			// 
			// This method is optional and can be removed if not
			// needed.
		}

		void GetTargets() {
			SwitchGroup.Clear();
			TurretGroup.Clear();
			CustomTurretGroup.Clear();

			List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
			GridTerminalSystem.GetBlockGroups(groups);
			if (UseTurretGroup) {
				foreach (var group in groups) {
					if (group.Name.Contains(Keyword) && !group.Name.Contains(Keyword + " SW")) {
						List<IMyLargeTurretBase> groupBlocks = new List<IMyLargeTurretBase>();
						List<IMyTurretControlBlock> groupCustomBlocks = new List<IMyTurretControlBlock>();
						group.GetBlocksOfType(groupBlocks);
						group.GetBlocksOfType(groupCustomBlocks);
						TurretGroup.AddList(groupBlocks);
						CustomTurretGroup.AddList(groupCustomBlocks);
					}
				}
			} else {
				GridTerminalSystem.GetBlocksOfType(TurretGroup, x => x.CustomName.Contains(Keyword));
				GridTerminalSystem.GetBlocksOfType(CustomTurretGroup, x => x.CustomName.Contains(Keyword));
			}

			foreach (var group in groups) {
				if (group.Name.Contains(Keyword + " SW")) {
					List<IMyTerminalBlock> groupBlocks = new List<IMyTerminalBlock>();
					group.GetBlocksOfType(groupBlocks);
					SwitchGroup.AddList(groupBlocks);
				}
			}
		}

		void SetLights() {

			bool TurnOn = false;
			if (WhenUnderControl) {
				foreach (var turr in TurretGroup) {
					TurnOn = TurnOn || turr.HasTarget || turr.IsUnderControl;
					if (TurnOn)
						break;
				}
				foreach (var turr in CustomTurretGroup) {
					TurnOn = TurnOn || turr.HasTarget || turr.IsUnderControl;
					if (TurnOn)
						break;
				}
			} else {
				foreach (var turr in TurretGroup) {
					TurnOn = TurnOn || turr.HasTarget;
					if (TurnOn)
						break;
				}
				foreach (var turr in CustomTurretGroup) {
					TurnOn = TurnOn || turr.HasTarget;
					if (TurnOn)
						break;
				}
			}

			if (TurnOn) {
				foreach (var block in SwitchGroup) {
					block.ApplyAction("OnOff_On");
					if (block is IMySoundBlock)
						if (!SoundIsPlaying)
							block.ApplyAction("PlaySound");
				}
				SoundIsPlaying = true;
			} else {
				foreach (var block in SwitchGroup) {
					block.ApplyAction("OnOff_Off");
					if (block is IMySoundBlock)
						block.ApplyAction("StopSound");
				}
				SoundIsPlaying = false;
			}
		}

		public void Main(string argument, UpdateType updateSource) {
			// The main entry point of the script, invoked every time
			// one of the programmable block's Run actions are invoked,
			// or the script updates itself. The updateSource argument
			// describes where the update came from. Be aware that the
			// updateSource is a  bitfield  and might contain more than 
			// one update type.
			// 
			// The method itself is required, but the arguments above
			// can be removed if not needed.

			if (StatusSpinnerC > StatusSpinner.Length - 1)
				StatusSpinnerC = 0;

			if ((updateSource & UpdateType.Terminal) != 0) {

			}
			if ((updateSource & UpdateType.Trigger) != 0) {

			}
			if ((updateSource & UpdateType.IGC) != 0) {

			}
			if ((updateSource & UpdateType.Update10) != 0) {
				SetLights();
			}
			if ((updateSource & UpdateType.Update100) != 0) {
				GetTargets();
			}

			StatusMessage = $"{SwitchGroup.Count} to switch, {TurretGroup.Count + CustomTurretGroup.Count} to check, {CustomTurretGroup.Count} of which custom.";

			StatusHeader = $"{StatusSpinner[StatusSpinnerC]} [TTD] TurretTargetDetect\n";
			StatusHeader += $"Keyword: {Keyword}\n";
			StatusSpinnerC++;
			StatusMessage = StatusHeader + StatusMessage;
			Echo(StatusMessage);
			StatusMessage = "";

		}
	}
}
