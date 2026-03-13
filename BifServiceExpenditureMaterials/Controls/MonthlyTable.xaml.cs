using System.Data;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using BifServiceExpenditureMaterials.Forms;
using BifServiceExpenditureMaterials.Models;
using BifServiceExpenditureMaterials.Views.Pages;
using BifServiceExpenditureMaterials.Views.Windows;
using Newtonsoft.Json;

namespace BifServiceExpenditureMaterials.Controls
{
    /// <summary>
    /// Логика взаимодействия для MonthlyTable.xaml.
    /// Отображает месячную таблицу расходов материалов:
    /// строки — машины, столбцы — дни месяца (1–31).
    /// </summary>
    public partial class MonthlyTable : UserControl
    {
        /// <summary>Ссылка на WPF DataGrid для внешнего доступа.</summary>
        public System.Windows.Controls.DataGrid? Datagrid { get; set; }

        /// <summary>Внутренняя таблица данных, привязанная к DataGrid.</summary>
        private DataTable _table = new();

        // Константы масштабирования
        private const double MinScale  = 0.5; // Минимальный масштаб (50%)
        private const double MaxScale  = 3.0; // Максимальный масштаб (300%)
        private const double ScaleStep = 0.1; // Шаг изменения масштаба при прокрутке

        /// <summary>Флаг: выполняется ли операция копирования ячейки.</summary>
        public static bool isCopy = false;

        /// <summary>Значение скопированной ячейки (номер ТО/доливки).</summary>
        public static string CopiedMachine = "";

        /// <summary>Глобальная ссылка на текущий экземпляр таблицы (для статических вызовов).</summary>
        public static MonthlyTable MonTable;

        /// <summary>Цвет фона скопированной ячейки (для вставки с сохранением типа).</summary>
        private SolidColorBrush isCopiedBrush = new SolidColorBrush();

        public MonthlyTable()
        {
            InitializeComponent();
            try
            {
                BuildTable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка инициализации таблицы: {ex.Message}");
            }
            Datagrid = MaterialsGrid;
            MonTable = this;
        }
        
        //private void MaterialsGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    isRightMouseDown = true;
        //    MaterialsGrid.Cursor = Cursors.SizeAll; // меняем курсор на "перемещение"
        //    e.Handled = true; // чтобы не открывалось контекстное меню
        //}

        //private void MaterialsGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    isRightMouseDown = false;
        //    MaterialsGrid.Cursor = Cursors.Arrow;
        //    e.Handled = true;
        //}
        /// <summary>
        /// Сериализует текущее содержимое таблицы в JSON-строку.
        /// Используется при сохранении проекта.
        /// </summary>
        public string SerializeTableData()
        {
            var rows = _table.AsEnumerable()
                .Select(row => _table.Columns.Cast<DataColumn>()
                    .ToDictionary(col => col.ColumnName, col => row[col]))
                .ToList();

            return JsonConvert.SerializeObject(rows);
        }
        /// <summary>
        /// Десериализует JSON-строку и восстанавливает данные таблицы.
        /// Используется при загрузке сохранённого проекта.
        /// </summary>
        public void DeserializeTableData(string json)
        {
            var rows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
            if (rows == null || rows.Count == 0) return;

            _table.Clear();
            _table.Columns.Clear();

            // Создаём колонки на основе ключей в первом словаре
            foreach (var colName in rows[0].Keys)
                _table.Columns.Add(colName);

            // Заполняем строки
            foreach (var rowDict in rows)
            {

                var newRow = _table.NewRow();
                foreach (var kvp in rowDict)
                    newRow[kvp.Key] = kvp.Value ?? DBNull.Value;
                _table.Rows.Add(newRow);
            }

            MaterialsGrid.ItemsSource = _table.DefaultView;
        }
        //private void MaterialsGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    if (!isRightMouseDown) return; // масштабируем только при зажатой правой кнопке

        //    double scaleStep = 0.1;
        //    var position = e.GetPosition(MaterialsGrid);

        //    double scale = DataGridScale.ScaleX;

        //    double delta = e.Delta > 0 ? ScaleStep : -ScaleStep;
        //    double targetScale = Math.Clamp(scale + delta, MinScale, MaxScale);
        //    double originX = position.X / MaterialsGrid.ActualWidth;
        //    double originY = position.Y / MaterialsGrid.ActualHeight;
        //    SelectionCanvas.RenderTransformOrigin = new Point(originX, originY);

