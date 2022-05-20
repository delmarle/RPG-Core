namespace Station
{
    public class IdGenerator
    {
        private int _lastUsedId;
            
        public IdGenerator ()
        {
            _lastUsedId = 0;
        }
            
        public int Generate ()
        {   
            _lastUsedId++;
            if (_lastUsedId >= int.MaxValue)  _lastUsedId = 0;
              
            return _lastUsedId;
        }
    }
}


