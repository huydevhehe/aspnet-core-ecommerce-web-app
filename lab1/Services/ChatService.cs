using Microsoft.ML;
using System;
using System.IO;
using lab1.Models;

namespace lab1.Services
{
    public class ChatService
    {
        private static readonly string modelPath = "wwwroot/mlmodel.zip";
        private PredictionEngine<ChatData, ChatPrediction> _predictionEngine;

        public ChatService()
        {
            if (File.Exists(modelPath))
            {
                var context = new MLContext();
                ITransformer trainedModel = context.Model.Load(modelPath, out var modelInputSchema);
                _predictionEngine = context.Model.CreatePredictionEngine<ChatData, ChatPrediction>(trainedModel);
            }
            else
            {
                Console.WriteLine("⚠ Chưa có mô hình! Hãy train AI trước.");
            }
        }

        public string GetAnswer(string userQuestion)
        {
            if (_predictionEngine == null) return "Xin lỗi, tôi chưa được train.";

            var prediction = _predictionEngine.Predict(new ChatData { Question = userQuestion });
            return prediction.Answer ?? "Xin lỗi, tôi chưa hiểu câu hỏi của bạn!";
        }
    }
}
