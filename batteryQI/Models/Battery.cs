using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Google.Protobuf.WellKnownTypes;
using System.Windows.Forms;
using System.Drawing;
using SkiaSharp;
using System.IO;
//using Microsoft.ML.O

namespace batteryQI.Models
{
    internal class Battery : ObservableObject
    {
        // 배터리 가용 변수 선언(변동사항 있음. Id나 date 데이터 등은 DB에 기입할 때 필요하지만 관리자가 건들이는 부분은 없음
        //private int _batteryId; // 자동 기입 -> DB에서 기입될 뿐 코드에선 사용 x
        //private DateTime _shootDate; // 자동 기입, 굳이 private으로 선언할 필요 x
        private string _usage = ""; // 사용처, 직접 기입
        private string _batteryType = ""; // 직접 기입
        private string _manufacName = "";
        //private string _manufacId; // 직접 기입(제조사 id를 가져옴)
        private string _batteryShape = ""; // 직접 기입
        private string _shootPlace = "line1"; // 자동 기입(일단은)
        private string? _imagePath = ""; // 파일 로드 경로 저장 -> 이미지 로드
        //private int _managerNum; // 자동 기입(현재 로그인 되어 있는 관리자로)
        private string _defectStat; // 직접 기입(불량, 정상 선택으로)
        private string _defectName; // 직접 기입
        // -------------
        // 배터리 객체도 싱글톤으로 수행
        // 사유: 여러 개의 배터리 객체를 관리하는 것이 아닌 순차적으로 배터리 객체를 활용하기 때문
        // (현재 검사 결과를 수행하지 않으면 다음 검사 불가, 이전 검사 결과는 휘발됨), 메모리 활용성을 위해서 재사용 하는 것으로 로직 변경

        private string modelPath = @".\weight\deeplab_model2.onnx"; // 이미지 처리 모델 로드

        static Battery staticBattery;
        public static Battery Instance()
        {
            if (staticBattery == null)
                staticBattery = new Battery();
            return staticBattery;
        }
        // ----------------
        // ViewModel에서 사용할 변수 선언, imagePath 필요..(이미지 변수에 이미지를 넣을 때, 파일 경로로 로드)
        public string ShootDate
        {
            get { return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"); }
        }
        public string Usage
        {
            get { return _usage; }
            set {SetProperty(ref _usage, value); }
        }
        public string BatteryType
        {
            get { return _batteryType; }
            set { SetProperty(ref _batteryType, value); }
        }
        public string ManufacName
        {
            get { return _manufacName; }
            set { SetProperty(ref _manufacName, value); }
        }
        public string ShootPlace
        {
            get { return _shootPlace; }
            set { SetProperty(ref _shootPlace, value); }
        }
        public string BatteryShape
        {
            get { return _batteryShape; }
            set { SetProperty(ref _batteryShape, value); }
        }
        public string ImagePath
        {
            get { return _imagePath; }
            set { SetProperty(ref _imagePath, value); }
        }
        public string DefectStat
        {
            get { return _defectStat; }
            set { SetProperty(ref _defectStat, value); }
        }
        public string DefectName
        {
            get { return _defectName; }
            set { SetProperty(ref _defectName, value); }
        }
        public BitmapImage BatteryBitmapImage { get; set; }
        // --------------------------
        // 멤버 메소드
        public void batteryInput()
        {
            //DB에 insert관련?
        }
        private DenseTensor<float> PreprocessImage()
        {
            // 이미지 로드
            Bitmap bitmap = new Bitmap(_imagePath);
            int width = bitmap.Width;
            int height = bitmap.Height;

            // ONNX 입력에 맞게 변환(RGB -> Float32 -> CHW)
            var inputTensor = new DenseTensor<float>(new[] { 1, 3, 512, 512 });
            using (var resizedImage = new Bitmap(bitmap, new Size(512, 512)))
            {
                for (int y = 0; y < 512; y++)
                {
                    for (int x = 0; x < 512; x++)
                    {
                        Color pixel = resizedImage.GetPixel(x, y);
                        inputTensor[0, 0, y, x] = pixel.R / 255.0f; // Red
                        inputTensor[0, 1, y, x] = pixel.G / 255.0f; // Green
                        inputTensor[0, 2, y, x] = pixel.B / 255.0f; // Blue
                    }
                }
            }
            return inputTensor;
        }
        private Bitmap Postprocess(Tensor<float> output)
        {
            int numClasses = 4;
            int height = output.Dimensions[1];
            int width = output.Dimensions[2];
            var colorMap = new int[,]
            {
                { 0, 0, 0},
                { 255, 0, 0 },
                { 0, 255, 0 },
                { 0, 0, 255 }
            };

            Bitmap resultBitmap = new Bitmap(width, height);

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x <width; x++)
                {
                    int maxClass = 0;
                    float maxVal = float.MinValue;

                    for(int c = 0; c < numClasses; c++)
                    {
                        float value = output[0, c, y, x];
                        if(value > maxVal)
                        {
                            maxVal = value;
                            maxClass = c;
                        }
                    }

                    Color classColor = Color.FromArgb(
                        colorMap[maxClass, 0],
                        colorMap[maxClass, 1],
                        colorMap[maxClass, 2]);

                    resultBitmap.SetPixel(x, y, classColor);
                }
            }

            // 결과 비트맵을 전역변수 비트맵으로 변환
            //BatteryBitmapImage = 
            return resultBitmap;
        }
        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Bitmap 데이터를 MemoryStream으로 저장
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Position = 0;

                // MemoryStream 데이터를 BitmapImage로 변환
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public void imgProcessing()
        {
            //AI를 통해서 처리하는 부분
            try
            {
                using var session = new InferenceSession(modelPath);

                if (_imagePath != "")
                {
                    var inputTensor = PreprocessImage(); // 이미지 전처리
                    var inputs = new List<NamedOnnxValue> { 
                        NamedOnnxValue.CreateFromTensor("input", inputTensor) 
                    };

                    using var results = session.Run(inputs);
                    var output = results.First().AsTensor<float>();

                    Bitmap result = Postprocess(output);
                    BatteryBitmapImage = BitmapToBitmapImage(result);
                }
                else
                {
                    MessageBox.Show("이미지 업로드 오류", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // 경로가 없으면
                }
            }
            catch(Exception e)
            {   
                MessageBox.Show(e.ToString());
                MessageBox.Show("AI 오류!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
