using Code137.JsonDb.Attributes;

namespace Code137.JsonDb.Models
{
    public abstract class AbstractEntity
    {
        private Guid _id;

        [JsonDbProperty(unique: true, notNull: true)]
        public Guid Id
        {
            get { return _id; }
            set { _id = value == default ? Guid.NewGuid() : value; }
        }

        public AbstractEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
