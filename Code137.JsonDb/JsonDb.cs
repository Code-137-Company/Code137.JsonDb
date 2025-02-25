using Code137.JsonDb.Attributes;
using Code137.JsonDb.Models;
using Code137.JsonDb.Security;
using Newtonsoft.Json;

namespace Code137.JsonDb
{
    public class JsonDb
    {
        public DatabaseOptions Options { get; private set; }

        private readonly Cryptograph _cryptograph;
        private List<Type> enabledTypes = new List<Type>();

        public JsonDb(DatabaseOptions options = default)
        {
            if (!Path.Exists(options.DatabasePath))
                throw new Exception("Path does not exist.");

            Options = options;

            if (!Path.Exists(Options.FilesPath))
                Directory.CreateDirectory(Options.FilesPath);

            _cryptograph = string.IsNullOrEmpty(Options.Password) ? default : new Cryptograph(Options.Password);
        }

        #region Internal Methods
        private string GetEntityName<T>()
        {
            var attributeClass = typeof(T).GetCustomAttributes(typeof(JsonDbEntity), false).FirstOrDefault();

            string entityName = typeof(T).Name;

            if (typeof(T) is AbstractEntity)
                throw new Exception($"The {entityName} class needs to inherit from the AbstractEntity.");

            if (attributeClass != default)
                entityName = ((JsonDbEntity)attributeClass).EntityName.ToString();

            return entityName;
        }

        private bool ValidateEntity<T>(out string message)
        {
            message = string.Empty;

            var entityName = GetEntityName<T>();

            string entityFile = Path.Combine(Options.FilesPath, $"{entityName}.jsondb");

            if (!enabledTypes.Any(x => x == typeof(T)) || !File.Exists(entityFile))
            {
                message = $"The {entityName} class is not defined with the JsonDb entity. Use <JsonDb>.AddEntity<Entity>().";

                return false;
            }

            return true;
        }

        private string StartEntityFile<T>()
        {
            if (!ValidateEntity<T>(out string message))
                throw new Exception($"{message}");

            var entityName = GetEntityName<T>();

            string entityFile = Path.Combine(Options.FilesPath, $"{entityName}.jsondb");

            return entityFile;
        }
        #endregion

        public void AddEntity<T>()
        {
            var entityName = GetEntityName<T>();

            if (!Path.Exists(Options.FilesPath))
                throw new Exception("Files path does not exist.");

            string entityFile = Path.Combine(Options.FilesPath, $"{entityName}.jsondb");

            if (!File.Exists(entityFile))
                File.Create(entityFile).Close();

            enabledTypes.Add(typeof(T));
        }

        #region JsonDb Read/Write
        private IEnumerable<T> ReadJsonDb<T>(string entityFile)
        {
            try
            {
                var content = File.ReadAllText(entityFile);

                if (_cryptograph != default)
                    content = _cryptograph.Decrypt(content);

                return JsonConvert.DeserializeObject<IEnumerable<T>>(content) ?? new List<T>();
            }
            catch
            {
                throw new Exception("Unable to read entity data.");
            }
        }

        private void WriteJsonDb<T>(string entityFile, IEnumerable<T> entities)
        {
            try
            {
                var content = JsonConvert.SerializeObject(entities);

                if (_cryptograph != default)
                    content = _cryptograph.Encrypt(content);

                File.WriteAllText(entityFile, content);
            }
            catch
            {
                throw new Exception("Unable to write entity data.");
            }
        }
        #endregion

        #region Read
        public IEnumerable<T> GetAll<T>() where T : AbstractEntity
        {
            string entityFile = StartEntityFile<T>();

            var entities = ReadJsonDb<T>(entityFile);

            return entities;
        }

        public IEnumerable<T> Get<T>(Func<T, bool> func) where T : AbstractEntity
        {
            string entityFile = StartEntityFile<T>();

            var entities = ReadJsonDb<T>(entityFile);

            var result = entities.Where(func);

            return result;
        }

        public T GetOne<T>(Func<T, bool> func) where T : AbstractEntity
        {
            string entityFile = StartEntityFile<T>();

            var entities = ReadJsonDb<T>(entityFile);

            var entity = entities.FirstOrDefault(func);

            return entity;
        }

        public T GetById<T>(Guid id) where T : AbstractEntity
        {
            string entityFile = StartEntityFile<T>();

            var entities = ReadJsonDb<T>(entityFile);

            var entity = entities.FirstOrDefault(x => x.Id == id);

            return entity;
        }
        #endregion

        #region Create
        public bool Insert<T>(T entity, out string message) where T : AbstractEntity
        {
            message = string.Empty;

            string entityFile = StartEntityFile<T>();

            var entities = GetAll<T>().ToList();

            var properties = entity.GetType().GetProperties();

            foreach (var property in properties)
            {
                var jsonProperty = (JsonDbProperty)property.GetCustomAttributes(typeof(JsonDbProperty), false).FirstOrDefault();

                if (jsonProperty == default)
                    continue;

                if (jsonProperty.NotNull && property.GetValue(entity) == null)
                {
                    message = $"Property {property.Name} cannot be null.";

                    return false;
                }

                if (jsonProperty.Unique)
                {
                    var props = entities.Select(x => x.GetType().GetProperty(property.Name));

                    if (props.Any(x => x.GetValue(entity) == property.GetValue(entity)))
                    {
                        message = $"The {property.Name} property is a unique property.";

                        return false;
                    }
                }
            }

            if (entity.Id == default)
                entity.Id = Guid.NewGuid();

            entities.Add(entity);

            WriteJsonDb(entityFile, entities);

            return true;
        }
        #endregion

        #region Update
        public bool Update<T>(T entity, out string message) where T : AbstractEntity
        {
            message = string.Empty;

            string entityFile = StartEntityFile<T>();

            var entities = GetAll<T>().ToList();

            if (entities.All(x => x.Id != entity.Id))
            {
                message = $"Entity Id not found. {entity.Id}";

                return false;
            }

            entities.Remove(entities.First(x => x.Id == entity.Id));
            entities.Add(entity);

            WriteJsonDb(entityFile, entities);

            return true;
        }

        public bool Update<T>(Guid id, Action<T> action, out string message) where T : AbstractEntity
        {
            message = string.Empty;

            string entityFile = StartEntityFile<T>();

            var entities = GetAll<T>().ToList();

            if (entities.All(x => x.Id != id))
            {
                message = $"Entity Id not found. {id}";

                return false;
            }

            var entity = entities.First(x => x.Id == id);

            action(entity);

            WriteJsonDb(entityFile, entities);

            return true;
        }
        #endregion

        #region Delete
        public bool Delete<T>(Guid id, out string message) where T : AbstractEntity
        {
            message = string.Empty;

            string entityFile = StartEntityFile<T>();

            var entities = GetAll<T>().ToList();

            var entity = entities.FirstOrDefault(x => x.Id == id);

            if (entity == default)
            {
                message = "Entity not found";

                return false;
            }

            entities.Remove(entity);

            WriteJsonDb(entityFile, entities);

            return true;
        }
        #endregion
    }
}
