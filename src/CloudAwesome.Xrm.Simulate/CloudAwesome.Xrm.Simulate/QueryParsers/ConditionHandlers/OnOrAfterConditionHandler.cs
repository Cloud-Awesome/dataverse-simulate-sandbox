﻿using CloudAwesome.Xrm.Simulate.DataServices;
using CloudAwesome.Xrm.Simulate.Interfaces;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CloudAwesome.Xrm.Simulate.QueryParsers.ConditionHandlers;

public class OnOrAfterConditionHandler : IConditionHandler
{
    public ConditionOperator Operator => ConditionOperator.OnOrAfter;

    public bool Evaluate(Entity entity, ConditionExpression condition, MockedEntityDataService dataService)
    {
        var attributeValue = entity.GetAttributeValue<DateTime>(condition.AttributeName);
        var targetDate = Convert.ToDateTime(condition.Values[0]);
        
        return attributeValue.Date >= targetDate.Date;
    }
}