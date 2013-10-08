using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;

namespace WP7FlacPlayer.Util
{
    public class IsolatedStorageExtentions
    {
        public static void GetFile(string root,string extention,  ref List<IsolatedStorageDirectoryInfo> directoryInfos,
                                    ref List<IsolatedStorageFileInfo> fileInfos)
        {
            root = root.Replace('\\', '/');
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!storage.DirectoryExists(root))
                    throw new DirectoryNotFoundException(string.Format("Directory {0} not exists", root));
                var directorys = string.IsNullOrWhiteSpace(root)
                                     ? storage.GetDirectoryNames()
                                     : storage.GetDirectoryNames(string.Format("{0}/*", root.TrimEnd('/')));
                var files = string.IsNullOrWhiteSpace(root)
                                ? storage.GetFileNames()
                                : storage.GetFileNames(string.Format("{0}/*", root.TrimEnd('/')));
                fileInfos.AddRange(from file in files
                                   where
                                       file.EndsWith(extention.StartsWith(".")
                                                         ? extention
                                                         : string.Format(".{0}", extention)) ||
                                       string.IsNullOrWhiteSpace(extention)
                                   select
                                       new IsolatedStorageFileInfo(string.Format("{0}/{1}", root, file).TrimStart('/')));
                foreach (var directory in directorys)
                {
                    var mergedDirectory = string.Format("{0}/{1}", root, directory).TrimStart('/');
                    directoryInfos.Add(new IsolatedStorageDirectoryInfo(mergedDirectory));
                    GetFile(mergedDirectory, extention, ref directoryInfos, ref fileInfos);
                }
            }
        }

        public static void GetFileTree(string root, string extention, ref IsolatedStorageFileTree rootTree)
        {
            root = string.IsNullOrEmpty(root) ? "" : root.Replace('\\', '/');
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!storage.DirectoryExists(root))
                    throw new DirectoryNotFoundException(string.Format("Directory {0} not exists", root));
                var directorys = string.IsNullOrWhiteSpace(root)
                                     ? storage.GetDirectoryNames()
                                     : storage.GetDirectoryNames(string.Format("{0}/*", root.TrimEnd('/')));
                var files = string.IsNullOrWhiteSpace(root)
                                ? storage.GetFileNames()
                                : storage.GetFileNames(string.Format("{0}/*", root.TrimEnd('/')));
                //var node = new IsolatedStorageFileTree(ref rootTree){NodeElement = new IsolatedStorageDirectoryInfo(root)};
                //rootTree.AddChild(ref node);
                foreach (var directory in directorys)
                {
                    var mergedDirectory = string.Format("{0}/{1}", root, directory).TrimStart('/');
                    var childNode = new IsolatedStorageFileTree(ref rootTree){NodeElement = new IsolatedStorageDirectoryInfo(mergedDirectory)};
                    GetFileTree(mergedDirectory, extention, ref childNode);
                    rootTree.AddChild(ref childNode);

                }
                foreach (var file in files)
                {
                    if (file.EndsWith(extention.StartsWith(".")
                                          ? extention
                                          : string.Format(".{0}", extention)) ||
                        string.IsNullOrWhiteSpace(extention))
                    {
                        var childNode = new IsolatedStorageFileTree(ref rootTree)
                            {
                                NodeElement =
                                    new IsolatedStorageFileInfo(string.Format("{0}/{1}", root, file).TrimStart('/'))
                            };
                        rootTree.AddChild(ref childNode);
                    }
                }
            }
        }

        public static IsolatedStorageFileTree GetFileTree(string root, string extention)
        {
            var tree = new IsolatedStorageFileTree();
            GetFileTree(root, extention, ref tree);
            return tree;
        }
    }

    public interface IIsolatedStorageFile
    {
        IsolatedStorageDirectoryInfo RootDirectory { get; }

        bool IsInRootDirectory { get; }

        string FullName { get; }

        string Name { get; }
    }

    public class IsolatedStorageFileInfo:IIsolatedStorageFile
    {
        public IsolatedStorageDirectoryInfo RootDirectory { get; private set; }

        public bool IsInRootDirectory
        {
            get { return RootDirectory == null; }
        }

        public string FullName { get; private set; }
        public string Parent { get; private set; }
        public string Name { get; private set; }
        public string Extention { get; private set; }

        public IsolatedStorageFileInfo(string fullName)
        {
            fullName = string.IsNullOrEmpty(fullName) ? "" : fullName.Replace('\\', '/');
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(fullName))
                {
                    FullName = fullName.Trim('/');
                    var pNodes = FullName.Split('/');
                    if (pNodes.Length < 2)
                    {
                        Name = FullName;
                        RootDirectory = null;
                    }
                    else
                    {
                        Name = pNodes.Last();
                        for (var i = 0; i < pNodes.Length - 1; i++)
                        {
                            Parent = string.Format("{0}/{1}", Parent, pNodes[i]);
                        }
                        RootDirectory = new IsolatedStorageDirectoryInfo(Parent);
                    }
                    Extention = GetExtentionName(Name);
                }
                else
                {
                    throw new FileNotFoundException(string.Format("File {0} not exists", fullName));
                }
            }
        }

        private string GetExtentionName(string name)
        {
            var names = name.Split('.');
            return names.Length < 2 ? string.Empty : names.Last();
        }

    }

    public class IsolatedStorageDirectoryInfo:IIsolatedStorageFile
    {
        public IsolatedStorageDirectoryInfo RootDirectory { get; private set; }

        public bool IsInRootDirectory
        {
            get { return RootDirectory == null; }
        }

        public string FullName { get; private set; }
        public string Parent { get; private set; }
        public string Name { get; private set; }

        public IsolatedStorageDirectoryInfo(string fullName)
        {
            fullName = string.IsNullOrEmpty(fullName) ? "" : fullName.Replace('\\', '/');
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.DirectoryExists(fullName))
                {
                    FullName = fullName.Trim('/');
                    var pNodes = FullName.Split('/');
                    if (pNodes.Length < 2)
                    {
                        Name = FullName;
                        Parent = string.Empty;
                        RootDirectory = null;
                    }
                    else
                    {
                        Name = pNodes.Last();
                        for (var i = 0; i < pNodes.Length - 1; i++)
                        {
                            Parent += string.Format("{0}/", pNodes[i]);
                        }
                        RootDirectory = new IsolatedStorageDirectoryInfo(Parent);
                    }
                }
                else
                {
                    throw new DirectoryNotFoundException(string.Format("Directory {0} not exists", fullName));
                }
            }
        }

    }

    public class IsolatedStorageFileTree : IsolatedStorageFileNode
    {
        public IsolatedStorageFileTree Parent { get; private set; }
        public List<IsolatedStorageFileTree> Children { get; private set; }

        public bool IsRootNode
        {
            get { return Parent == null; }
        }
        
        public IsolatedStorageFileTree()
        {
            Children = new List<IsolatedStorageFileTree>();
            Parent = null;
        }

        public IsolatedStorageFileTree(ref IsolatedStorageFileTree parent)
        {
            Children=new List<IsolatedStorageFileTree>();
            if (parent != null)
                Parent = parent;
        }

        public void AddChild(ref IsolatedStorageFileTree child)
        {
            if (Children == null)
                Children = new List<IsolatedStorageFileTree>();
            Children.Add(child);
        }
    }

    public class IsolatedStorageFileNode
    {

        public IsolatedStorageFileNodeType NodeType { get
        {
            return NodeElement == null
                       ? IsolatedStorageFileNodeType.NotDefined
                       : (NodeElement is IsolatedStorageDirectoryInfo
                              ? IsolatedStorageFileNodeType.Directory
                              : IsolatedStorageFileNodeType.File);
        } }

        public IIsolatedStorageFile NodeElement { get; set; }
    }

    public enum IsolatedStorageFileNodeType
    {
        NotDefined = 0,
        Directory = 1,
        File = 2
    }

}
