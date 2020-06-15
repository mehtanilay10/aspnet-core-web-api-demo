using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DemoApp.Business.Services;
using DemoApp.Models.ApiModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace DemoApp.API.Controllers
{
    public class CommonController : AppBaseController<AuthController>
    {
        private readonly ICommonService _commonService;

        #region Constructor

        public CommonController(ICommonService commonService, IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _commonService = commonService;
        }

        #endregion

        #region Actions

        #region Logs

        /// <summary>
        /// Add text message log to log file
        /// Used directly from dashboard to write Javascript errors
        /// </summary>
        /// <param name="logMessage">Exception details</param>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(Status200OK)]
        public IActionResult LogError([Required][FromBody] string logMessage)
        {
            _logger.LogError(logMessage);
            return Ok();
        }

        /// <summary>
        /// Write log message at Info level
        /// </summary>
        /// <param name="logMessage">String message</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(Status200OK)]
        public IActionResult LogInfo([Required][FromBody] string logMessage)
        {
            _logger.LogInformation(logMessage);
            return Ok();
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Dictionary<string, IEnumerable<EnumDetails>>), Status200OK)]
        public IActionResult GetEnums()
        {
            var enums = _commonService.GetEnumList();
            return Ok(enums);
        }

        #endregion
    }
}
