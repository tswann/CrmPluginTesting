using System;
using Microsoft.Xrm.Sdk;

namespace CrmPluginTesting
{
    public class ContactPlugin : IPlugin
    {
        private const int _preValidatePluginStage = 10;
        private const int _postOperationPluginStage = 40;

        #region IPlugin Members

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            if (context.Stage == _preValidatePluginStage)
            {
                PreValidateContactCreate(context, service);
            }
            else if (context.Stage == _postOperationPluginStage)
            {
                PostContactCreate(context, service);
            }
        }

        #endregion

        #region Methods
        private static void PreValidateContactCreate(IPluginExecutionContext context, IOrganizationService service)
        {

        }

        private static void PostContactCreate(IPluginExecutionContext context, IOrganizationService service)
        {

        }
        #endregion
    }
}
