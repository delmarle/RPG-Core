using System.Collections.Generic;

namespace Station
{
    public class SoundEventPool
    {
        private readonly List<SoundEvent> _pool;
        private readonly int _size;
            
        public SoundEventPool (int poolSize)
        {
            _size = poolSize;
                
            _pool = new List<SoundEvent> ();
            for (var i = 0; i < _size; ++i) 
            {
                _pool.Add(new SoundEvent());
            }
        }
            
        public SoundEvent Spawn ()
        {
            if (_pool.Count <= 0) return new SoundEvent();
                
            var instance = _pool[0];
            _pool.RemoveAt(0);
                    
            return instance;

        }
            
        public void Despawn (SoundEvent soundEvent)
        {
            if (_pool.Count < _size)
            {
                _pool.Add(soundEvent);
            }
        }
    }
}
