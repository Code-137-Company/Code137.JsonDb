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
