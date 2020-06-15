using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace DemoApp.API.ApiModels
{
    /// <summary>
    /// Common Model class that will used to generate any kind of response
    /// Including validation errors for 422
    /// </summary>
    public class CommonResultModel
    {
        public bool Success { get; set; } = false;

        public string TitleMessage { get; }

        public List<ContentMessage> ContentMessages { get; }

        /// <summary>
        /// Used when any error generate while performing operation
        /// Manually used in almost actions
        /// </summary>
        /// <param name="titleMessage">String message representing error details</param>
        /// <param name="allMessages">List of error message</param>
        public CommonResultModel(string titleMessage, List<string> allMessages = null, bool success = false)
        {
            Success = success;
            TitleMessage = titleMessage;
            ContentMessages = allMessages
                    ?.Select(m => new ContentMessage(null, m))
                    .ToList();
        }

        /// <summary>
        /// Used when any error generate while performing operation
        /// Manually used in almost actions
        /// </summary>
        /// <param name="allMessages">Dictionary of field name & error message text</param>
        public CommonResultModel(Dictionary<string, string> allMessages)
        {
            Success = false;
            TitleMessage = "Validation Failed";
            ContentMessages = allMessages
                    ?.Select(m => new ContentMessage(m.Key, m.Value))
                    .ToList();
        }

        /// <summary>
        /// Used in Validation of Model
        /// </summary>
        /// <param name="modelState"></param>
        public CommonResultModel(ModelStateDictionary modelState)
        {
            Success = false;
            TitleMessage = "Validation Failed";
            ContentMessages = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new ContentMessage(key, x.ErrorMessage)))
                    .ToList();
        }
    }

    /// <summary>
    /// Model class that contains error details for single field in Model
    /// </summary>
    public class ContentMessage
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        public string Message { get; }

        public ContentMessage(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }
}