namespace Code137.JsonDb.Models
{
    public class DatabaseOptions
    {
        public string DatabaseName { get; private set; }
        public string DatabasePath { get; private set; }
        public string FilesPath { get; private set; }
        public string Password { get; private set; }

        public DatabaseOptions(string databaseName)
        {
            DatabaseName = databaseName;
            DatabasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            FilesPath = Path.Combine(DatabasePath, DatabaseName);
            Password = string.Empty;
        }

        public DatabaseOptions(string databaseName, string path = "", string password = "")
        {
            DatabaseName = databaseName;
            DatabasePath = string.IsNullOrEmpty(path) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data") : path;
            FilesPath = Path.Combine(DatabasePath, DatabaseName);
            Password = password;
        }
    }
}
