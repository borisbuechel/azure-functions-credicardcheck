using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Boris.Function
{
    public static class CreditCardIDCheck
    {
        [FunctionName("CreditCardIDCheck")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string creditCardNumber = req.Query["id"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            creditCardNumber = creditCardNumber ?? data?.id;

            return creditCardNumber != null
                ? (ActionResult)new OkObjectResult($"ID is valid: {idCheck(creditCardNumber)}")
                : new BadRequestObjectResult("Pass a credit card id on the query string or in the request body");
        }

        private static bool idCheck(string creditCardNumber) {
        // check whether input string is null or empty
        if (string.IsNullOrEmpty(creditCardNumber))
        {
            return false;
        }

        int sumOfDigits = creditCardNumber.Where((e) => e >= '0' && e <= '9')
                        .Reverse()
                        .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
                        .Sum((e) => e / 10 + e % 10);


        return sumOfDigits % 10 == 0;
        }
    }
}
