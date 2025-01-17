using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;


namespace batteryQI.Models
{
    internal class DBlink : ObservableObject
    {
        private string _server = ""; // _server : ip 주소
        private string _port = ""; // _port : 포트번호
        private string _dbName = ""; // _dbName : 연결 스키마
        private string _dbId = ""; // _dbId : 접속아이디
        private string _dbPw = ""; // _dbpw : 접속패스워드

        static DBlink staticDBlink; // DB 연결 객체 생성
        MySqlConnection connection; // DB connection 객체
        public static DBlink Instance()
        {
            if(staticDBlink == null)
            {
                staticDBlink = new DBlink();
            }
            return staticDBlink;
        }
        private void setDBLink()
        {
            string relativePath = @".\Models\DB.txt";
            string fullPath = Path.Combine(Environment.CurrentDirectory, relativePath);
            StreamReader sr = new StreamReader(fullPath);
            string[] Data = sr.ReadToEnd().Split("\n");
            _server = Data[0];
            _port = Data[1];
            _dbName = Data[2];
            _dbId = Data[3];
            _dbPw = Data[4];
        }
        public void Connect()
        {
            this.setDBLink();
            string myConnection = "Server="+_server + ";Port=" + _port + ";Database=" + _dbName + ";User Id = " + _dbId + ";Password = " + _dbPw;
            try
            {
                connection = new MySqlConnection(myConnection);
                connection.Open(); // DB 오픈
            }
            catch(Exception E)
            {
                MessageBox.Show(E.ToString());
            }
        }
        // DB 연결 확인 함수
        public bool ConnectOk()
        {
            try
            {
                if (connection.Ping())
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        // DB insert
        public bool Insert(string sql)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection); // sql 실행
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
        public bool Select(string sql)
        {
            return false; // 테이블
        }

        public void Disconnect()
        {
            connection.Close(); // 연결 해제
        }
    }
}
