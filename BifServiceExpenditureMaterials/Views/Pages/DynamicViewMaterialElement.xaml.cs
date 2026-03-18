using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using BifServiceExpenditureMaterials.Controls;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Models;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Динамическая карточка материала с пролистыванием записей текущего года.
    /// </summary>
    public partial class DynamicViewMaterialElement : UserControl
    {
        private int _index = 0;
        private List<Material> _materials = new();

        public DynamicViewMaterialElement()
        {
            InitializeComponent();
        }

        // ─── Загрузка ──────────────────────────────────────────────────────────

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _materials = App.dBcontext?.Materials?
                    .Where(m => m.Год == DateTime.Now.Year)
                    .OrderBy(m => m.Id)
                    .ToList() ?? new List<Material>();

                _index = 0;
                UpdateCardPanel();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DynamicViewMaterialElement] Ошибка загрузки: {ex.Message}");
            }
        }

        // ─── Навигация ─────────────────────────────────────────────────────────

        private void Button_ClickLeft(object sender, RoutedEventArgs e)
        {
            if (_index > 0)
                _index--;
            AnimateAndUpdate();
        }

        private void Button_ClickRight(object sender, RoutedEventArgs e)
        {
            if (_index < _materials.Count - 1)
                _index++;
            AnimateAndUpdate();
        }

        // ─── Обновление карточки ───────────────────────────────────────────────

        private void AnimateAndUpdate()
        {
            var fadeOut = new DoubleAnimation { From = 1, To = 0, Duration = TimeSpan.FromMilliseconds(150) };
            fadeOut.Completed += (s, e) =>
            {
                UpdateCardPanel();
                var fadeIn = new DoubleAnimation { From = 0, To = 1, Duration = TimeSpan.FromMilliseconds(150) };
                panelDynamicCards.BeginAnimation(OpacityProperty, fadeIn);
            };
            panelDynamicCards.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void UpdateCardPanel()
        {
            try
            {
                if (_materials.Count == 0 || _index < 0 || _index >= _materials.Count)
                    return;

                panelDynamicCards.Children.Clear();

                var material = _materials[_index];
                var color = material.ТипТраты == "ТО"
                    ? System.Windows.Media.Color.FromRgb(220, 20, 60)
                    : System.Windows.Media.Color.FromRgb(0, 205, 120);

                var parts = material.Ячейка?.Split(':');
                double x = parts?.Length >= 1 && double.TryParse(parts[0], out double px) ? px : 0;
                double y = parts?.Length >= 2 && double.TryParse(parts[1], out double py) ? py : 0;

                DynamicCardElement.CoordinatesPoint = new System.Windows.Point(x, y);

                panelDynamicCards.Children.Add(new DynamicCardElement(color)
                {
                    Responsible = material.Ответственный,
                    Year = material.Год.ToString(),
                    Month = material.Месяц,
                    Message = material.message,
                    TitleCard = material.НомерЯчейки
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DynamicViewMaterialElement] Ошибка обновления карточки: {ex.Message}");
            }
        }

        // ─── Анимация (для XAML-кнопок focus/unfocus) ─────────────────────────

        private void Border_GotFocusLeft(object sender, RoutedEventArgs e)   => FadeButton(sender, 1);
        private void Border_GotFocusRight(object sender, RoutedEventArgs e)  => FadeButton(sender, 1);
        private void Border_LostFocusLeft(object sender, RoutedEventArgs e)  => FadeButton(sender, 0);
        private void Border_LostFocusRight(object sender, RoutedEventArgs e) => FadeButton(sender, 0);

        private static void FadeButton(object sender, double toOpacity)
        {
            if (sender is UIElement el)
                el.BeginAnimation(OpacityProperty, new DoubleAnimation { To = toOpacity });
        }
    }
}
