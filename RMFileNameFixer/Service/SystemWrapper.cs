using System.Globalization;
using System.Text;
using System.Text.Json;

using RMFileNameFixer.Service.Interface;

namespace RMFileNameFixer.Service
{
    public class SystemWrapper : ISystemWrapper
    {
        public SystemWrapper()
        {
            Console.OutputEncoding = Encoding.UTF8;
        }

        public void CreateFile(string path, string content)
        {
            var directory = Path.GetDirectoryName(path);
            if (directory != null)
            {
                Directory.CreateDirectory(directory);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                File.WriteAllText(path, content);
            }
        }

        public bool DirectoryExists(string directoryName)
        {
            return Directory.Exists(directoryName);
        }

        public string GetCulture()
        {
            return CultureInfo.CurrentUICulture.Name;
        }

        public string[] GetFiles(string directoryName)
        {
            return Directory.GetFiles(directoryName);
        }

        public bool FileExist(string path)
        {
            return File.Exists(path);
        }

        public void MoveFile(string oldName, string newName)
        {
            if(File.Exists(oldName))
            {
                File.Move(oldName, newName);
            }
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void DeleteFile(string path)
        {
            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public T? Deserialize<T>(string path)
        {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path));
        }
    }
}
