namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class GameplaySpawnSystem : SystemSignalsOnly, ISignalOnPlayerAdded, ISignalOnPlayerRemoved
    {
        void ISignalOnPlayerAdded.OnPlayerAdded(Frame frame, PlayerRef playerRef, bool firstTime)
        {
            var runtimePlayer = frame.GetPlayerData(playerRef);
            var playerEntity = frame.Create(runtimePlayer.PlayerAvatar);

            if (frame.Unsafe.TryGetPointer<PlayerController>(playerEntity, out var playerController))
            {
                playerController->PlayerRef = playerRef;
            }

            var playerControllerEntity = GetPlayerEntity(frame, playerRef);
            if (playerControllerEntity == EntityRef.None)
                return;

            int desiredOrder = 0;
            var pcFilter = frame.Filter<PlayerController>();
            int existingPlayers = 0;
            while (pcFilter.NextUnsafe(out _, out _)) 
                existingPlayers++;
            
            desiredOrder = existingPlayers - 1;
            if (desiredOrder < 0) desiredOrder = 0;

            Transform3D* chosenSpawn = null;
            var spFilter = frame.Filter<SpawnPoint, Transform3D>();
            while (spFilter.NextUnsafe(out _, out var sp, out var tr))
            {
                if (sp->Order == desiredOrder)
                {
                    chosenSpawn = tr;
                    break;
                }
            }

            if (chosenSpawn == null)
            {
                var spAny = frame.Filter<SpawnPoint, Transform3D>();
                if (spAny.NextUnsafe(out _, out _, out var trAny))
                    chosenSpawn = trAny;
            }

            if (chosenSpawn != null && frame.Unsafe.TryGetPointer<Transform3D>(playerControllerEntity, out var t))
            {
                t->Position = chosenSpawn->Position;
                t->Rotation = chosenSpawn->Rotation;
            }
        }

        void ISignalOnPlayerRemoved.OnPlayerRemoved(Frame frame, PlayerRef playerRef)
        {
            var playerEntity = GetPlayerEntity(frame, playerRef);
            if (playerEntity != EntityRef.None)
            {
                frame.Destroy(playerEntity);
            }
        }

        private EntityRef GetPlayerEntity(Frame frame, PlayerRef playerRef)
        {
            foreach (var pair in frame.GetComponentIterator<PlayerController>())
            {
                if (pair.Component.PlayerRef == playerRef)
                    return pair.Entity;
            }
            return EntityRef.None;
        }
    }
}
