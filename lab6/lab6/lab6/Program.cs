using System;
using System.IO;

// Делегат для сповіщення про зміни файлів
public delegate void FileMonitorDelegate(string filePath);

// Клас подій для файлового монітора
public class FileMonitor
{
    // Події для різних файлових операцій
    public event FileMonitorDelegate OnFileCreated;
    public event FileMonitorDelegate OnFileDeleted;
    public event FileMonitorDelegate OnFileModified;
    public event FileMonitorDelegate OnFileRenamed;

    // Фільтр для типів файлів
    private string fileFilter;

    // Каталог, який буде відслідковуватися
    private readonly string directoryPath;

    private readonly FileSystemWatcher fileSystemWatcher;

    public FileMonitor(string path, string filter = "*.*")
 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    {
        directoryPath = path;
        fileFilter = filter;

        // Створення екземпляра FileSystemWatcher
        fileSystemWatcher = new FileSystemWatcher(directoryPath, fileFilter);

        // Налаштування параметрів відслідковування
        fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        fileSystemWatcher.IncludeSubdirectories = true;

        // Додавання обробників подій
        fileSystemWatcher.Created += (sender, e) => NotifySubscribers(OnFileCreated, e.FullPath);
        fileSystemWatcher.Deleted += (sender, e) => NotifySubscribers(OnFileDeleted, e.FullPath);
        fileSystemWatcher.Changed += (sender, e) => NotifySubscribers(OnFileModified, e.FullPath);
        fileSystemWatcher.Renamed += (sender, e) =>
        {
            NotifySubscribers(OnFileRenamed, e.OldFullPath);
            NotifySubscribers(OnFileCreated, e.FullPath);
        };
    }

    // Метод для додавання фільтрації за типами файлів
    public void SetFileFilter(string filter)
    {
        fileFilter = filter;
        fileSystemWatcher.Filter = fileFilter;
    }

    // Метод для початку моніторингу
    public void StartMonitoring()
    {
        fileSystemWatcher.EnableRaisingEvents = true;
    }

    // Метод для зупинки моніторингу
    public void StopMonitoring()
    {
        fileSystemWatcher.EnableRaisingEvents = false;
    }

    // Метод для сповіщення підписників подій
    private void NotifySubscribers(FileMonitorDelegate eventDelegate, string filePath)
    {
        eventDelegate?.Invoke(filePath);
    }
}

class Program
{
    static void Main()
    {
        // Створення екземпляру FileMonitor
        FileMonitor fileMonitor = new FileMonitor(@"C:\Users\Evgen\VirtualBox VMs");

        // Підписка на події за допомогою лямбда-виразів
        fileMonitor.OnFileCreated += (filePath) => Console.WriteLine($"File created: {filePath}");
        fileMonitor.OnFileDeleted += (filePath) => Console.WriteLine($"File deleted: {filePath}");
        fileMonitor.OnFileModified += (filePath) => Console.WriteLine($"File modified: {filePath}");
        fileMonitor.OnFileRenamed += (filePath) => Console.WriteLine($"File renamed: {filePath}");

        // Запуск моніторингу
        fileMonitor.StartMonitoring();

        Console.WriteLine("File monitoring started. Press Enter to exit.");
        Console.ReadLine();

        // Зупинка моніторингу при виході
        fileMonitor.StopMonitoring();
    }
}
