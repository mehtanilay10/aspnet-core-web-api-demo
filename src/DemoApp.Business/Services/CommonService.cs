using System;
using System.Collections.Generic;
using DemoApp.Models.ApiModels.Common;
using DemoApp.Models.Enums;
using DemoApp.Models.Helpers;

namespace DemoApp.Business.Services
{
    #region Interface

    [AspNetCore.ServiceRegistration.Dynamic.Attributes.TransientService]
    public interface ICommonService : IServiceBase
    {
        Dictionary<string, IEnumerable<EnumDetails>> GetEnumList();
    }

    #endregion

    public class CommonService : ServiceBase<CommonService>, ICommonService
    {
        #region Private Properties & Constructor

        public CommonService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        #endregion

        #region Implementation

        #region Create account related

        public Dictionary<string, IEnumerable<EnumDetails>> GetEnumList()
        {
            var returnData = new Dictionary<string, IEnumerable<EnumDetails>>
            {
                { "SupportedCulture", EnumHelpers<SupportedCulture>.GetEnumDetails() }
            };

            return returnData;
        }

        #endregion

        #endregion
    }
}
