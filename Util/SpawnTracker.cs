using Drova_Modding_API.Access;
using Il2CppDrova.Utilities.LazyLoading;
using UnityEngine;

namespace RandomEvents.Util
{
    /// <summary>
    /// Tracks a mixed set of asset-backed <see cref="GameObject"/> spawns and
    /// lazy <see cref="LazyActor"/> bandits so event classes have a single call-site
    /// for liveness checks and cleanup.
    /// </summary>
    public class SpawnTracker
    {
        private readonly List<GameObject> _gameObjects = new();
        private readonly List<LazyActor> _lazyActors = new();

        /// <summary>Total number of currently tracked entries (not pruned).</summary>
        public int Count => _gameObjects.Count + _lazyActors.Count;

        public void Add(GameObject go)
        {
            if (go != null) _gameObjects.Add(go);
        }

        public void Add(LazyActor actor)
        {
            if (actor != null) _lazyActors.Add(actor);
        }

        /// <summary>
        /// Returns <c>true</c> if at least one tracked entity is still alive.
        /// Prunes null / destroyed entries as a side effect.
        /// </summary>
        public bool HasLive()
        {
            for (int i = _gameObjects.Count - 1; i >= 0; i--)
                if (_gameObjects[i] == null) _gameObjects.RemoveAt(i);

            for (int i = _lazyActors.Count - 1; i >= 0; i--)
            {
                var a = _lazyActors[i];
                if (a == null || a.IsDestroyed) _lazyActors.RemoveAt(i);
            }

            return _gameObjects.Count > 0 || _lazyActors.Count > 0;
        }

        /// <summary>
        /// Removes everything this tracker spawned, then clears both lists.
        ///
        /// **The lazy half used to destroy the wrong object.** <c>Object.Destroy</c> on a
        /// <see cref="LazyActor"/> destroys the component; the creature it spawned is a separate child
        /// object registered separately as an entity, and the component's own <c>OnDestroy</c> unregisters
        /// its handle without touching the body. So an event that ended after its bandits had materialised
        /// left them alive and hostile in the world for the rest of the session, while this method
        /// reported success. <see cref="ActorSpawnAccess"/> owns the correct teardown - it kills a body a
        /// player may have seen, and quietly removes a handle that never loaded.
        /// </summary>
        public void DespawnAll()
        {
            foreach (var go in _gameObjects)
                if (go != null) UnityEngine.Object.Destroy(go);
            _gameObjects.Clear();

            foreach (var lazy in _lazyActors)
                ActorSpawnAccess.TryKillOrDespawn(lazy);
            _lazyActors.Clear();
        }
    }
}
