using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Practices.Prism.ViewModel;

namespace WP7FlacPlayer.ViewModel
{
    [DataContract]
    public class ViewModel : NotificationObject, IDisposable
    {
        /// <summary>
        /// 墓碑化使用PhoneApplicationService保存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SaveInAppState(string key, object value)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(key))
            {
                PhoneApplicationService.Current.State.Remove(key);
            }
            PhoneApplicationService.Current.State.Add(key, value);
        }

        /// <summary>
        /// 从PhoneApplicationService删除保存值
        /// </summary>
        /// <param name="key"></param>
        public void RemoveInAppState(string key)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(key))
                PhoneApplicationService.Current.State.Remove(key);
        }

        /// <summary>
        /// 加载保存值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns>对应值，如果找不到，返回null</returns>
        public T LoadFromAppState<T>(string key)
        {
            object result;

            if (!PhoneApplicationService.Current.State.TryGetValue(key, out result))
            {
                result = default(T);
            }
            return (T)result;
        }

        /// <summary>
        /// 清空PhoneApplicationService数据
        /// </summary>
        public void ClearAll()
        {
            PhoneApplicationService.Current.State.Clear();
        }

        public void Dispose()
        {
            ((App)Application.Current).RootFrame.Navigated -= ViewModelNavigated;
            ((App)Application.Current).RootFrame.Navigating -= ViewModelNavigating;
            ((App)Application.Current).RootFrame.BackKeyPress -= RootFrameBackKeyPress;
            GC.SuppressFinalize(this);
        }

        public ViewModel()
        {
            ((App)Application.Current).RootFrame.Navigated += ViewModelNavigated;
            ((App)Application.Current).RootFrame.Navigating += ViewModelNavigating;
            ((App)Application.Current).RootFrame.BackKeyPress += RootFrameBackKeyPress;
        }

        void RootFrameBackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = OnBackKeyPressed(sender, e);
        }

        void ViewModelNavigating(object sender, NavigatingCancelEventArgs e)
        {
            e.Cancel = OnNavigating(sender, e);
        }

        void ViewModelNavigated(object sender, NavigationEventArgs e)
        {
            OnNavigated(sender,e);
        }

        ~ViewModel()
        {
            Dispose();
        }

        protected NavigationService NavigationService
        {
            get
            {
                return ((PhoneApplicationPage) ((App) Application.Current).RootFrame.Content).NavigationService;
            }
        }

        protected void RemoveBackEntry()
        {
            if(NavigationService.BackStack.Any())
                NavigationService.RemoveBackEntry();
        }

        protected void RemoveBackEntryToHome()
        {
            var count = NavigationService.BackStack.Count();
            for (var i = 0; i < count - 1; i++)
            {
                RemoveBackEntry();
            }
        }

        protected void GoBackHome()
        {
            RemoveBackEntryToHome();
            NavigationService.GoBack();
        }

        protected virtual bool OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            return false;
        }

        protected virtual void OnNavigated(object sender, NavigationEventArgs e)
        {
            
        }

        protected virtual bool OnBackKeyPressed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            return false;
        }
    }

    public static class NavigationServiceExtention
    {
        public static bool Navigate(this NavigationService navigationService,string uri)
        {
            return navigationService.Navigate(new Uri(uri, UriKind.Relative));
        }
    }

}
