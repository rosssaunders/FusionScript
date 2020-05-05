//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Net.Security;
using System.ServiceModel;
using RxdSolutions.FusionScript.Interface;

namespace RxdSolutions.FusionScript.Service
{
    public class DataServerHostFactory
    {
        private static readonly string ServiceName = $"/";

        public static Uri GetListeningAddress()
        {
            var processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            var sessionId = System.Diagnostics.Process.GetCurrentProcess().SessionId;

#if DEBUG
            return new Uri($"net.tcp://localhost:58769/FusionScript");
#else
            return new Uri($"net.tcp://localhost:58769/FusionScript/{sessionId}/{processId}");
#endif
        }

        public static ServiceHost Create(DataService server)
        {
            var baseAddress = GetListeningAddress();
            var host = new ServiceHost(server, baseAddress);

            //Add the NamedPipes endPoint
            var binding = new NetTcpBinding()
            {
                MaxReceivedMessageSize = int.MaxValue,
            };

            binding.MaxConnections = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;

            host.AddServiceEndpoint(typeof(IDataService), binding, ServiceName);

            //Secure to only the current user
            host.Authorization.ServiceAuthorizationManager = new CurrentUserOnlyAuthorizationManager();

            host.Open();

            return host;
        }
    }
}
