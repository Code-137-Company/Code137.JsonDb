using Code137.JsonDb.Models;
using Code137.JsonDb.Sample.Model;

namespace Code137.JsonDb.Sample;

public class Program
{
    public static void Main(string[] args)
    {
        var jsonDb = new JsonDb(new DatabaseOptions("TestLib", path: AppDomain.CurrentDomain.BaseDirectory, password: "1234"));
        jsonDb.AddEntity<User>();
        jsonDb.AddEntity<Licence>();

        var users = jsonDb.GetAll<User>();

        var user = new User()
        {
            Name = "Guilherme",
            Age = 23,
            Email = "teste@gmail.com"
        };

        var licence = new Licence()
        {
            Name = "Default",
            Days = 30
        };

        jsonDb.Insert(user, out string messageInsert);
        jsonDb.Insert(licence, out messageInsert);

        user = jsonDb.GetById<User>(user.Id);

        jsonDb.Delete<User>(user.Id, out string messageDelete);

        jsonDb.Insert(user, out messageInsert);

        users = jsonDb.GetAll<User>();

        user.Age = 24;
        jsonDb.Update(user, out string messageUpdate);

        users = jsonDb.GetAll<User>();

        jsonDb.Update<User>(user.Id, (x => x.Email = "teste@teste.com"), out messageUpdate);

        users = jsonDb.GetAll<User>();
    }
}
