namespace Code137.JsonDb.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class JsonDbEntity : Attribute
    {
        public string EntityName { get; set; }

        public JsonDbEntity(string entityName)
        {
            EntityName = entityName;
        }
    }
}
