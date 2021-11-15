using Markov;
using Microsoft.EntityFrameworkCore;
using ReviewGenerator.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewGenerator
{
    public class ReviewService : IReviewService
    {
        // time crunch.. not doing this for now
        //private ISentimentAnalysisService _sentimentAnalysisService;
        //public ReviewService(ISentimentAnalysisService sentimentAnalysisService)
        //{
        //    _sentimentAnalysisService = sentimentAnalysisService;
        //}


        private Context _dbContext;
        public ReviewService(Context dbContext)
        {
            _dbContext = dbContext;
        }

        public Review GetBotGeneratedReview()
        {
            var rand = new Random();
            var randomReviewId = rand.Next(1, _dbContext.Reviews.CountAsync().Result);

            //pick a random review
            var review = _dbContext.Reviews.FirstOrDefaultAsync(x => x.ReviewId == randomReviewId).Result;
            while (review is null)
            {
                randomReviewId = rand.Next(1, GzipReader.reviewsCount);
                review = _dbContext.Reviews.FirstOrDefaultAsync(x => x.ReviewId == randomReviewId).Result;
            }

            // pick some reviews for the Asin/productid in the review
            var skip = rand.Next(1, 5);
            var reviews = _dbContext.Reviews.Where(x => x.Asin == review.Asin).Skip(skip).Take(5);

            // Create Markov Chain using collected reviews for a given product
            List<string[]> chainArray = new List<string[]>();
            var chain = new MarkovChain<string>(1);
            foreach (var rw in reviews)
            {
                string[] lines = rw.ReviewText.Split(new[] { '.' });
                foreach (string line in lines)
                {
                    // we need fullstops/periods in the random generated review
                    var jline = string.Concat(line, ".");
                    string[] words = jline.Split(" ");
                    chainArray.Add(words);
                    chain.Add(words, 1);
                }
            }

            // generate random sentences for review for the given product
            StringBuilder strBldr = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                var sentence = string.Join(" ", chain.Chain(rand));
                strBldr.Append(sentence);
            }

            var resultPrediction = SentimentAnalysis.PredictRating(strBldr.ToString());
            // if predicion is positive high rating, else low rating
            double score = resultPrediction.Prediction ? rand.Next(3, 5) : rand.Next(1, 3);

            return new Review
            {
                ReviewText = strBldr.ToString(),
                Overall = score,
                Asin = review.Asin,
            };
        }
    }
}
