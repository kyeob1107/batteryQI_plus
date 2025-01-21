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
//using SkiaSharp; // 아마 ScottPlot5 쓸데 있던 것 같은데 왜 여기에 있고, 남아있는지는 모르겠음
using System.IO;
//using Microsoft.ML.O

namespace batteryQI.Models
{
    internal class Battery : ObservableObject
    {
        // 배터리 가용 변수 선언(변동사항 있음. Id나 date 데이터 등은 DB에 기입할 때 필요하지만 관리자가 건들이는 부분은 없음
        private string _usage = ""; // 사용처, 직접 기입
        private string _batteryType = ""; // 직접 기입
        private string _manufacName = "";
        private string _batteryShape = ""; // 직접 기입
        private string _shootPlace = "line1"; // 자동 기입(일단은)
        private string? _imagePath = ""; // 파일 로드 경로 저장 -> 이미지 로드
        private string _defectStat; // 직접 기입(불량, 정상 선택으로)
        private string _defectName; // 직접 기입
        // -------------
        // 배터리 객체도 싱글톤으로 수행
        // 사유: 여러 개의 배터리 객체를 관리하는 것이 아닌 순차적으로 배터리 객체를 활용하기 때문
        
        private string modelPath = @".\weight\deeplab_model5.onnx"; // 이미지 처리 모델 로드

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
            get { return DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"); }
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

            // ONNX 입력에 맞게 변환(RGB -> Float32 -> CHW)
            int targetWidth = 512; // ONNX 모델의 입력 크기
            int targetHeight = 512;

            var inputTensor = new DenseTensor<float>(new[] { 1, 3, targetHeight, targetWidth });
            using (var resizedImage = new Bitmap(bitmap, new Size(targetWidth, targetHeight)))
            {
                for (int y = 0; y < targetHeight; y++)
                {
                    for (int x = 0; x < targetWidth; x++)
                    {
                        Color pixel = resizedImage.GetPixel(x, y);
                        inputTensor[0, 0, y, x] = (pixel.R / 255.0f - 0.5f) / 0.5f; // Red [-1, 1] 정규화
                        inputTensor[0, 1, y, x] = (pixel.G / 255.0f - 0.5f) / 0.5f; // Green
                        inputTensor[0, 2, y, x] = (pixel.B / 255.0f - 0.5f) / 0.5f; // Blue
                    }
                }
            }
            return inputTensor;
        }

        private Bitmap Postprocess(Tensor<float> output)
        {
            int numClasses = 4; // 클래스 수
            int height = output.Dimensions[2]; // 출력 차원
            int width = output.Dimensions[3];
            var colorMap = new int[,]
            {
                { 0, 0, 0 },     // 클래스 0
                { 255, 0, 0 },   // 클래스 1
                { 0, 255, 0 },   // 클래스 2
                { 0, 0, 255 }    // 클래스 3

            };

            Bitmap resultBitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int maxClass = 0;
                    float maxVal = float.MinValue;

                    // 각 픽셀에서 가장 높은 클래스 확률 선택
                    for (int c = 0; c < numClasses; c++)
                    {
                        float value = output[0, c, y, x];
                        if (value > maxVal)
                        {
                            maxVal = value;
                            maxClass = c;
                        }
                    }

                    // 클래스 색상 매핑
                    Color classColor = Color.FromArgb(
                        colorMap[maxClass, 0],
                        colorMap[maxClass, 1],
                        colorMap[maxClass, 2]);

                    resultBitmap.SetPixel(x, y, classColor);
                }
            }
            return resultBitmap;
        }

        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Position = 0;

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
            // AI 처리 로직
            try
            {
                using var session = new InferenceSession(modelPath);

                if (!string.IsNullOrEmpty(_imagePath))
                {
                    var inputTensor = PreprocessImage(); // 이미지 전처리
                    var inputs = new List<NamedOnnxValue>
                    {
                        NamedOnnxValue.CreateFromTensor("input", inputTensor) // ONNX 입력 이름 확인 필요
                    };

                    using var results = session.Run(inputs);
                    var output = results.First().AsTensor<float>();

                    Bitmap result = Postprocess(output); // 출력 후처리
                    BatteryBitmapImage = BitmapToBitmapImage(result); // 결과 이미지 저장
                }
                else
                {
                    MessageBox.Show("이미지 경로가 비어 있습니다.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"오류 발생: {e.Message}\n{e.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
