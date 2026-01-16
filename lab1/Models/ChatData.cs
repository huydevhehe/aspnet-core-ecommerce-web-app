using Microsoft.ML.Data;

namespace lab1.Models
{
    public class ChatData
    {
        [LoadColumn(0)] public string Question { get; set; }
        [LoadColumn(1)] public string Answer { get; set; }
    }

    public class ChatPrediction
    {
        [ColumnName("PredictedLabel")] public string Answer { get; set; }
    }
}
