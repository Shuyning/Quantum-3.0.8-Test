namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class GameplaySpawnSystem : SystemSignalsOnly, ISignalOnPlayerAdded, ISignalOnPlayerRemoved
    {
        void ISignalOnPlayerAdded.OnPlayerAdded(Frame frame, PlayerRef playerRef, bool firstTime)
        {
            if (!firstTime)
                return;

            var playerEntity = CreatePlayer(frame, playerRef);
            PlacePlayerOnSpawnPosition(frame, playerEntity);
        }

        private EntityRef CreatePlayer(Frame frame, PlayerRef playerRef)
        {
            var playerData = frame.GetPlayerData(playerRef);
            var entityPrototype = frame.FindAsset<EntityPrototype>(playerData.PlayerAvatar);
            var playerEntity = frame.Create(entityPrototype);
            
            frame.AddOrGet<PlayerController>(playerEntity, out var playerController);
            playerController->PlayerRef = playerRef;

            return playerEntity;
        }

        private void PlacePlayerOnSpawnPosition(Frame frame, EntityRef playerEntity)
        {
            if (playerEntity == EntityRef.None)
                return;

            int desiredOrder = 0;
            var playerFilter = frame.Filter<PlayerController>();
            int existingPlayers = 0;
            while (playerFilter.NextUnsafe(out _, out _)) 
                existingPlayers++;
            
            desiredOrder = existingPlayers - 1;
            if (desiredOrder < 0)
                desiredOrder = 0;

            Transform3D* chosenSpawn = null;
            var spawnPointFilter = frame.Filter<SpawnPoint, Transform3D>();
            while (spawnPointFilter.NextUnsafe(out _, out var spawnPoint, out var transform))
            {
                if (spawnPoint->Order == desiredOrder)
                {
                    chosenSpawn = transform;
                    break;
                }
            }

            if (chosenSpawn == null)
            {
                var spawnPoint = frame.Filter<SpawnPoint, Transform3D>();
                if (spawnPoint.NextUnsafe(out _, out _, out var trAny))
                    chosenSpawn = trAny;
            }

            if (chosenSpawn != null && frame.Unsafe.TryGetPointer<Transform3D>(playerEntity, out var transform3D))
            {
                transform3D->Position = chosenSpawn->Position;
                transform3D->Rotation = chosenSpawn->Rotation;
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
