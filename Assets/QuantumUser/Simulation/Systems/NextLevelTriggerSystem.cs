using UnityEngine;

namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class NextLevelTriggerSystem : SystemSignalsOnly, ISignalOnTriggerEnter3D
    {
        public void OnTriggerEnter3D(Frame frame, TriggerInfo3D info)
        {
            if (!frame.Has<NextLevelTrigger>(info.Entity))
                return;

            if (!frame.Has<PlayerController>(info.Other))
                return;

            var trigger = frame.Get<NextLevelTrigger>(info.Entity);
            if (!trigger.NextLevelMap.Id.IsValid)
                return;

            if (frame.IsVerified)
                frame.Map = frame.FindAsset(trigger.NextLevelMap);
        }
    }
}
