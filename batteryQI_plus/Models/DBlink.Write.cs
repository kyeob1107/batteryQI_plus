using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using MySql.Data.MySqlClient;

namespace batteryQI_plus.Models
{
    public partial class DBlink
    {
        // DB insert
        public bool Insert(string sql)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, this.connection); // sql 실행
                if (cmd.ExecuteNonQuery() == 1) // 정상 수행 완료
                    return true;
                else
                    return false;
            }
            catch
            {
                return false; // db insert 에러
            }
        }

        public void Update(string sql)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, this.connection);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                MessageBox.Show("데이터가 반영되지 않았습니다!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