        //    if (Math.Abs(targetScale - scale) < 0.001)
        //        return;
        //    double newScaleX = DataGridScale.ScaleX;
        //    double newScaleY = DataGridScale.ScaleY;

        //    if (e.Delta > 0)
        //    {
        //        newScaleX += scaleStep;
        //        newScaleY += scaleStep;
        //    }
        //    else
        //    {
        //        newScaleX -= scaleStep;
        //        newScaleY -= scaleStep;

        //    }
        //    double paneScaletarget = Math.Clamp((scale - delta), MinScale, MaxScale);
        //    var animX = new DoubleAnimation(targetScale, TimeSpan.FromMilliseconds(150))
        //    { AccelerationRatio = 0.3, DecelerationRatio = 0.3 };
        //    var animY = animX.Clone();
        //    var animXPane = new DoubleAnimation(paneScaletarget, TimeSpan.FromMilliseconds(150))
        //    { AccelerationRatio = 0.3, DecelerationRatio = 0.3 };
        //    var animYPane = animXPane.Clone();
        //    // Ограничения масштаба
        //    newScaleX = Math.Clamp(newScaleX, 0.5, 3);
        //    newScaleY = Math.Clamp(newScaleY, 0.5, 3);

        //    DataGridScale.ScaleX = newScaleX;
        //    DataGridScale.ScaleY = newScaleY;
        //    CanvasScale.BeginAnimation(ScaleTransform.ScaleXProperty, animX);
        //    CanvasScale.BeginAnimation(ScaleTransform.ScaleYProperty, animY);        
        //    PanelOptionScale.BeginAnimation(ScaleTransform.ScaleXProperty, animX);
        //    PanelOptionScale.BeginAnimation(ScaleTransform.ScaleYProperty, animY);

        //    e.Handled = true;
        //}
        /// <summary>
        /// Строит таблицу: создаёт столбцы (Дата/Машины + дни 1–31)
        /// и добавляет строки по количеству машин из базы данных + 3 запасные.
        /// </summary>
        public void BuildTable()
        {
            try
            {
                _table.Clear();
                _table.Columns.Clear();
                _table.Rows.Clear();

                // Добавляем колонки (0-я — строка/название, остальные 1–31)
                _table.Columns.Add("Дата\\\nМашины");
                for (int i = 1; i <= 31; i++)
                    _table.Columns.Add(i.ToString());

                // Количество строк = машины + 3 запасных
                int machineCount = 0;
                try
                {
                    machineCount = App.dBcontext?.machine?.Count() ?? 0;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[MonthlyTable] Ошибка подсчёта машин: {ex.Message}");
                }

                var count = machineCount + 3;
                for (int row = 0; row < count; row++)
                    _table.Rows.Add(_table.NewRow());

                MaterialsGrid.ItemsSource = _table.DefaultView;
                MaterialsGrid.Columns.Clear();

                for (int i = 0; i < _table.Columns.Count; i++)
                {
                    var text = new System.Windows.Controls.TextBlock
                    {
                        Text = _table.Columns[i].ColumnName,
                        TextAlignment = TextAlignment.Center,
                        Width = 90
                    };

                    var column = new DataGridTextColumn
                    {
                        Width = 100,
                        Header = text,
                        IsReadOnly = true,
                        Binding = new System.Windows.Data.Binding($"[{_table.Columns[i].ColumnName}]")
                    };

                    // Заголовки 1–31 — светло-зеленые
                    if (i > 0)
                    {
                        column.HeaderStyle = new Style(typeof(DataGridColumnHeader))
                        {
                            Setters =
                            {
                                new Setter(BackgroundProperty, Brushes.LightGreen),
                                new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch),
                                new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                            }
                        };
                    }

                    MaterialsGrid.Columns.Add(column);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка построения таблицы: {ex.Message}");
            }
        }
        private void MaterialsGrid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
            if (MaterialsGrid.SelectedCells.Count == 0) return;

            var cellInfo = MaterialsGrid.SelectedCells[0];
            var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);
            if (cellContent == null) return;

            var cell = cellContent.Parent as DataGridCell;
            if (cell == null) return;

