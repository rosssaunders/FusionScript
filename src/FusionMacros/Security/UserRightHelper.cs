using System;
using sophis.utils;

namespace RxdSolutions.FusionScript.Security
{
    public static class UserRightHelper
    {
        public static bool IsUserManager(CSMUserRights userRights)
        {
            using (var name = userRights.GetName())
            {
                if (name.StringValue.Equals("MANAGER", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static int GetCurrentUserId()
        {
            using (var str = new CMString())
            {
                sophis.globals.CSMApi.gApi().GetUser(str);
                var id = CSMUserRights.ConvNameToIdent(str);

                return (int)id;
            }
        }
    }
}
