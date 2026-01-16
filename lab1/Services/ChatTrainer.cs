using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using lab1.Models;

namespace lab1.Services
{
    public class ChatTrainer
    {
        private static readonly string modelPath = "wwwroot/mlmodel.zip";

        public void TrainModel()
        {
            var context = new MLContext();

            // Dữ liệu mẫu (bạn có thể thêm câu hỏi và câu trả lời tại đây)
            var data = new List<ChatData>
            {
                new ChatData { Question = "xin chào", Answer = "Chào bạn! Tôi là chatbox do huydz tạo ra tôi có thể giúp gì?" },
                new ChatData { Question = "bạn tên gì", Answer = "Tôi là chatbot hỗ trợ khách hàng." },
                new ChatData { Question = "cách xem giá", Answer = "Bạn có thể xem giá trên danh sách sản phẩm!" },
                new ChatData { Question = "cách đặt hàng", Answer = "Bạn có thể thêm sản phẩm vào giỏ hàng và thanh toán." },
                new ChatData { Question = "mua hàng như thế nào", Answer = "Bạn có thể đặt hàng bằng cách nhấn vào nút 'Thêm vào giỏ'." },
                new ChatData { Question = "cảm ơn", Answer = "Không có gì Nếu cần bạn có thể tiếp tục hỏi chúng tôi." },
                new ChatData { Question = "khách hàng là ", Answer = "Thượng đế." },
                new ChatData { Question = "thầy ánh ", Answer = "Quá Vip Quá Đẹp Trai ." },

            };

            var trainingData = context.Data.LoadFromEnumerable(data);

            // Xây dựng pipeline xử lý dữ liệu và train model
            var pipeline = context.Transforms.Text.FeaturizeText("Features", "Question")
                .Append(context.Transforms.Conversion.MapValueToKey("Label", "Answer"))
                .Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // Train model
            var model = pipeline.Fit(trainingData);
            context.Model.Save(model, trainingData.Schema, modelPath);
            Console.WriteLine("✅ AI đã được train và lưu vào file!");
        }
    }
}
