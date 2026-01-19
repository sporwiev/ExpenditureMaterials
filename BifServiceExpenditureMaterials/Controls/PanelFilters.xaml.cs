using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BifServiceExpenditureMaterials.Controls
{
    /// <summary>
    /// Логика взаимодействия для PanelFilters.xaml
    /// </summary>
    [ContentProperty(nameof(Children))]
    public partial class PanelFilters : UserControl
    {

        private bool _isOpen = false;
        private const double PanelHeight = 70;
        private const double PanelWidth = 730;

        public ItemCollection Children => ItemsHost.Items;

        public PanelFilters()
        {
            InitializeComponent();
            DataContext = this;

        }
        public bool IsOpen
        {
            get { return _isOpen; }
        }
        
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var animationHeight = new DoubleAnimation
            {
                To = _isOpen ? 0 : PanelHeight,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            var animationWidth = new DoubleAnimation
            {
                To = _isOpen ? 0 : PanelWidth,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            Panel.BeginAnimation(Border.HeightProperty, animationHeight);
            Panel.BeginAnimation(Border.WidthProperty, animationWidth);

            _isOpen = !_isOpen;
        }
    }
}
