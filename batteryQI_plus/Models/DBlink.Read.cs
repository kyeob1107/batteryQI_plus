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
        public List<Dictionary<string, object>> Select(string sql)
        {
            // 간단한 Select문 메소드, 불러오는 데이터가 크면 그냥 직접 Select을 하는 것을 추천
            // 결과 저장 List
            List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();
            try
            {
                using(MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    using(MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            // 데이터 필드에 따른 길이
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                object value = reader.GetValue(i);
                                row[columnName] = value;
                            }
                            resultList.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (connection == null)
                    MessageBox.Show($"데이터베이스 접속 오류 \r\n 에러메시지: {ex.Message} 연결 안됨 에러위치: {ex.StackTrace}", 
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show($"데이터베이스 접속 오류 \r\n 에러메시지: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return resultList;
        }
        

        // DB select count(*) action
        public CountResult? CountQuery(string table, string groupingCriteria, string mode = "label")
        {
            CountResult result = new CountResult();
            
            // label이 default
            string query = @$"
                        SELECT
	                        {groupingCriteria},
	                        Count(*)
                        FROM
	                        {table}
                        GROUP BY
	                        {groupingCriteria};";

            if (mode == "timestamp")
            {
                query = @$"
                            SELECT
                             DATE_FORMAT({groupingCriteria}, '%Y-%m-%d %H:00:00') AS hour_interval,
                             COUNT(*) AS count
                            FROM
                             {table}
                            WHERE 
                                defectStat = 1
                            GROUP BY
                             hour_interval
                            ORDER BY
                             hour_interval;";
            }

            MySqlCommand cmd = new MySqlCommand(query, this.connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.defectGroups.Add(reader[0]);
                result.counts.Add(reader.GetDouble(1));
            }
            reader.Close();

            return result;

        }

        // 저장할 구조가 달라서 따로 선언
        public List<(string, string, int)> GroupCountQuery(string table, string groupingCriteria, string mode = "label")
        {
            List<(string, string, int)>result = new List<(string, string, int)>();
            string query = @$"
                        SELECT
	                        {groupingCriteria},
	                        Count(*)
                        FROM
	                        {table}
                        GROUP BY
	                        {groupingCriteria};";
            
            MySqlCommand cmd = new MySqlCommand(query, this.connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add((reader.GetString(0), reader.GetString(1), reader.GetInt16(2)));
            }
            reader.Close();

            return result;
        }
    }
}
