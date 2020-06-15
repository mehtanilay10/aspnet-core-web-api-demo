using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace DemoApp.Models.ServiceModels
{
    public class ServiceResultModel
    {
        public bool Success { get; set; }
        public bool IsNotFound { get; set; }
        public List<string> Messages { get; set; }
        public dynamic Data { get; set; }

        public ServiceResultModel()
        {

        }

        public ServiceResultModel(bool success, params string[] messages)
        {
            Success = success;
            Messages = messages?.ToList() ?? new List<string>();
        }

        public ServiceResultModel(IdentityResult identityResult)
        {
            Success = identityResult.Succeeded;
            Messages = identityResult.Errors?.Where(x => !x.Code.EndsWith("UserName")).Select(x => x.Description).ToList();
        }

        public ServiceResultModel(dynamic data)
        {
            Success = true;
            Data = data;
        }
    }
}
