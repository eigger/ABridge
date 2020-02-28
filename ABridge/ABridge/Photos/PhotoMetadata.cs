using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;
using TagLib;

namespace ABridge.Photos
{
    public class PhotoMetadata
    {
        String _path;
        public PhotoMetadata(String path)
        {
            _path = path;
        }

        public String[] GetTags()
        {
            String[] tags = null;
            using (var file = TagLib.File.Create(_path))
            {
                var tag = file.Tag as TagLib.Image.CombinedImageTag;
                tags = tag.Keywords;
            }
            return tags;
        }

        public void SetTags(String[] tags)
        {
            using (var file = TagLib.File.Create(_path))
            {
                var tag = file.Tag as TagLib.Image.CombinedImageTag;
                tag.Keywords = tags;
                file.Save();
            }
        }

        public String GetDate
        {
            get
            {
                DateTime date = new DateTime();
                using (var file = TagLib.File.Create(_path))
                {
                    var tag = file.Tag as TagLib.Image.CombinedImageTag;
                    try
                    {
                        date = (DateTime)tag.DateTime;
                    }
                    catch (Exception e)
                    {

                    }
                }
                return date.ToString();
            }

        }
    }
}
