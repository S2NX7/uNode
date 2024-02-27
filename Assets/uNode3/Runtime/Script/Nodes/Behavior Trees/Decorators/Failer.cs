﻿using UnityEngine;
using System.Collections;

namespace MaxyGames.UNode.Nodes {
	[NodeMenu("Behavior Tree.Decorators", "Failer", scope = NodeScope.StateGraph, IsCoroutine = true)]
	[Description("Always failure regardless of whether the targetNode success or failure.")]
	public class Failer : CoroutineNode {
		protected override bool AutoExit => false;

		protected override IEnumerator OnExecutedCoroutine(Flow flow) {
			if(!exit.isConnected) {
				yield break;
			}
			flow.TriggerCoroutine(exit, out var wait, out var jump);
			if(wait != null)
				yield return wait;
			var js = jump();
			if(js != null) {
				flow.jumpStatement = js;
				yield break;
			}
			yield return false;
		}

		public override void OnGeneratorInitialize() {
			//Register this node as state node, because this is coroutine node with state.
			CG.RegisterAsStateFlow(enter);
			CG.SetStateInitialization(enter, () => CG.GeneratePort(enter));
			if(exit.isAssigned) {
				CG.RegisterAsStateFlow(exit.GetTargetFlow());
			}
			CG.RegisterPort(enter, () => {
				if(!exit.isAssigned)
					throw new System.Exception("Exit is not assigned");
				return CG.New(typeof(Runtime.Failer), CG.GetEvent(exit));
			});
		}
	}
}
