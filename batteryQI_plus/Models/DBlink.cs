using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Google.Protobuf;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;


namespace batteryQI.Models
{
    // DB select count결과 저장용도 - 추후 따로 cs분리해서 넣을까 고민중
    public class CountResult
    {
        public List<object> defectGroups { get; set; }
        public List<double> counts { get; set; }

        public CountResult()
        {
            defectGroups = new List<object>();
            counts = new List<double>();
        }
    }

    public partial class DBlink : ObservableObject, IDisposable
    {
        private string _server = ""; // _server : ip 주소
        private string _port = ""; // _port : 포트번호
        private string _dbName = ""; // _dbName : 연결 스키마
        private string _dbId = ""; // _dbId : 접속아이디
        private string _dbPw = ""; // _dbpw : 접속패스워드

        static DBlink staticDBlink; // DB 연결 객체 생성
        MySqlConnection connection; // DB connection 객체

        private DBlink() { } // 생성자 접근 제어 변경
        public static DBlink Instance()
        {
            if(staticDBlink == null)
            {
                staticDBlink = new DBlink();
            }
            return staticDBlink;
        }
        // -----------
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
            string myConnection = "Server="+_server + 
                                    ";Port=" + _port + 
                                    ";Database=" + _dbName + 
                                    ";User Id = " + _dbId + 
                                    ";Password = " + _dbPw + 
                                    ";CharSet=utf8;";
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

        // 연결 해제 및 리소스 정리
        public void Dispose() 
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close(); // 연결 해제
                connection.Dispose();
                connection = null;
            }
        }
    }
}
