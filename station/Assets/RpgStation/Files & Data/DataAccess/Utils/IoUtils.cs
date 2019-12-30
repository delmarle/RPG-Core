using System.IO;

public static class IoUtils
{
    public static void CreateFile(byte[] data, string path)
    {
        TryCreateFolder(path);
        var file = new FileInfo(path);

        if (file.Exists)
        {
            file.Delete();
        }
        File.WriteAllBytes(path, data);
    }
    
    public static void CreateFile(string data, string path)
    {
        TryCreateFolder(path);
        var file = new FileInfo(path);

        if (file.Exists)
        {
            file.Delete();
        }

        var writer = file.CreateText();

        writer.Write(data);
        writer.Close();
    }

    private static void TryCreateFolder(string path)
    {
        int index = path.LastIndexOf('/');
        string folderPath = path.Remove(index);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    public static void TryDeleteFolder(string path)
    {
        int index = path.LastIndexOf('/');
        string folderPath = path.Remove(index);
        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath,true);
        }
    }

    public static bool LoadFile(string path, out string data)
    {
        data = null;
        
        if (File.Exists(path))
        {
            var reader = File.OpenText(path);
            data = reader.ReadToEnd();
            reader.Close();
        }

        return data != null;
    }

    public static void DeleteFileOrDirectory(string path)
    {
        var file = new FileInfo(path);

        if (file.Exists)
        {
            file.Delete();
        }
        else
        {
            var directory = new DirectoryInfo(path);

            if (directory.Exists)
            {
                directory.Delete(true);
            }
        }
    }

    public static void DeleteFile(string path)
    {
        var file = new FileInfo(path);

        if (file.Exists)
        {
            file.Delete();
        }
    }

    public static bool IsDirectoryExist(string path)
    {
        int index = path.LastIndexOf('/');
        string folderPath = path.Remove(index);

        return Directory.Exists(folderPath);
    }

    public static bool IsFileExist(string path)
    {
        return File.Exists(path);
    }
}
