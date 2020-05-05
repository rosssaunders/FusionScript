//  Copyright (c) RXD Solutions. All rights reserved.
using System;
using System.Security.Principal;
using System.ServiceModel;

namespace RxdSolutions.FusionScript.Service
{
    public class CurrentUserOnlyAuthorizationManager : ServiceAuthorizationManager
    {
        public CurrentUserOnlyAuthorizationManager()
        {
        }

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            var currentUser = WindowsIdentity.GetCurrent()?.User;
            var contextUser = operationContext?.ServiceSecurityContext?.WindowsIdentity?.User;
            if (currentUser == null || contextUser == null)
            {
                return false;
            }

            return currentUser.Equals(contextUser);
        }
    }
}
