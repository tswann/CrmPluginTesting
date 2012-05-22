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
        #region Fields
        private SIServiceProvider _serviceProvider;
        private SIPluginExecutionContext _pluginExecutionContext;
        private SIOrganizationServiceFactory _orgServiceFactory;
        private SIOrganizationService _orgService;
        #endregion

        /// <summary>
        /// Constructor arranges stubs for CRM services (performs function of Setup in NUnit)
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

        /// <summary>
        /// Execute pre-validation create Contact plugin and asset that default contact
        /// preferences have been set.
        /// </summary>
        [Fact]
        public void DefaultContactPreferencesSet()
        {
            // Arrange
            ContactPlugin plugin = new ContactPlugin();
            Entity contact = new Entity("contact");
            ParameterCollection parameterCollection = new ParameterCollection();
            parameterCollection.Add("Target", contact);
            _pluginExecutionContext.InputParametersGet = () => { return parameterCollection; };
            _pluginExecutionContext.MessageNameGet = () => { return "Create"; }; // Fake a 'Create' message
            _pluginExecutionContext.StageGet = () => { return 10; }; // Fake the 'Pre-Validation' execution stage

            // Act
            plugin.Execute(_serviceProvider);

            // Assert that default contact preferences have been set
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
