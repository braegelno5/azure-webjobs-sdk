﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Microsoft.Azure.WebJobs.ServiceBus.Listeners
{
    internal class ServiceBusSubscriptionListenerFactory : IListenerFactory
    {
        private readonly NamespaceManager _namespaceManager;
        private readonly MessagingFactory _messagingFactory;
        private readonly string _topicName;
        private readonly string _subscriptionName;
        private readonly ITriggeredFunctionInstanceFactory<BrokeredMessage> _instanceFactory;

        public ServiceBusSubscriptionListenerFactory(ServiceBusAccount account, string topicName,
            string subscriptionName, ITriggeredFunctionInstanceFactory<BrokeredMessage> instanceFactory)
        {
            _namespaceManager = account.NamespaceManager;
            _messagingFactory = account.MessagingFactory;
            _topicName = topicName;
            _subscriptionName = subscriptionName;
            _instanceFactory = instanceFactory;
        }

        public async Task<IListener> CreateAsync(IFunctionExecutor executor, CancellationToken cancellationToken)
        {
            // Must create all messaging entities before creating message receivers and calling OnMessage.
            // Otherwise, some function could start to execute and try to output messages to entities that don't yet
            // exist.
            await _namespaceManager.CreateTopicIfNotExistsAsync(_topicName, cancellationToken);
            await _namespaceManager.CreateSubscriptionIfNotExistsAsync(_topicName, _subscriptionName,
                cancellationToken);

            string entityPath = SubscriptionClient.FormatSubscriptionPath(_topicName, _subscriptionName);

            ITriggerExecutor<BrokeredMessage> triggerExecutor = new ServiceBusTriggerExecutor(_instanceFactory,
                executor);
            return new ServiceBusListener(_messagingFactory, entityPath, triggerExecutor);
        }
    }
}
