using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DemoApp.API.ApiModels;
using DemoApp.API.Attributes;
using DemoApp.DataAccess.Services;
using DemoApp.Localization.Localizers.SharedLocalizer;
using DemoApp.Models.AppSettings;
using DemoApp.Models.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace DemoApp.API.Controllers
{
    [ApiController]
    [AuthorizeJwt]
    [Route("api/[controller]/[action]")]
    [ValidateModel]
    [Produces("application/json")]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    [ProducesResponseType(Status422UnprocessableEntity)]
    public class AppBaseController<TController> : ControllerBase where TController : class
    {
        #region Private fields & Constructors

        protected readonly ILogger<TController> _logger;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        protected readonly AppSettings _appSettings;

        protected readonly IStringLocalizerFactory _localizerFactory;
        protected readonly IStringLocalizer _sharedLocalizer;

        public AppBaseController(IServiceProvider serviceProvider)
        {
            _logger = (ILogger<TController>)serviceProvider.GetService(typeof(ILogger<TController>));
            _unitOfWork = (IUnitOfWork)serviceProvider.GetService(typeof(IUnitOfWork));
            _mapper = (IMapper)serviceProvider.GetService(typeof(IMapper));

            _appSettings = (AppSettings)serviceProvider.GetService(typeof(AppSettings));

            _localizerFactory = (IStringLocalizerFactory)serviceProvider.GetService(typeof(IStringLocalizerFactory));
            _sharedLocalizer = _localizerFactory.Create(typeof(SharedLocalizer));
        }

        #endregion

        #region Methods

        #region 200 OK

        [ApiExplorerSettings(IgnoreApi = true)]
        public new OkObjectResult Ok()
        {
            return base.Ok(true);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public OkObjectResult Ok(string message)
        {
            return Ok(_sharedLocalizer["Done"], new List<string> { message });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public OkObjectResult Ok(string title, string message)
        {
            return Ok(title, new List<string> { message });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public OkObjectResult Ok(string title, List<string> messages)
        {
            return Ok(new CommonResultModel(title, messages, true));
        }

        #endregion

        #region 204 NoContent

        [ApiExplorerSettings(IgnoreApi = true)]
        public new ObjectResult NoContent()
        {
            return StatusCode(Status204NoContent,
                new CommonResultModel(_sharedLocalizer["Done"],
                    new List<string> { _sharedLocalizer["NoDataChange"] }));
        }

        #endregion

        #region 403 Forbidden

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult Forbidden(string title, string message)
        {
            return Forbidden(title, new List<string> { message });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult Forbidden(string title, List<string> messages)
        {
            return StatusCode(Status403Forbidden, new CommonResultModel(title, messages));
        }

        #endregion

        #region 404 NotFound

        [ApiExplorerSettings(IgnoreApi = true)]
        public new ObjectResult NotFound()
        {
            return NotFound(_sharedLocalizer["NotFound"], _sharedLocalizer["NotFoundAnyData"]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult NotFound(string title, string message)
        {
            return NotFound(title, new List<string> { message });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult NotFound(string title, List<string> messages)
        {
            return StatusCode(Status404NotFound, new CommonResultModel(title, messages));
        }

        #endregion

        #region 422 UnprocessableEntity

        [ApiExplorerSettings(IgnoreApi = true)]
        public new ObjectResult UnprocessableEntity()
        {
            return UnprocessableEntity(_sharedLocalizer["AnErrorOccurred"], new List<string> { _sharedLocalizer["SomethingWentWrong"] });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult UnprocessableEntity(string message)
        {
            return UnprocessableEntity(_sharedLocalizer["AnErrorOccurred"], new List<string> { message });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult UnprocessableEntity(List<string> message)
        {
            return UnprocessableEntity(_sharedLocalizer["AnErrorOccurred"], message);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult UnprocessableEntity(string title, string message)
        {
            return UnprocessableEntity(title, new List<string> { message });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult UnprocessableEntity(string title, List<string> messages)
        {
            return StatusCode(Status422UnprocessableEntity, new CommonResultModel(title, messages));
        }

        #endregion

        #region GenerateResponse

        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult GenerateResponse(ServiceResultModel resultModel)
        {
            if (resultModel.IsNotFound)
            {
                return NotFound();
            }
            else if (resultModel.Success)
            {
                if (resultModel.Data != null)
                    return Ok(resultModel.Data);
                else if (resultModel.Messages != null)
                    return Ok(resultModel.Messages.FirstOrDefault());
                else
                    return Ok();
            }
            else
            {
                return UnprocessableEntity(resultModel.Messages);
            }
        }

        #endregion

        #endregion
    }
}
