namespace Quantum
{
    using Photon.Deterministic;

    public class ChangeMapCommand : DeterministicCommand
    {
        public AssetRef<Map> MapRef;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref MapRef);
        }

        public void Execute(Frame frame)
        {
            if (frame.IsVerified && MapRef.Id.IsValid)
            {
                frame.Map = frame.FindAsset(MapRef);
            }
        }
    }
}
