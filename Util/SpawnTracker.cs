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
        /// Destroys all tracked <see cref="GameObject"/>s and unloads all tracked
        /// <see cref="LazyActor"/>s, then clears both lists.
        /// </summary>
        public void DespawnAll()
        {
            foreach (var go in _gameObjects)
                if (go != null) UnityEngine.Object.Destroy(go);
            _gameObjects.Clear();

            foreach (var lazy in _lazyActors)
                if (lazy != null && !lazy.IsDestroyed) UnityEngine.Object.Destroy(lazy);
            _lazyActors.Clear();
        }
    }
}
