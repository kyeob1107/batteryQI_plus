using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows;

namespace batteryQI_plus.ViewModels
{
    public partial class AnalysisViewModel
    {
        // 테이블 뷰
        private DataView _queryResultsTable;
        public DataView QueryResultsTable
        {
            get => _queryResultsTable;
            set => SetProperty(ref _queryResultsTable, value);
        }

        // 이름 DrawTable로 할까 고민중
        private void LoadData(string filter = "TRUE")
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) // 디자인 타임(모드) 동안 DB 연결이 수행되는것을 방지
                return;

            try
            {
                string tablequery = $"SELECT * FROM inspectionResults WHERE {filter}";
                Console.WriteLine("테이블: " + tablequery);
                // 이 부분 using 사용하는 것으로 수정하기
                MySqlCommand cmd = new MySqlCommand(tablequery, _dblink.connection);

                DataTable dataTable = new DataTable();
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                }

                QueryResultsTable = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 로드 중 오류 발생: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
