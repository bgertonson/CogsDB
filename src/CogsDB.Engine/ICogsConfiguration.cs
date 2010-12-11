namespace CogsDB.Engine
{
    public interface ICogsConfiguration
    {
        string ConnectionName { get; set; }
        CogsSessionManagementStrategy SessionManagementStrategy { get; set; }
    }

    public class CogsConfiguration: ICogsConfiguration
    {
        public string ConnectionName { get; set; }

        public CogsSessionManagementStrategy SessionManagementStrategy { get; set; }
    }
}