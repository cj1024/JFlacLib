using System;
using Funq;

namespace WP7FlacPlayer.ViewModel
{
    public class ViewModelLocator : IDisposable
    {
        private readonly Container _containerLocator;

        public ViewModelLocator()
        {
            _containerLocator = InitialContainer();
        }

        public void Dispose()
        {
            _containerLocator.Dispose();
            GC.SuppressFinalize(this);
        }

        private Container InitialContainer()
        {
            var instance = new Container();
            instance.Register(c => new MainPageViewModel()).ReusedWithin(ReuseScope.None);
            return instance;
        }

        public MainPageViewModel MainPageViewModel
        {
            get { return _containerLocator.Resolve<MainPageViewModel>(); }
        }

    }
}
