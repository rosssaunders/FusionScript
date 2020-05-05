//  Copyright (c) RXD Solutions. All rights reserved.
//  FusionLink is licensed under the MIT license. See LICENSE.txt for details.

using System;
using System.Windows;
using RxdSolutions.FusionScript.Security;
using sophis.scenario;
using sophis.utils;

namespace RxdSolutions.FusionScript.Triggers
{
    internal class ExecuteScriptScenario : CSMScenario
    {
        public override eMProcessingType GetProcessingType()
        {
            return eMProcessingType.M_pUserPreference;
        }

        public override bool AlwaysEnabled()
        {
            return true;
        }

        public override CMString GetName()
        {
            return "Execute FusionScript";
        }

        public override void Run()
        {
            if (UserRights.CanExecute())
            {
            }
            else
            {
                throw new ApplicationException("You do not have permissions to execute scripts");
            }

            base.Run();
        }
    }
}
