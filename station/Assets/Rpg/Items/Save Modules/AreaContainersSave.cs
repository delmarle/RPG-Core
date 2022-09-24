namespace Station
{
    public class AreaContainersSave : AreaSaveModule<ContainersListSave>
    {
        protected override void BuildDefaultData()
        {
            Value = new ContainersListSave();
        }
    }
}

