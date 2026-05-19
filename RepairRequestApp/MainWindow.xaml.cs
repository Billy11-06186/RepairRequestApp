using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RepairRequestApp
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<RepairRequest> allRequests;
        private ObservableCollection<RepairRequest> filteredRequests;
        private RepairRequest selectedRequest;
        private int nextId = 1;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            allRequests = new ObservableCollection<RepairRequest>();
            filteredRequests = new ObservableCollection<RepairRequest>();

            AddTestData();

            UpdateDataGrid();
        }

        private void AddTestData()
        {
            var testRequests = new List<RepairRequest>
            {
                new RepairRequest { Id = nextId++, Equipment = "Ноутбук Dell", FaultType = "Не включается",
                    Status = "Новая", Client = "Иванов И.И.", Description = "Ноутбук не реагирует на кнопку включения",
                    CreatedDate = DateTime.Now.AddDays(-5) },
                new RepairRequest { Id = nextId++, Equipment = "Смартфон iPhone", FaultType = "Разбит экран",
                    Status = "В работе", Client = "Петров П.П.", Description = "Трещины на экране, требуется замена",
                    CreatedDate = DateTime.Now.AddDays(-3) },
                new RepairRequest { Id = nextId++, Equipment = "Холодильник Samsung", FaultType = "Не морозит",
                    Status = "Завершена", Client = "Сидорова А.А.", Description = "Холодильник работает, но не охлаждает",
                    CreatedDate = DateTime.Now.AddDays(-7) },
                new RepairRequest { Id = nextId++, Equipment = "Стиральная машина LG", FaultType = "Не сливает воду",
                    Status = "Новая", Client = "Козлов Д.Д.", Description = "Вода не уходит после стирки",
                    CreatedDate = DateTime.Now.AddDays(-2) },
                new RepairRequest { Id = nextId++, Equipment = "Телевизор Sony", FaultType = "Нет изображения",
                    Status = "В работе", Client = "Михайлова Е.Е.", Description = "Звук есть, изображения нет",
                    CreatedDate = DateTime.Now.AddDays(-1) }
            };

            foreach (var request in testRequests)
            {
                allRequests.Add(request);
            }
        }

        private void UpdateDataGrid()
        {
            ApplyFilterAndSort();
        }

        private void ApplyFilterAndSort()
        {
            var query = allRequests.AsEnumerable();

            string searchText = txtSearch.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(r =>
                    r.Equipment.ToLower().Contains(searchText) ||
                    r.FaultType.ToLower().Contains(searchText) ||
                    r.Client.ToLower().Contains(searchText) ||
                    r.Description.ToLower().Contains(searchText));
            }

            if (cmbFilterStatus.SelectedItem is ComboBoxItem selectedStatus &&
                selectedStatus.Content.ToString() != "Все")
            {
                string status = selectedStatus.Content.ToString();
                query = query.Where(r => r.Status == status);
            }

            if (cmbSort.SelectedItem is ComboBoxItem selectedSort)
            {
                string sortBy = selectedSort.Content.ToString();
                switch (sortBy)
                {
                    case "По ID":
                        query = query.OrderBy(r => r.Id);
                        break;
                    case "По дате (возр.)":
                        query = query.OrderBy(r => r.CreatedDate);
                        break;
                    case "По дате (убыв.)":
                        query = query.OrderByDescending(r => r.CreatedDate);
                        break;
                    case "По оборудованию":
                        query = query.OrderBy(r => r.Equipment);
                        break;
                    case "По клиенту":
                        query = query.OrderBy(r => r.Client);
                        break;
                }
            }

            filteredRequests.Clear();
            foreach (var item in query)
            {
                filteredRequests.Add(item);
            }

            dgRequests.ItemsSource = filteredRequests;
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

            var newRequest = new RepairRequest
            {
                Id = nextId++,
                Equipment = txtEquipment.Text,
                FaultType = txtFaultType.Text,
                Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Новая",
                Client = txtClient.Text,
                Description = txtDescription.Text,
                CreatedDate = DateTime.Now
            };

            allRequests.Add(newRequest);
            UpdateDataGrid();
            ClearForm();

            MessageBox.Show("Заявка успешно добавлена!", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
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

            selectedRequest.Equipment = txtEquipment.Text;
            selectedRequest.FaultType = txtFaultType.Text;
            selectedRequest.Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Новая";
            selectedRequest.Client = txtClient.Text;
            selectedRequest.Description = txtDescription.Text;

            UpdateDataGrid();
            ClearForm();

            MessageBox.Show("Заявка успешно обновлена!", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
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
                allRequests.Remove(selectedRequest);
                UpdateDataGrid();
                ClearForm();

                MessageBox.Show("Заявка успешно удалена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid();
            ClearForm();
        }

        private void BtnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            cmbFilterStatus.SelectedIndex = 0;
            cmbSort.SelectedIndex = 0;
            UpdateDataGrid();
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
            UpdateDataGrid();
        }

        private void CmbFilterStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDataGrid();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDataGrid();
        }
    }

    public class RepairRequest : INotifyPropertyChanged
    {
        private int id;
        private string equipment;
        private string faultType;
        private string status;
        private string client;
        private string description;
        private DateTime createdDate;

        public int Id
        {
            get => id;
            set { id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Equipment
        {
            get => equipment;
            set { equipment = value; OnPropertyChanged(nameof(Equipment)); }
        }

        public string FaultType
        {
            get => faultType;
            set { faultType = value; OnPropertyChanged(nameof(FaultType)); }
        }

        public string Status
        {
            get => status;
            set { status = value; OnPropertyChanged(nameof(Status)); }
        }

        public string Client
        {
            get => client;
            set { client = value; OnPropertyChanged(nameof(Client)); }
        }

        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged(nameof(Description)); }
        }

        public DateTime CreatedDate
        {
            get => createdDate;
            set { createdDate = value; OnPropertyChanged(nameof(CreatedDate)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}