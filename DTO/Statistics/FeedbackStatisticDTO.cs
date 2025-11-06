using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Statistics
{
    public class FeedbackStatisticDTO
    {
        public string Type { get; set; } = null!;  // "Positive" hoặc "Negative"
        public int Count { get; set; }             // Số lượng đánh giá
        public double Percentage { get; set; }
    }

}