﻿using System;
using System.Linq;
using CloudAwesome.Xrm.Simulate.QueryParsers.ConditionHandlers;
using CloudAwesome.Xrm.Simulate.Test.EarlyBoundEntities;
using CloudAwesome.Xrm.Simulate.Test.TestEntities;
using FluentAssertions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace CloudAwesome.Xrm.Simulate.Test.QueryParserTests.ConditionHandlerTests;

[TestFixture]
public class NextXMonthsTests
{
    private IOrganizationService _organizationService = null!;
    
    private readonly Contact _positiveContact = Arthur.Contact();
    private readonly Contact _oldNegativeContact = Bruce.Contact();
    private readonly Contact _futureNegativeContact = Daniel.Contact();

    [SetUp]
    public void SetUp()
    {
        _positiveContact.OverriddenCreatedOn = new DateTime(2023, 08, 12);
        _oldNegativeContact.OverriddenCreatedOn = new DateTime(2023, 05, 09);
        _futureNegativeContact.OverriddenCreatedOn = new DateTime(2024, 01, 02);
        
        var options = new SimulatorOptions
        {
            ClockSimulator = new MockSystemTime(new DateTime(2023, 06, 12))
        };
        
        _organizationService = _organizationService.Simulate(options);
    }

    [Test]
    public void QueryExpression_Returns_Positive_Results()
    {
        _organizationService.Simulated().Data().Add(_positiveContact);
        _organizationService.Simulated().Data().Add(_oldNegativeContact);
        _organizationService.Simulated().Data().Add(_futureNegativeContact);

        var contacts = _organizationService.RetrieveMultiple(_queryExpression);

        contacts.Entities.Count().Should().Be(1);
    }
    
    [Test]
    public void QueryExpression_Returns_Empty_Set_If_None_Found()
    {
        _organizationService.Simulated().Data().Add(_oldNegativeContact);
        _organizationService.Simulated().Data().Add(_futureNegativeContact);

        var contacts = _organizationService.RetrieveMultiple(_queryExpression);

        contacts.Entities.Count().Should().Be(0);
    }
    
    [Test]
    public void FetchExpression_Returns_Positive_Results()
    {
        _organizationService.Simulated().Data().Add(_positiveContact);
        _organizationService.Simulated().Data().Add(_oldNegativeContact);
        _organizationService.Simulated().Data().Add(_futureNegativeContact);

        var contacts = _organizationService.RetrieveMultiple(_fetchQuery);

        contacts.Entities.Count().Should().Be(1);
    }
    
    [Test]
    public void FetchExpression_Returns_Empty_Set_If_None_Found()
    {
        _organizationService.Simulated().Data().Add(_oldNegativeContact);
        _organizationService.Simulated().Data().Add(_futureNegativeContact);

        var contacts = _organizationService.RetrieveMultiple(_fetchQuery);

        contacts.Entities.Count().Should().Be(0);
    }
    
    [Test]
    public void Correct_ConditionOperator_Is_Set()
    {
        var handler = new NextXMonthsConditionHandler();
        handler.Operator.Should().Be(ConditionOperator.NextXMonths);
    }
    
    private readonly QueryExpression _queryExpression = new()
    {
        EntityName = Contact.EntityLogicalName,
        Criteria = new FilterExpression
        {
            Conditions =
            {
                new ConditionExpression(Contact.Fields.OverriddenCreatedOn, 
                    ConditionOperator.NextXMonths, 4)
            }
        },
        ColumnSet = new ColumnSet(
            Contact.Fields.FirstName, 
            Contact.Fields.LastName)
    };
    
    private readonly FetchExpression _fetchQuery = new()
    { 
        Query = @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
                  <entity name=""contact"">
                    <attribute name=""firstname"" />
                    <attribute name=""lastname"" />
                    <order attribute=""fullname"" descending=""false"" />
                    <filter type=""and"">
                      <condition attribute=""overriddencreatedon"" operator=""next-x-months"" value=""4"" />
                    </filter>
                  </entity>
                </fetch>" 
    };
}