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
                MySqlConnection connection = new MySqlConnection(myConnection);
                connection.Open(); // DB 오픈
            }
            catch(Exception E)
            {
                MessageBox.Show(E.ToString());
            }
        }
    }
}
