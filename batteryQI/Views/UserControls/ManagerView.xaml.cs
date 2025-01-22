using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using batteryQI.ViewModels;

namespace batteryQI.Views.UserControls
{
    /// <summary>
    /// Interaction logic for ManagerPage.xaml
    /// </summary>
    public partial class ManagerView : UserControl
    {
        public ManagerView()
        {
            InitializeComponent();
            this.DataContext = new ManagerViewModel();
        }

        // 텍스트 입력 검사 이벤트 핸들러.새 작업량 설정에 정수만 입력 가능하도록 규제
        private void InspectionLimitBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textBox)
                return;
            // 입력된 문자열
            string input = textBox.Text;
            // 숫자만 남기기
            string numericInput = Regex.Replace(input, "[^0-9]", "");
            // 텍스트 박스가 비어 있을 경우 0으로 초기화
            if (numericInput.Length == 0)
            {
                numericInput = "0";
            }
            // "0"만 여러 개 입력되지 않도록 제한
            else if (numericInput.Length > 1 && numericInput.StartsWith("0"))
            {
                numericInput = numericInput.TrimStart('0');
            }
            // 변경된 텍스트가 원본과 다를 경우만 업데이트
            if (input != numericInput)
            {
                int caretIndex = textBox.CaretIndex; // 현재 커서 위치 저장
                textBox.Text = numericInput; // 수정된 텍스트 설정
                textBox.CaretIndex = Math.Max(caretIndex, numericInput.Length); // 커서 위치 복원
            }
        }
    }
}
