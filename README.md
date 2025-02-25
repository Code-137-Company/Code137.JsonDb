# Code137.JsonDb
* Files in the json standard being used as a local and simple database.

# Requirements
* .NET Core v8.0 +

# How To Import?
**NuGet**
* Access the NuGet package manager in your project
* Click Search
* Search for Code137.JsonDb
* Install the latest version of the library

**Command Line**
* Access the Package Manager Console
* Type the command ```Install-Package Code137.JsonDb -Version 1.0.2```

**.NET CLI**
* Type the command ```dotnet add package Code137.JsonDb --version 1.0.2```

**NuGet ORG**
* [Nuget](https://www.nuget.org/packages/Code137.JsonDb/1.0.1)

# How It Works?
**Description**
* This library was developed to create .jsondb files that are normal json's but is already prepared to treat and deal with them as if it were a database, in addition to the encryption itself that the .jsondb file may or may not use depending on the configuration defined (with password/no password).
* The use of this library is similar to the entity framework, however it is worth highlighting that this library was not designed to use this type of data storage for large quantities nor with performance in mind. This library aims to facilitate the storage of data in files, both in the creation, manipulation and security of data.

**JsonDbEntity Attribute**
* entityName: Entity name in the database.

**JsonDbProperty Attribute**
* unique: Makes the property mandatory.
* notNull: It makes the property unique, not allowing identical records in the database.

**Example:**
* User Model in code
```
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

```

* Code in Program.Main
```
using Code137.JsonDb.Models;
using Code137.JsonDb.Sample.Model;

namespace Code137.JsonDb.Sample;

public class Program
{
    public static void Main(string[] args)
    {
        var jsonDb = new JsonDb(new DatabaseOptions("MyDatabase", path: AppDomain.CurrentDomain.BaseDirectory, password: "123456"));

        jsonDb.AddEntity<User>();

        var user = new User()
        {
            Name = "Willian",
            Age = 23,
            Email = "teste@gmail.com"
        };

        jsonDb.Insert(user, out string outputMessage);

        user = jsonDb.GetById<User>(user.Id);

        jsonDb.Update<User>(user.Id, x => x.Age = 24, out outputMessage);

        jsonDb.Delete<User>(user.Id, out string messageDelete);
    }
}

```
