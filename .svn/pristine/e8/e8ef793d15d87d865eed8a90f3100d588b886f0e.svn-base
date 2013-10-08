using System;
using System.Globalization;
using System.Linq;
using Microsoft.Practices.Prism.ViewModel;
using WPJFlacLib.JFlac.Metadata;

namespace WP7FlacPlayer.Util
{
    public class SongProperty : NotificationObject
    {
        private string _album;
        public string Album { get { return _album; } set { _album = value; RaisePropertyChanged(() => Album); } }
        private string _artist;
        public string Artist { get { return _artist; } set { _artist = value; RaisePropertyChanged(() => Artist); } }
        private string _genre;
        public string Genre { get { return _genre; } set { _genre = value; RaisePropertyChanged(() => Genre); } }
        private string _title;
        public string Title { get { return _title; } set { _title = value; RaisePropertyChanged(() => Title); } }
        private string _discNumber;
        public string DiscNumber { get { return _discNumber; } set { _discNumber = value;RaisePropertyChanged(()=>DiscNumber); } }
        private string _discTotal;
        public string DiscTotal { get { return _discTotal; } set { _discTotal = value;RaisePropertyChanged(()=>DiscTotal); } }
        private string _trackNumber;
        public string TrackNumber { get { return _trackNumber; } set { _trackNumber = value;RaisePropertyChanged(()=>TrackNumber); } }
        private string _trackTotal;
        public string TrackTotal { get { return _trackTotal; } set { _trackTotal = value; RaisePropertyChanged(() => TrackTotal); } }

        private SongProperty()
        {
            
        }

        public static SongProperty FromVorbisComment(VorbisComment comment)
        {
            var result = new SongProperty();
            var type = result.GetType();
            var propertyInfos = type.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var temp = comment.GetCommentByName(propertyInfo.Name);
                var value = "";
                if(temp!=null&&temp.Length>0)
                {
                    value = temp.Aggregate(value, (current, info) => current + string.Format("{0};", info));
                }
                value = value.TrimEnd(';');
                propertyInfo.SetValue(result,Convert.ChangeType(value, propertyInfo.PropertyType, new CultureInfo("zh-CN")),null);
            }
            return result;
        }

    }
}
