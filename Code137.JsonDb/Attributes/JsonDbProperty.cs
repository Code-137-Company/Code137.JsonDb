namespace Code137.JsonDb.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class JsonDbProperty : Attribute
    {
        public bool Unique { get; private set; }
        public bool NotNull { get; private set; }

        public JsonDbProperty(bool unique = false, bool notNull = false)
        {
            Unique = unique;
            NotNull = notNull;
        }
    }
}
