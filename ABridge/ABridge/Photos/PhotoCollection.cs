// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace ABridge.Photos
{
    /// <summary>
    ///     This class represents a collection of photos in a directory.
    /// </summary>
    public class PhotoCollection : ObservableCollection<Photo>
    {
        private static int MAX_LIST_COUNT = 200;
        private DirectoryInfo _directory;

        public PhotoCollection()
        {
        }

        public PhotoCollection(string path) : this(new DirectoryInfo(path))
        {
        }

        public PhotoCollection(DirectoryInfo directory)
        {
            _directory = directory;
        }


        public bool AddPhoto(Photo photo)
        {
            if (Count > MAX_LIST_COUNT) return false;
            Add(photo);
            return true;
        }
        private FileInfo[] GetImageFilesFromFolder(string path)
        {
            string[] extensions = new[] { ".jpg", ".gif", ".png" };
            DirectoryInfo dInfo = new DirectoryInfo(path);

            FileInfo[] files = dInfo.GetFiles()
              .Where(f => extensions.Contains(f.Extension.ToLower()))
              .ToArray();
            return files;
        }

        public void UpdatePhotosWithTag(string path, string tag = "")
        {
            try
            {
                FileInfo[] files = GetImageFilesFromFolder(path);
                foreach (FileInfo _file in files)
                {
                    Photo photo = new Photo(_file.FullName);
                    try
                    {
                        if (tag.Length == 0)
                        {
                            if (AddPhoto(photo) == false) return;
                        }
                        else
                        {
                            string[] tags = photo.Metadata.GetTags();
                            if (tags != null)
                            {
                                foreach (string _tag in tags)
                                {
                                    if (tag == _tag)
                                    {
                                        AddPhoto(photo);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                string[] directoreis = Directory.GetDirectories(path);
                if (directoreis.Length > 0)
                {
                    foreach (string _directory in directoreis)
                    {
                        UpdatePhotosWithTag(_directory, tag);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}