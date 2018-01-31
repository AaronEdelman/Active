using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Active
{
    public class Joiner
    {
        public string name;
        public float ratingTotal;
        public float ratingCount;
        public string message;

        public Joiner (string name, int ratingTotal, int ratingCount, string message)
        {
            this.name = name;
            this.ratingTotal = ratingTotal;
            this.ratingCount = ratingCount;
            this.message = message;
        }

        public string CreateRatingString()
        {
            string rating = GetRatingAverage();
            string count = ConvertCountToString();
            string rating_Count = name + " ★" + rating + count;
            return rating_Count;
        }
        private string GetRatingAverage()
        {
            float ratingAvg = (ratingTotal / ratingCount);
            string rating = Math.Round(ratingAvg, 1).ToString();
            return rating;
        }
        private string ConvertCountToString()
        {
            string count = " ("+ratingCount.ToString()+" ratings)";
            return count;
        }

        public string CreateMessageString()
        {
            string messageFull = DateTime.Now.ToShortTimeString() + " - " + name + ": " + message;
            return messageFull;
        }
    }
}