using WP7FlacPlayer.ViewModel;

namespace WP7FlacPlayer.View
{
    public partial class MainPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            ((MainPageViewModel) DataContext).PlayListBox = PlayList;
            ((MainPageViewModel)DataContext).ControlPanel = ControlPanel;
        }
    }
}