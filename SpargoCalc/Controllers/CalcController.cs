using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpargoCalc.Infrastructure.Concrete;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SpargoCalc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalcController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> Post()
        {
            string request = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                try
                {
                    request = await reader.ReadToEndAsync();
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            var loanInfo = new ConcreteLoanInfo();
            var (status, result) = loanInfo.GetInfo(request);
            if (status != ConcreteLoanResult.Ok)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    }
}
