using System;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace CrmPluginTesting
{
    public class ContactPlugin : IPlugin
    {
        private const int _preValidatePluginStage = 10;
        private const int _postOperationPluginStage = 40;
        private const string _createMessage = "Create";

        #region IPlugin Members

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            if (context.MessageName == _createMessage)
            {
                if (context.Stage == _preValidatePluginStage)
                {
                    PreValidateContactCreate(context, service);
                }
                else if (context.Stage == _postOperationPluginStage)
                {
                    PostContactCreate(context, service);
                }
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Pre-Validation method will default the values of contact preference fields
        /// </summary>
        private static void PreValidateContactCreate(IPluginExecutionContext context, IOrganizationService service)
        {
            Entity contactEntity = (Entity)context.InputParameters["Target"];
            OptionSetValue doNotAllow = new OptionSetValue(1);

            contactEntity.SetAttribute("donotemail", doNotAllow);
            contactEntity.SetAttribute("donotpostalmail", doNotAllow);
            contactEntity.SetAttribute("donotbulkemail", doNotAllow);
            contactEntity.SetAttribute("donotfax", doNotAllow);

            // Get a count of child phone call entities associated with this Contact
            QueryExpression query = new QueryExpression();
            query.EntityName = "phonecall";
            query.ColumnSet = new ColumnSet(allColumns: true);
            query.Criteria = new FilterExpression();
            query.Criteria.AddCondition(new ConditionExpression("regardingobjectid", ConditionOperator.Equal, context.PrimaryEntityId));

            RetrieveMultipleRequest request = new RetrieveMultipleRequest();
            request.Query = query;
            IEnumerable<Entity> results = ((RetrieveMultipleResponse)service.Execute(request)).EntityCollection.Entities;
            if (results.Any())
            {
                // Do not default contact preference for phone if there are already some associated phone calls
                // Why? Because! Testing!
                contactEntity.SetAttribute("donotphone", doNotAllow);
            }
                
        }

        /// <summary>
        /// Post Create method will share the Contact with a user specified in the custom 'ShareWithUser' field
        /// </summary>
        private static void PostContactCreate(IPluginExecutionContext context, IOrganizationService service)
        {

        }
        #endregion
    }
}
