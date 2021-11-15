using EFCore.BulkExtensions;
using ICSharpCode.SharpZipLib.GZip;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReviewGenerator.DataModels;
using System.Collections.Generic;
using System.IO;

namespace ReviewGenerator
{
    public static class GzipReader
    {


        public static List<string[]> reviewWords = new List<string[]>();
        // added count variable because, it is faster to read this variable in reviewService
        // than having to run a iterator to get the count of sentences
        public static int reviewsCount = 0;

        public static async void ReadGzipFileAsync(string filepath)
        {
            //reference used 
            //https://www.youtube.com/watch?v=w3hc7nxXxf4
            //Interesting pattern with yield return and Async function


            // gets the reviews and appends words in reviewWords static variables which would then be available app wide
            var reviews = GetReviews(filepath);

            List<Review> reviewsList = new List<Review>();
            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=ReviewGenerator;Trusted_Connection=True;");
            using (Context context = new Context(optionsBuilder.Options))
            {
                await foreach (var review in reviews)
                {
                    reviewsList.Add(review);
                    reviewsCount++;

                    if (reviewsCount % 10000 == 0)
                    {
                        await context.BulkInsertAsync(reviewsList);
                        reviewsList.Clear();
                    }
                }
                await context.BulkInsertAsync(reviewsList);
            }

        }

        public static async IAsyncEnumerable<Review> GetReviews(string filePath)
        {
            // reference used
            //https://www.codeart.dk/blog/2020/5/reading-very-large-gzipped-json-files-in-c/

            using (Stream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (GZipInputStream gzipStream = new GZipInputStream(fs))
            using (StreamReader streamReader = new StreamReader(gzipStream))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                reader.SupportMultipleContent = true;
                var serializer = new JsonSerializer();
                while (await reader.ReadAsync())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        var review = serializer.Deserialize<Review>(reader);
                        //Add custom logic here - perhaps a yield return?
                        yield return review;
                    }
                }
            }
        }
    }
}
