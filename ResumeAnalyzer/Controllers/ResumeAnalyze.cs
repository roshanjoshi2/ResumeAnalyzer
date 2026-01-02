using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzer.Models;
using ResumeAnalyzer.Services;

namespace ResumeAnalyzer.Controllers
{
    public class ResumeAnalyze : Controller
    {
        private readonly IResumeAnalyzerService _analyzerService;
    
        public ResumeAnalyze(IResumeAnalyzerService analyzerService)
        {
            _analyzerService = analyzerService;
        }
 
        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }

        //[HttpPost]
        //public async Task<IActionResult> Index(JobApplication model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _analyzerService.AnalyzeAsync(model.Resume, model.JobDescription);
        //        return View("Result", result);
        //    }
        //    return View(model);
        //}
      
        [HttpPost]
        public async Task<IActionResult> Index(JobApplication model)
        {
            if (model.Resume == null)
            {
                ModelState.AddModelError(nameof(model.Resume), "Resume is required.");
            }

            if (model.JobDescription == null && string.IsNullOrWhiteSpace(model.JobDescriptionText))
            {
                ModelState.AddModelError("", "Please provide a job description (file or text).");
            }
            if((model.Resume!= null && model.JobDescription!= null)||(model.Resume!=null && model.JobDescriptionText!= null))
            {
                var result = await _analyzerService.AnalyzeAsync(model.Resume, model.JobDescription,model.JobDescriptionText);
                return View("Result", result);
            }


            return View(model);

        }

    }
}