            Point cellPos = cell.TranslatePoint(new Point(0, 0), SelectionCanvas);
            Point cellPosopt = cell.TranslatePoint(new Point(0, 0), SelectionCanvas);

            double cellWidth = cell.ActualWidth;
            double cellHeight = cell.ActualHeight;
            
            // Задаём размер рамки
            SelectionBorder.Width = cellWidth;
            SelectionBorder.Height = cellHeight;
            
            // Анимируем рамку
            AnimateSelection(cellPos.X, cellPos.Y);
            AnimateSelectionOpt(cellPos.X, cellPos.Y, e.GetPosition(MainWindow.window.SnackbarPresenter));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка MouseLeftButtonUp: {ex.Message}");
            }
        }

        private void AnimateSelection(double toX, double toY)
        {
            DoubleAnimation xAnim = new DoubleAnimation()
            {
                To = toX,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            
            DoubleAnimation yAnim = new DoubleAnimation()
            {
                To = toY,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            SelectionTransform.BeginAnimation(TranslateTransform.XProperty, xAnim);
            SelectionTransform.BeginAnimation(TranslateTransform.YProperty, yAnim);
           
            
        }
        public async void AnimateSelectionOpt(double toX, double toY, Point a)
        {
            switch (MainWindow.window.WindowState)
            {
                case WindowState.Normal:

                    if (a.X > 500)
                    {
                        toX -= 630;
                    }
                    else
                    {
                        toX -= 200;

                    }
                    if (a.Y > 410)
                    {
                        toY -= 270;
                    }
                    else
                    {
                        toY -= 50;
                    }
                    
                    break;
                case WindowState.Maximized:

                    if (a.X > 500)
                    {
                        toX -= 700;
                        //toY -= 50;
                    }
                    else
                    {
                        toX -= 300;
                        //toY -= 50;
                    }
                    if(a.Y > 400)
                    {
                        toY -= 300;
                    }
                    else
                    {
                        toY -= 50;
                    }
                    break;
            }
            
                DoubleAnimation xAnim = new DoubleAnimation()
                {
                    To = toX,//SelectionTransform.X,
                    Duration = TimeSpan.FromMilliseconds(200),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

            DoubleAnimation yAnim = new DoubleAnimation()
            {
                To = toY,//SelectionTransform.Y,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            //System.Windows.MessageBox.Show(a.ToString());
            PanelOptionTransform.BeginAnimation(TranslateTransform.YProperty, yAnim);
            PanelOptionTransform.BeginAnimation(TranslateTransform.XProperty, xAnim);
            //var dialog = new Wpf.Ui.Controls.MessageBox();
            //dialog.Content = a.ToString();
            //await dialog.ShowDialogAsync();

        }
        private void MaterialsGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Окрашиваем первую ячейку (0,0) в серый
            if (e.Row.GetIndex() == 0)
            {
                var cell = GetCell(e.Row, 0);
                if (cell != null)
                    cell.Background = Brushes.Gray;
            }
        }

        private DataGridCell GetCell(DataGridRow row, int columnIndex)
        {
            if (row == null) return null;
            var presenter = FindVisualChild<DataGridCellsPresenter>(row);
            return presenter?.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
        }

        /// <summary>
        /// Рекурсивно ищет первый дочерний элемент заданного типа в визуальном дереве WPF.
        /// </summary>
        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T childOfType)
                    return childOfType;
                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void MaterialsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (MaterialsGrid.CurrentCell != null)
            //{
            //    var cellInfo = MaterialsGrid.CurrentCell;
            //    int rowIndex = MaterialsGrid.Items.IndexOf(cellInfo.Item);
            //    int columnIndex = cellInfo.Column.DisplayIndex;
            //}
        }
        public void SearchCell(System.Windows.Controls.DataGrid dataGrid,int rowind,int columnind)
        {
            try
            {
                int rowIndex = rowind;
                int columnIndex = columnind;

                if (rowIndex < 0 || rowIndex >= dataGrid.Items.Count) return;
                dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);

                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);

                if (row != null)
                {
                    DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);
                    if (presenter == null) return;
                    DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);

                    if (cell != null)
                    {
                        cell.Focus();
                        cell.IsSelected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка SearchCell: {ex.Message}");
            }
        }
        private void MaterialsGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                var datagrid = MaterialsGrid;
                if (datagrid.SelectedCells.Count > 0)
                {
                    DataGridCellInfo cellInfo = datagrid.SelectedCells[0];
                    var cell = NewGetCell(datagrid, datagrid.Items.IndexOf(cellInfo.Item), 0);
                    if (cell?.Content is System.Windows.Controls.TextBlock tb)
                        nammachine.Text = tb.Text;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка SelectedCellsChanged: {ex.Message}");
            }
        }
        private DataGridCell GetCell(System.Windows.Controls.DataGrid grid, DataGridCellInfo cellInfo)
        {
            var row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
            if (row == null)
            {
                // Строка ещё не отрисована
                grid.ScrollIntoView(cellInfo.Item);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
                if (row == null)
                    return null;
            }

            var presenter = FindVisualChild<DataGridCellsPresenter>(row);
            if (presenter == null)
                return null;

            var cell = presenter.ItemContainerGenerator.ContainerFromIndex(cellInfo.Column.DisplayIndex) as DataGridCell;
            return cell;
        }

        private async void TO_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            if (MaterialsGrid.SelectedCells.Count == 0) return;
            if (MaterialsGrid.CurrentCell != null)
            {
                var dataGrid = MaterialsGrid;

                var selectedCell = dataGrid.SelectedCells[0];

                string nomer = "";
                int rowIndex = 0;
                int columnIndex = 0;
                DataGridCellInfo cellInfo = dataGrid.SelectedCells[0];
                rowIndex = dataGrid.Items.IndexOf(cellInfo.Item);
                columnIndex = dataGrid.Columns.IndexOf(cellInfo.Column);
                var firstnomermachine = GetFirstColumnCellText(dataGrid, dataGrid.SelectedCells[0]);
                var currentselecttext = GetCurrentColumnCellText(dataGrid, dataGrid.SelectedCells[0]);
                var row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                var currentColumnCell = NewGetCell(dataGrid, rowIndex, columnIndex);
                if (currentColumnCell == null) return;
                nomer = firstnomermachine + "_" + columnIndex;
                if (currentColumnCell.Content is System.Windows.Controls.TextBlock ts)
                {
                    if (ts.Text != "")
                    {
                        dataGrid.CancelEdit(DataGridEditingUnit.Cell);
                        dataGrid.CancelEdit(DataGridEditingUnit.Row);
                        var ty = currentselecttext;
                        var sltbvl = HomePage.activeMonth;
                        var year = Convert.ToInt32(HomePage._home.YearComboBox.Text);
                        HomePage.CurrentMounth = sltbvl;
                            //HomePage.CurrentYear = year;
                        FormMaterial toForm = new FormMaterial(ty, null, false, Convert.ToInt32(HomePage._home.YearComboBox.Text));
                        //FormTO to = new FormTO(ty,null,false, Convert.ToInt32(HomePage._home.YearComboBox.Text));
                        toForm.groupBox.IsEnabled = false;
                        toForm.SaveButton.IsEnabled = false;
                        toForm.AddPersonBtn.IsEnabled = false;
                        toForm.TitleBar.Title = $"Биф сервисы - Форма TO ({ty})";
                        toForm.Show();
                    }
                    else
                    {
                        ts.Text = nomer;
                        //App.dBcontext.dataediting.Where(e => e.НомерЯчейки == nomer).ToList()[0].is_editing = 1;
                        currentColumnCell.Background = Brushes.DarkRed;
                        var sltbvl = HomePage.activeMonth;
                        HomePage.CurrentMounth = sltbvl;
                        FormMaterial toForm = new FormMaterial(ts.Text, rowIndex + ":" + columnIndex, true, Convert.ToInt32(HomePage._home.YearComboBox.Text));
                            //FormTO to = new FormTO(ts.Text, rowIndex + ":" + columnIndex, true, Convert.ToInt32(HomePage._home.YearComboBox.Text));
                        toForm.groupBox.IsEnabled = true;
                        toForm.SaveButton.IsEnabled = true;
                        toForm.TitleBar.Title = $"Биф сервисы - Форма TO (Новая) - {ts.Text}";
                        toForm.Show();
                                      
                    }

                }
            }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка TO_Click: {ex.Message}");
            }
        }
        public static void ViewCellOnPoints(int x, int y)
        {
            try
            {
                if (MonTable?.Datagrid == null) return;
                if (x < 0 || x >= MonTable.Datagrid.Items.Count) return;
                if (y < 0 || y >= MonTable.Datagrid.Columns.Count) return;
                MonTable.Datagrid.ScrollIntoView(MonTable.Datagrid.Items[x], MonTable.Datagrid.Columns[y]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка ViewCellOnPoints: {ex.Message}");
            }
        }
        public string GetFirstColumnCellText(System.Windows.Controls.DataGrid grid, DataGridCellInfo cellInfo)
        {
            try
            {
                int rowIndex = grid.Items.IndexOf(cellInfo.Item);
                var firstColumnCell = NewGetCell(grid, rowIndex, 0);
                if (firstColumnCell?.Content is System.Windows.Controls.TextBlock tb)
                {
                    return tb.Text;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка GetFirstColumnCellText: {ex.Message}");
            }
            return null;
        }
        public string GetCurrentColumnCellText(System.Windows.Controls.DataGrid grid, DataGridCellInfo cellInfo)
        {
            try
            {
                int rowIndex = grid.Items.IndexOf(cellInfo.Item);
                int columnIndex = grid.Columns.IndexOf(cellInfo.Column);
                var currentColumnCell = NewGetCell(grid, rowIndex, columnIndex);
                if (currentColumnCell?.Content is System.Windows.Controls.TextBlock ts)
                {
                    return ts.Text;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка GetCurrentColumnCellText: {ex.Message}");
            }
            return null;
        }
        public SolidColorBrush GetCurrentColumnCellBackground(System.Windows.Controls.DataGrid grid, DataGridCellInfo cellInfo)
        {
            try
            {
                int rowIndex = grid.Items.IndexOf(cellInfo.Item);
                int columnIndex = grid.Columns.IndexOf(cellInfo.Column);
                var cell = NewGetCell(grid, rowIndex, columnIndex);
                if (cell != null)
                    return (SolidColorBrush)cell.Background;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка GetCurrentColumnCellBackground: {ex.Message}");
            }
            return null;
        }
        public void EnabledCell(System.Windows.Controls.DataGrid dataGrid, int rowIndex, int columnIndex, bool isEnabled)
        {
            try
            {
                dataGrid.ScrollIntoView(dataGrid.Items[rowIndex], dataGrid.Columns[columnIndex]);

                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
                if (row == null)
                {
                    dataGrid.UpdateLayout();
                    dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
                    row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
                }

                if (row != null)
                {
                    DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);
                    if (presenter == null)
                    {
                        dataGrid.ScrollIntoView(row, dataGrid.Columns[columnIndex]);
                        presenter = FindVisualChild<DataGridCellsPresenter>(row);
                    }
                    if (presenter == null) return;

                    DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                    if (cell != null)
                    {
                        cell.IsEditing = isEnabled;
                        cell.IsEnabled = isEnabled;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка EnabledCell: {ex.Message}");
            }
        }
        public void SetCellValue(System.Windows.Controls.DataGrid dataGrid, int rowIndex = 1, int columnIndex = 1, string newValue = "")
        {
            try
            {
                // Прокручиваем до нужной ячейки, чтобы она была создана (если виртуализирована)
                //dataGrid.ScrollIntoView(dataGrid.Items[rowIndex], dataGrid.Columns[columnIndex]);

                // Получаем контейнер строки
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
                if (row == null)
                {
                    dataGrid.UpdateLayout();
                    dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
                    row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
                }

                if (row != null)
                {
                    // Получаем визуальный элемент ячеек
                    DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);
                    if (presenter == null)
                    {
                        dataGrid.ScrollIntoView(row, dataGrid.Columns[columnIndex]);
                        presenter = FindVisualChild<DataGridCellsPresenter>(row);
                    }
                    try
                    {
                        DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                        if (cell != null)
                        {
                            cell.VerticalContentAlignment   = VerticalAlignment.Center;
                            cell.HorizontalContentAlignment = HorizontalAlignment.Center;
                            cell.Foreground = Brushes.Black;

                            if (cell.Content is System.Windows.Controls.TextBlock tb)
                            {
                                tb.TextAlignment   = TextAlignment.Center;
                                tb.VerticalAlignment = VerticalAlignment.Center;
                                tb.Text = newValue;
                            }
                            else if (cell.Content is System.Windows.Controls.TextBox textBox)
                            {
                                textBox.Text = newValue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Ячейка может быть виртуализирована и недоступна — игнорируем
                        System.Diagnostics.Debug.WriteLine($"[MonthlyTable] SetCellValue: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MonthlyTable] SetCellValue (внешний): {ex.Message}");
            }
        }
        public void SetCellBackground(System.Windows.Controls.DataGrid dataGrid, int rowIndex, int columnIndex, string nomer)
        {
            if (dataGrid.Items.Count <= rowIndex || dataGrid.Columns.Count <= columnIndex)
                return;

            dataGrid.UpdateLayout();
            dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
            Task.Run(() =>
            {
                // Даем WPF время отрисовать ячейку
                dataGrid.Dispatcher.Invoke(() =>
                {
                    DataGridRow row = GetRow(dataGrid, rowIndex);
                    if (row == null) return;
                    DataGridCell cell = GetCell(dataGrid, row, columnIndex);
                    if (cell == null) return;
                    
                    try
                    {
                        var material = App.dBcontext?.Materials?
                            .OrderBy(e => e.Id)
                            .FirstOrDefault(e => e.НомерЯчейки == nomer);

                        if (material == null) return;

                        cell.Background = material.ТипТраты == "ТО" ? Brushes.DarkRed : Brushes.DarkGreen;
                        cell.VerticalContentAlignment = VerticalAlignment.Center;
                        cell.HorizontalContentAlignment = HorizontalAlignment.Center;
                        cell.Foreground = Brushes.White;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[MonthlyTable] Ошибка установки фона ячейки '{nomer}': {ex.Message}");
                    }


                }, DispatcherPriority.Background);
            });
        }
        public void SetCellBackground(System.Windows.Controls.DataGrid dataGrid, int rowIndex, int columnIndex, SolidColorBrush brush)
        {
            if (dataGrid.Items.Count <= rowIndex || dataGrid.Columns.Count <= columnIndex)
                return;


            Task.Run(() =>
            {
                // Даем WPF время отрисовать ячейку
                dataGrid.Dispatcher.Invoke(() =>
                {
                    DataGridRow row = GetRow(dataGrid, rowIndex);
                    if (row == null) return;
                    DataGridCell cell = GetCell(dataGrid, row, columnIndex);
                    if (cell == null) return;
                    cell.Background = brush;


                }, DispatcherPriority.Background);
            });
        }
        public static DataGridRow GetRow(System.Windows.Controls.DataGrid dataGrid, int index)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);

            if (row == null)
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[index]);
                row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }

            return row;
        }
        public static DataGridCell GetCell(System.Windows.Controls.DataGrid dataGrid, DataGridRow row, int columnIndex)
        {
            if (dataGrid.IsLoaded && dataGrid.IsInitialized)
            {
                if (row == null) return null;

                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null)
                {
                    dataGrid.ScrollIntoView(row, dataGrid.Columns[columnIndex]);
                    presenter = FindVisualChild<DataGridCellsPresenter>(row);
                }
                if (presenter == null) return null;

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                return cell;
            }
            return null;
        }
        
        public DataGridCell NewGetCell(System.Windows.Controls.DataGrid grid, int row, int column)
        {
            try
            {
                DataGridRow rowContainer = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(row);

                if (rowContainer != null)
                {
                    DataGridCellsPresenter presenter = NewFindVisualChild<DataGridCellsPresenter>(rowContainer);
                    if (presenter == null) return null;

                    DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                    if (cell == null)
                    {
                        grid.ScrollIntoView(rowContainer, grid.Columns[column]);
                        cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                    }
                    return cell;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка NewGetCell: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Псевдоним для <see cref="FindVisualChild{T}"/>.
        /// Ищет первый дочерний элемент заданного типа в визуальном дереве.
        /// </summary>
        public static T NewFindVisualChild<T>(DependencyObject obj) where T : DependencyObject
            => FindVisualChild<T>(obj);
        private async void Dol_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            if (MaterialsGrid.SelectedCells.Count == 0) return;
            if (MaterialsGrid.CurrentCell != null)
            {
                var dataGrid = MaterialsGrid;

                var selectedCell = dataGrid.SelectedCells[0];

                string nomer = "";
                int rowIndex = 0;
                int columnIndex = 0;
                DataGridCellInfo cellInfo = dataGrid.SelectedCells[0];
                rowIndex = dataGrid.Items.IndexOf(cellInfo.Item);
                columnIndex = dataGrid.Columns.IndexOf(cellInfo.Column);
                var firstnomermachine = GetFirstColumnCellText(dataGrid, dataGrid.SelectedCells[0]);
                var currentselecttext = GetCurrentColumnCellText(dataGrid, dataGrid.SelectedCells[0]);
                var row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                var currentColumnCell = NewGetCell(dataGrid, rowIndex, columnIndex);
                if (currentColumnCell == null) return;
                nomer = firstnomermachine + "_" + columnIndex;
                if (currentColumnCell.Content is System.Windows.Controls.TextBlock ts)
                {
                    if (ts.Text != "")
                    {
                        dataGrid.CancelEdit(DataGridEditingUnit.Cell);
                        dataGrid.CancelEdit(DataGridEditingUnit.Row);
                        var ty = currentselecttext;
                        var sltbvl = HomePage.activeMonth;
                        var year = Convert.ToInt32(HomePage._home.YearComboBox.Text);
                        HomePage.CurrentMounth = sltbvl;
                        HomePage.CurrentYear = year;
                        FormMaterial dolForm = new FormMaterial(ty, null, false, Convert.ToInt32(HomePage._home.YearComboBox.Text), "Доливка");
                        dolForm.groupBox.IsEnabled = false;
                        dolForm.SaveButton.IsEnabled = false;
                        dolForm.AddPersonBtn.IsEnabled = false;
                        dolForm.TitleBar.Title = $"Биф сервисы - Форма Доливки ({ty})";
                        dolForm.Show();
                    }
                    else
                    {
                        ts.Text = nomer;
                        //App.dBcontext.dataediting.Where(e => e.НомерЯчейки == nomer).ToList()[0].is_editing = 1;
                        currentColumnCell.Background = Brushes.DarkGreen;
                        var sltbvl = HomePage.activeMonth;
                        HomePage.CurrentMounth = sltbvl;
                        FormMaterial dolForm = new FormMaterial(ts.Text, rowIndex + ":" + columnIndex, true, Convert.ToInt32(HomePage._home.YearComboBox.Text), "Доливка");
                        dolForm.groupBox.IsEnabled = true;
                        dolForm.SaveButton.IsEnabled = true;
                        dolForm.TitleBar.Title = $"Биф сервисы - Форма Доливки (Новая) - {ts.Text}";
                        dolForm.Show();
                    }

                }
            }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка Dol_Click: {ex.Message}");
            }
        }
        /// <summary>
        /// Загружает данные машин из базы данных и заполняет первый столбец таблицы.
        /// Отображаются только машины текущего года.
        /// </summary>
        public async void GetData()
        {
            try
            {
            await Task.Run(async () =>
            {
                try
                {
                    var machines = App.dBcontext.machine?.ToList() ?? new List<machine>();
                    int i = 0;
                    foreach (var machine in machines)
                    {
                        if (machine.Год == DateTime.Now.Year)
                        {
                            await Dispatcher.BeginInvoke(() =>
                            {
                                SetCellValue(MaterialsGrid, ++i, 0, (machine.Code));
                                EnabledCell(MaterialsGrid, i, 0, false);
                            });
                        }
                    }
                    var materials = App.dBcontext.Materials?.ToList() ?? new List<Material>();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[MonthlyTable] Ошибка GetData: {ex.Message}");
                }
            });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка GetData (внешний): {ex.Message}");
            }
        }
        private async void MaterialsGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            var dataGrid = MaterialsGrid;
            dataGrid.BeginningEdit += DataGrid_BeginningEdit;
            int rowIndex = 0;
            var selectedCell = dataGrid.SelectedCells[0];
            if (dataGrid.SelectedCells.Count > 0)
            {
                DataGridCellInfo cellInfo = dataGrid.SelectedCells[0];

                // Номер строки
                rowIndex = dataGrid.Items.IndexOf(cellInfo.Item);
                if (rowIndex == 1)
                {

                    if (e.EditAction == DataGridEditAction.Commit)
                    {
                        var tr = ((System.Windows.Controls.TextBox)e.EditingElement).Text;
                        if (tr == null)
                        {
                            await Task.Run(async () =>
                            {
                                
                                {

                                    machine Mach = new machine();
                                    await Dispatcher.BeginInvoke(() =>
                                {
                                            Mach = new machine
                                            {
                                                Code = tr,
                                                Год = DateTime.Now.Year
                                            };
                                        });
                                    await App.dBcontext.machine.AddAsync(Mach);
                                    await App.dBcontext.SaveChangesAsync();

                                }

                            });
                        }
                        
                    }
                }
            }
        }

        private void DataGrid_BeginningEdit(object? sender, DataGridBeginningEditEventArgs e)
        {
            //if
        }
        private async void Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            var button = (System.Windows.Controls.Button)sender;
            if (MaterialsGrid.SelectedCells.Count == 0) return;
            if (MaterialsGrid.CurrentCell != null)
            {
                var nomer = "";
                var dataGrid = MaterialsGrid;
                int rowIndex = 0;
                int columnIndex = 0;
                
                DataGridCellInfo cellInfo = dataGrid.SelectedCells[0];
                rowIndex = dataGrid.Items.IndexOf(cellInfo.Item);
                columnIndex = dataGrid.Columns.IndexOf(cellInfo.Column);


                var firstnomermachine = GetFirstColumnCellText(dataGrid, dataGrid.SelectedCells[0]);
                
                {
                    var material = App.dBcontext?.Materials?.OrderBy(s => s.Id);
                    if (material == null) return;
                    var copyCell = NewGetCell(dataGrid, rowIndex, columnIndex);
                    if (copyCell?.Content is System.Windows.Controls.TextBlock tb)
                    {
                        CopiedMachine = tb.Text;
                    }
                    else return;
                    InsertBtn.Content = "Вставить: " + CopiedMachine;

                    if (material.Any(s => s.НомерЯчейки == CopiedMachine))
                    {
                        isCopiedBrush = GetCurrentColumnCellBackground(dataGrid, cellInfo);
                    }
                }
            }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка Copy_Click: {ex.Message}");
            }
        }
        private async void Insert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            var button = (System.Windows.Controls.Button)sender;
            if (MaterialsGrid.SelectedCells.Count == 0) return;
            if (MaterialsGrid.CurrentCell != null)
            {
                var nomer = "";
                var dataGrid = MaterialsGrid;
                int rowIndex = 0;
                int columnIndex = 0;

                DataGridCellInfo cellInfo = dataGrid.SelectedCells[0];
                rowIndex = dataGrid.Items.IndexOf(cellInfo.Item);
                columnIndex = dataGrid.Columns.IndexOf(cellInfo.Column);


                var firstnomermachine = GetFirstColumnCellText(dataGrid, dataGrid.SelectedCells[0]);
                
                {
                    var material = App.dBcontext?.Materials?.OrderBy(s => s.Id);
                    if (material == null) return;
                    if (!string.IsNullOrEmpty(CopiedMachine))
                    {
                        var currentColumnCell = NewGetCell(dataGrid, rowIndex, columnIndex);
                        if (currentColumnCell == null) return;
                        nomer = firstnomermachine + "_" + columnIndex;
                        if (currentColumnCell.Content is System.Windows.Controls.TextBlock ts)
                        {
                            if (string.IsNullOrWhiteSpace(ts.Text))
                            {
                                var sltbvl = HomePage.activeMonth;
                                var year = Convert.ToInt32(HomePage._home.YearComboBox.Text);
                                SetCellValue(dataGrid, rowIndex, columnIndex, nomer);
                                SetCellBackground(dataGrid, rowIndex, columnIndex, isCopiedBrush);
                                var mat = material.FirstOrDefault(s => s.НомерЯчейки == CopiedMachine);
                                if (mat == null) return;
                                var countmat = App.dBcontext?.CountMaterials?.FirstOrDefault(s => s.Id == mat.countmaterial_id);
                                if (countmat == null) return;
                                nomer = firstnomermachine + "_" + columnIndex;
                                new FormMaterial(mat, countmat, nomer, rowIndex + ":" + columnIndex, mat.ТипТраты ?? "ТО").Show();
                                await Task.Delay(1000);
                                isCopiedBrush = null;
                                InsertBtn.Content = "Буфер пуст";
                            }
                        }
                    }
                }
            }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MonthlyTable] Ошибка Insert_Click: {ex.Message}");
            }
        }
    }
}