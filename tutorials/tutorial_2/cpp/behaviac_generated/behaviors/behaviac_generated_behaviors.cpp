﻿// ---------------------------------------------------------------------
// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!
// ---------------------------------------------------------------------

#include "behaviac/behaviac.h"

#include "behaviac_generated_behaviors.h"

namespace behaviac
{
	class CppGenerationManager : GenerationManager
	{
	public:
		CppGenerationManager()
		{
			SetInstance(this);
		}

		virtual void RegisterBehaviorsImplement()
		{
			Workspace::GetInstance()->RegisterBehaviorTreeCreator("LoopBT", bt_LoopBT::Create);
			Workspace::GetInstance()->RegisterBehaviorTreeCreator("SelectBT", bt_SelectBT::Create);
			Workspace::GetInstance()->RegisterBehaviorTreeCreator("SequenceBT", bt_SequenceBT::Create);
		}
	};

	CppGenerationManager _cppGenerationManager_;
}
