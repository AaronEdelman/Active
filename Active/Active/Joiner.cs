using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Active
{
    public class Joiner
    {
        public string name;
        public int ratingTotal;
        public int ratingCount;

        public Joiner (string name, int ratingTotal, int ratingCount)
        {
            this.name = name;
            this.ratingTotal = ratingTotal;
            this.ratingCount = ratingCount;
        }

        public string CreateRatingString()
        {
            string rating = GetRatingAverage();
            string count = ConvertCountToString();
            string rating_Count = name + rating + count;
            return rating_Count;
        }
        private string GetRatingAverage()
        {
            string rating = (ratingTotal / ratingCount).ToString();
            return rating;
        }
        private string ConvertCountToString()
        {
            string count = " ("+ratingCount.ToString()+" ratings)";
            return count;
        }
    }
}