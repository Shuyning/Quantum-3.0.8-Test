using Photon.Deterministic;

namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class MapChangeSystem : SystemMainThread, ISignalOnMapChanged
    {
        public override void Update(Frame frame)
        {
            for (int i = 0; i < frame.PlayerCount; i++)
            {
                var command = frame.GetPlayerCommand(i) as ChangeMapCommand;
                command?.Execute(frame);
            }
        }

        public void OnMapChanged(Frame frame, AssetRef<Map> previousMap)
        {
            foreach (var pair in frame.GetComponentIterator<PlayerController>())
            {
                if (frame.Unsafe.TryGetPointer<Transform3D>(pair.Entity, out var transform))
                {
                    transform->Position = FPVector3.Zero;
                    transform->Position.Y = FP._2;
                }
            }
        }
    }
}
