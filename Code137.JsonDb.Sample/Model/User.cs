using Code137.JsonDb.Attributes;
using Code137.JsonDb.Models;

namespace Code137.JsonDb.Sample.Model
{
    [JsonDbEntity("Users")]
    public class User : AbstractEntity
    {
        [JsonDbProperty(notNull: true)]
        public string Name { get; set; }
        [JsonDbProperty()]
        public int Age { get; set; }
        [JsonDbProperty(unique: true)]
        public string Email { get; set; }
    }
}
