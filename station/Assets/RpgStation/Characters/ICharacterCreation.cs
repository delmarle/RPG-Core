
namespace Station
{
    
    public interface ICharacterCreation
    {
        void Init(RpgStation station);
        bool HasData();
        void StartSequence();
        void DrawEditor();
    }
}

