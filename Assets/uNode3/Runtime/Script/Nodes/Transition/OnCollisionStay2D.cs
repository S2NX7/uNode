﻿namespace MaxyGames.UNode.Transition {
	[TransitionMenu("OnCollisionStay2D", "OnCollisionStay2D")]
	public class OnCollisionStay2D : TransitionEvent {
		[Filter(typeof(UnityEngine.Collision2D), SetMember = true)]
		public MemberData storeCollision = new MemberData();

		public override void OnEnter(Flow flow) {
			UEvent.Register<UnityEngine.Collision2D>(UEventID.OnCollisionStay2D, flow.target as UnityEngine.Component, (value) => Execute(flow, value));
		}

		public override void OnExit(Flow flow) {
			UEvent.Unregister<UnityEngine.Collision2D>(UEventID.OnCollisionStay2D, flow.target as UnityEngine.Component, (value) => Execute(flow, value));
		}

		void Execute(Flow flow, UnityEngine.Collision2D collision) {
			if(storeCollision.isAssigned) {
				storeCollision.Set(flow, collision);
			}
			Finish(flow);
		}

		public override string GenerateOnEnterCode() {
			if(!CG.HasInitialized(this)) {
				CG.SetInitialized(this);
				var mData = CG.generatorData.GetMethodData("OnCollisionStay2D");
				if(mData == null) {
					mData = CG.generatorData.AddMethod(
						"OnCollisionStay2D",
						typeof(void),
						typeof(UnityEngine.Collision2D));
				}
				string set = null;
				if(storeCollision.isAssigned) {
					set = CG.Set(CG.Value((object)storeCollision), mData.parameters[0].name).AddLineInEnd();
				}
				mData.AddCode(
					CG.Condition(
						"if",
						CG.CompareNodeState(node.enter, null),
						set + CG.FlowTransitionFinish(this)
					),
					this
				);
			}
			return null;
		}
	}
}
