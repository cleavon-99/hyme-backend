using Hyme.Application.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Hyme.API.Controllers
{


    /// <summary>
    /// Validation Handler
    /// </summary>
    public class ApiBase : ControllerBase
    {
        /// <summary>
        /// Handling Validatin
        /// </summary>
        /// <param name="errors">Validaton Errors</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult Problem(IEnumerable<ValidationError> errors)
        {
            foreach (var e in errors)
            {
                ModelState.AddModelError(e.PropertyName, e.Message);
            }
            return ValidationProblem(ModelState);
        }
    }
}
