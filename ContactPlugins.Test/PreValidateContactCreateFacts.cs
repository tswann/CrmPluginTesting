using System;
using System.Moles;
using CrmPluginTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Moles;
using Should.Fluent;
using Xunit;

namespace ContactPlugins.Test
{
    public class PreValidateContactCreateFacts
    {
        private SIServiceProvider _serviceProvider;
        private SIPluginExecutionContext _pluginExecutionContext;
        private SIOrganizationServiceFactory _orgServiceFactory;
        private SIOrganizationService _orgService;

        /// <summary>
        /// Constructor performs test setup of CRM service stubs
        /// </summary>
        public PreValidateContactCreateFacts()
        {
            _serviceProvider = new SIServiceProvider();
            _pluginExecutionContext = new SIPluginExecutionContext();
            _orgServiceFactory = new SIOrganizationServiceFactory();
            _orgService = new SIOrganizationService();

            // Provide stub implementations
            _serviceProvider.GetServiceType = (Type type) =>
            {
                if (type.Equals(typeof(IPluginExecutionContext)))
                    return _pluginExecutionContext;
                else if (type.Equals(typeof(IOrganizationServiceFactory)))
                    return _orgServiceFactory;
                return null;
            };

            _orgServiceFactory.CreateOrganizationServiceNullableOfGuid = delegate(Guid? userid) { return _orgService; };

            _pluginExecutionContext.UserIdGet = () => { return new Guid(); };
        }
        #region Tests

        [Fact]
        public void DefaultContactPreferencesSet()
        {
            ContactPlugin plugin = new ContactPlugin();
            Entity contact = new Entity("contact");
            ParameterCollection parameterCollection = new ParameterCollection();
            parameterCollection.Add("Target", contact);
            _pluginExecutionContext.InputParametersGet = () => { return parameterCollection; };
            _pluginExecutionContext.MessageNameGet = () => { return "Create"; };
            _pluginExecutionContext.StageGet = () => { return 10; };
            plugin.Execute(_serviceProvider);

            OptionSetValue doNotAllow = new OptionSetValue(1);
            contact.Attributes["donotemail"].Should().Equal(doNotAllow);
            contact.Attributes["donotphone"].Should().Equal(doNotAllow);
            contact.Attributes["donotpostalmail"].Should().Equal(doNotAllow);
            contact.Attributes["donotbulkemail"].Should().Equal(doNotAllow);
            contact.Attributes["donotfax"].Should().Equal(doNotAllow);
        }
        #endregion
    }
}
