using ResumeAnalyzer.Models;
using System.Text.RegularExpressions;

namespace ResumeAnalyzer.Services
{
    public class ResumeAnalyzerService : IResumeAnalyzerService
    {
        private readonly IFileRepository _fileRepository;

        public ResumeAnalyzerService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<ResumeAnalysisResult> AnalyzeAsync(IFormFile resume, IFormFile jobDescription, string JobDescriptionText)
        {
            // Call repository to save files & extract text
            var resumeText = await _fileRepository.SaveFileAndExtractTextAsync(resume);
            string jobDescText;
            if (jobDescription != null)
            {
                jobDescText = await _fileRepository.SaveFileAndExtractTextAsync(jobDescription);
            }
            else 
            {
                // Otherwise use the text input
                jobDescText = JobDescriptionText;
            }

            // Business logic: calculate match %
            double matchPercent = CalculateMatchPercentage(resumeText, jobDescText);

            return new ResumeAnalysisResult
            {
                ResumeText = resumeText,
                JobDescriptionText = jobDescText,
                MatchPercentage = matchPercent
            };
        }
        string CleanText(string text)
        {
            var cleaned = new string(text.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
            return cleaned.ToLower();
        }
        List<string> GenerateNGrams(string[] words, int n)
        {
            var ngrams = new List<string>();
            for (int i = 0; i <= words.Length - n; i++)
            {
                ngrams.Add(string.Join(" ", words.Skip(i).Take(n)));
            }
            return ngrams;
        }
        public static Dictionary<string, double> BuildWordVector(string text, bool removeStopwords = true)
        {
            text = Normalize(text);
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Optional stopwords removal
            if (removeStopwords)
            {
                var stopwords = new HashSet<string> {
                "a", "an", "the", "and", "or", "in", "on", "for", "with", "of", "to", "at", "by"
            };
                words = words.Where(w => !stopwords.Contains(w)).ToArray();
            }

            var vec = new Dictionary<string, double>();
            foreach (var word in words)
            {
                if (vec.ContainsKey(word))
                    vec[word]++;
                else
                    vec[word] = 1;
            }

            return vec;
        }
        Dictionary<string, double> ComputeTfIdf(string[] words, string[] corpus)
        {
            var tf = new Dictionary<string, double>();
            int totalWords = words.Length;

            foreach (var word in words.Distinct())
            {
                tf[word] = words.Count(w => w == word) / (double)totalWords;
            }

            var idf = new Dictionary<string, double>();
            foreach (var word in words.Distinct())
            {
                int docCount = corpus.Count(doc => doc.Split(' ').Contains(word));
                idf[word] = Math.Log((1 + corpus.Length) / (1 + docCount)) + 1;
            }

            var tfidf = new Dictionary<string, double>();
            foreach (var word in words.Distinct())
            {
                tfidf[word] = tf[word] * idf[word];
            }
            return tfidf;
        }


        private static string Normalize(string text)
        {
            text = text.ToLower();
            text = Regex.Replace(text, @"[^\w\s]", "");  // remove punctuation
            text = Regex.Replace(text, @"\s+", " ");     // collapse spaces
            return text.Trim();
        }
   
        double CosineSimilarityJobFactor1(Dictionary<string, double> jdVec, Dictionary<string, double> resumeVec)
        {
            double dot = 0;
            double normJD = 0;

            foreach (var word in jdVec.Keys) // iterate only over JD words
            {
                double x = jdVec[word]; // value in JD
                double y = resumeVec.ContainsKey(word) ? resumeVec[word] : 0; // value in Resume
                dot += x * y;
                normJD += x * x;
            }

            double normResume = Math.Sqrt(resumeVec.Values.Sum(v => v * v));

            // Avoid divide by zero
            if (normJD == 0 || normResume == 0)
                return 0;

            return dot / (Math.Sqrt(normJD) * normResume);
        }




        double CosineSimilarityJobFactor(Dictionary<string, double> jdVec, Dictionary<string, double> resumeVec)
        {
            double dot = 0;
            double normJD = 0;

            foreach (var word in jdVec.Keys) // only iterate over Job Description words
            {
                double x = jdVec[word];                        // value in JD
                double y = resumeVec.ContainsKey(word) ? resumeVec[word] : 0; // value in resume
                dot += x * y;
                normJD += x * x;
            }

            double normResume = Math.Sqrt(resumeVec.Values.Sum(v => v * v));

            // Avoid divide by zero
            if (normJD == 0 || normResume == 0)
                return 0;

            return dot / (Math.Sqrt(normJD) * normResume);
        }





        public double CalculateMatchPercentage(string resumeText, string jobDescText)
        {
            resumeText = CleanText(resumeText);
            jobDescText = CleanText(jobDescText);

            var resumeWords = resumeText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var jobWords = jobDescText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Optional: generate n-grams
            var resumeNgrams = GenerateNGrams(resumeWords, 3); // 2-grams
            var jobNgrams = GenerateNGrams(jobWords, 3);

            // Compute TF-IDF
            var corpus = new string[] { resumeText, jobDescText };          
            var resumeTfIdf = ComputeTfIdf(resumeWords, corpus);
            var jobTfIdf = ComputeTfIdf(jobWords, corpus);
            var jd = BuildWordVector(jobDescText);
            var re = BuildWordVector(resumeText);
            
            // Cosine similarity
           // double similarity = CosineSimilarityJobFactor1(jd, re);
           double similarity = CosineSimilarityJobFactor1(resumeTfIdf, jobTfIdf);

            return Math.Round(similarity * 100, 2); // percentage
        }
        public async Task<ResumeAnalysisResult> AnalyzeAsyncFromText(string resumeText, string jobDescText)
        {
            double matchPercent = CalculateMatchPercentage(resumeText, jobDescText);

            return new ResumeAnalysisResult
            {
                ResumeText = resumeText,
                JobDescriptionText = jobDescText,
                MatchPercentage = matchPercent
            };
        }


    }

}
