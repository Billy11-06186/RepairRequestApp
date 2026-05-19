using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RepairRequestApp
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<RepairRequest> allRequests;
        private RepairRequest selectedRequest;
        private string currentSortBy = "По ID";
        private string currentFilterStatus = "Все";
        private string currentSearchText = "";

        public MainWindow()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadData();

            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void InitializeDatabase()
        {
            try
            {
                DatabaseHelper.InitializeDatabase();
                System.Diagnostics.Debug.WriteLine("База данных успешно инициализирована");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации базы данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                var requests = DatabaseHelper.GetAllRepairRequests();
                allRequests = new ObservableCollection<RepairRequest>(requests);
                ApplyFilterAndSort();

                System.Diagnostics.Debug.WriteLine($"Загружено {requests.Count} заявок");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilterAndSort()
        {
            var query = allRequests.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(currentSearchText))
            {
                var searchResults = DatabaseHelper.SearchRepairRequests(currentSearchText);
                query = searchResults;
            }

            if (currentFilterStatus != "Все")
            {
                var filteredResults = DatabaseHelper.FilterByStatus(currentFilterStatus);
                query = filteredResults;
            }

            var sortedResults = DatabaseHelper.GetSortedRequests(currentSortBy);

            if (!string.IsNullOrWhiteSpace(currentSearchText) || currentFilterStatus != "Все")
            {
                var tempList = sortedResults.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(currentSearchText))
                {
                    tempList = tempList.Where(r =>
                        r.Equipment.ToLower().Contains(currentSearchText.ToLower()) ||
                        r.FaultType.ToLower().Contains(currentSearchText.ToLower()) ||
                        r.Client.ToLower().Contains(currentSearchText.ToLower()) ||
                        r.Description.ToLower().Contains(currentSearchText.ToLower()));
                }

                if (currentFilterStatus != "Все")
                {
                    tempList = tempList.Where(r => r.Status == currentFilterStatus);
                }

                dgRequests.ItemsSource = tempList.ToList();
            }
            else
            {
                dgRequests.ItemsSource = sortedResults;
            }
        }

        private void RefreshData()
        {
            LoadData();
        }

        private void ClearForm()
        {
            txtEquipment.Clear();
            txtFaultType.Clear();
            txtClient.Clear();
            txtDescription.Clear();
            cmbStatus.SelectedIndex = 0;
            selectedRequest = null;
            btnAdd.IsEnabled = true;
            btnUpdate.IsEnabled = false;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtEquipment.Text))
            {
                MessageBox.Show("Введите название оборудования!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFaultType.Text))
            {
                MessageBox.Show("Введите тип неисправности!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtClient.Text))
            {
                MessageBox.Show("Введите имя клиента!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                var newRequest = new RepairRequest
                {
                    Equipment = txtEquipment.Text,
                    FaultType = txtFaultType.Text,
                    Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Новая",
                    Client = txtClient.Text,
                    Description = txtDescription.Text,
                    CreatedDate = DateTime.Now
                };

                DatabaseHelper.AddRepairRequest(newRequest);
                RefreshData();
                ClearForm();

                MessageBox.Show("Заявка успешно добавлена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRequest == null)
            {
                MessageBox.Show("Выберите заявку для обновления!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateForm()) return;

            try
            {
                selectedRequest.Equipment = txtEquipment.Text;
                selectedRequest.FaultType = txtFaultType.Text;
                selectedRequest.Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Новая";
                selectedRequest.Client = txtClient.Text;
                selectedRequest.Description = txtDescription.Text;

                DatabaseHelper.UpdateRepairRequest(selectedRequest);
                RefreshData();
                ClearForm();

                MessageBox.Show("Заявка успешно обновлена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRequest == null)
            {
                MessageBox.Show("Выберите заявку для удаления!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Удалить заявку №{selectedRequest.Id}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteRepairRequest(selectedRequest.Id);
                    RefreshData();
                    ClearForm();

                    MessageBox.Show("Заявка успешно удалена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
            ClearForm();
        }

        private void BtnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            cmbFilterStatus.SelectedIndex = 0;
            cmbSort.SelectedIndex = 0;
            currentSearchText = "";
            currentFilterStatus = "Все";
            currentSortBy = "По ID";
            RefreshData();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void DgRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRequest = dgRequests.SelectedItem as RepairRequest;

            if (selectedRequest != null)
            {
                txtEquipment.Text = selectedRequest.Equipment;
                txtFaultType.Text = selectedRequest.FaultType;
                txtClient.Text = selectedRequest.Client;
                txtDescription.Text = selectedRequest.Description;

                for (int i = 0; i < cmbStatus.Items.Count; i++)
                {
                    if ((cmbStatus.Items[i] as ComboBoxItem)?.Content.ToString() == selectedRequest.Status)
                    {
                        cmbStatus.SelectedIndex = i;
                        break;
                    }
                }

                btnAdd.IsEnabled = false;
                btnUpdate.IsEnabled = true;
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentSearchText = txtSearch.Text;
            ApplyFilterAndSort();
        }

        private void CmbFilterStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFilterStatus.SelectedItem is ComboBoxItem selectedStatus)
            {
                currentFilterStatus = selectedStatus.Content.ToString();
                ApplyFilterAndSort();
            }
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbSort.SelectedItem is ComboBoxItem selectedSort)
            {
                currentSortBy = selectedSort.Content.ToString();
                ApplyFilterAndSort();
            }
        }
    }
}