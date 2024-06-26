﻿using CloudAwesome.Xrm.Simulate.DataServices;
using CloudAwesome.Xrm.Simulate.Interfaces;
using Microsoft.Xrm.Sdk;
using NSubstitute;

namespace CloudAwesome.Xrm.Simulate.ServiceProviders;

public static class ServiceEndpointNotificationSimulator
{
    public static IServiceEndpointNotificationService? Create(MockedEntityDataService dataService, ISimulatorOptions? options)
    {
        if (dataService.FakeServiceFailureSettings is { ServiceEndpointNotificationService: true })
        {
            return null;
        }
        
        var notificationSimulator = Substitute.For<IServiceEndpointNotificationService>();
        
        // TODO - Mock out the .Execute method
        

        return notificationSimulator;
    } 
}