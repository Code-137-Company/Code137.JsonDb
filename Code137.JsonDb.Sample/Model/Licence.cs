using Code137.JsonDb.Attributes;
using Code137.JsonDb.Models;

namespace Code137.JsonDb.Sample.Model
{
    public class Licence : AbstractEntity
    {
        [JsonDbProperty(unique: true, notNull: false)]
        public string Name { get; set; }
        public int Days { get; set; }
    }
}
