namespace RMFileNameFixer.Service.Interface
{
    public interface ISystemWrapper
    {
        void WriteLine(string message);
        ConsoleKeyInfo ReadKey();
        string GetCulture();
        bool DirectoryExists(string directoryName);
        string[] GetFiles(string directoryName);
        void CreateFile(string path, string content);
        void MoveFile(string oldName, string newName);
        bool FileExist(string path);
        void DeleteFile(string path);
        string Serialize<T>(T obj);
        T? Deserialize<T>(string path);
    }
}