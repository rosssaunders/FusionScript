namespace RxdSolutions.FusionScript.Security
{
    public class UserRights
    {
        private const string ExecuteRightName =         "FusionScript: Execute Scripts";
        private const string ManagePrivateRightName =   "FusionScript: Manage Private Scripts";
        private const string ManageFirmRightName =      "FusionScript: Manage Firm Scripts";
        private const string FullToolkitAccess =        "FusionScript: Full Toolkit Access";

        public static bool CanManagePrivate()
        {
            using (var userRights = LoadUserRights())
            {
                if (UserRightHelper.IsUserManager(userRights))
                {
                    return true;
                }

                if (IsRightEnabled(userRights, ManagePrivateRightName))
                {
                    return true;
                }

                if (userRights.GetUserDefRight(ManagePrivateRightName) == eMRightStatusType.M_rsSameAsParent)
                {
                    using (var groupRights = LoadUserRights(userRights.GetParentID()))
                    {
                        return IsRightEnabled(groupRights, ManagePrivateRightName);
                    }
                }

                return false;
            }
        }

        public static bool CanAccessFullToolkit()
        {
            using (var userRights = LoadUserRights())
            {
                if (UserRightHelper.IsUserManager(userRights))
                {
                    return true;
                }

                if (IsRightEnabled(userRights, FullToolkitAccess))
                {
                    return true;
                }

                if (userRights.GetUserDefRight(FullToolkitAccess) == eMRightStatusType.M_rsSameAsParent)
                {
                    using (var groupRights = LoadUserRights(userRights.GetParentID()))
                    {
                        return IsRightEnabled(groupRights, FullToolkitAccess);
                    }
                }

                return false;
            }
        }

        public static bool CanManageFirm()
        {
            using (var userRights = LoadUserRights())
            {
                if (UserRightHelper.IsUserManager(userRights))
                {
                    return true;
                }

                if (IsRightEnabled(userRights, ManageFirmRightName))
                {
                    return true;
                }

                if (userRights.GetUserDefRight(ManageFirmRightName) == eMRightStatusType.M_rsSameAsParent)
                {
                    using (var groupRights = LoadUserRights(userRights.GetParentID()))
                    {
                        return IsRightEnabled(groupRights, ManageFirmRightName);
                    }
                }

                return false;
            }
        }

        public static bool CanExecute()
        {
            using (var userRights = LoadUserRights())
            {
                if (UserRightHelper.IsUserManager(userRights))
                {
                    return true;
                }

                if (IsRightEnabled(userRights, ExecuteRightName))
                {
                    return true;
                }

                if (userRights.GetUserDefRight(ExecuteRightName) == eMRightStatusType.M_rsSameAsParent)
                {
                    using (var groupRights = LoadUserRights(userRights.GetParentID()))
                    {
                        return IsRightEnabled(groupRights, ExecuteRightName);
                    }
                }

                return false;
            }
        }

        private static CSMUserRights LoadUserRights(int? groupId = null)
        {
            CSMUserRights userRights;
            if (!groupId.HasValue)
                userRights = new CSMUserRights();
            else
                userRights = new CSMUserRights((uint)groupId.Value);

            userRights.LoadDetails();
            return userRights;
        }

        private static bool IsRightEnabled(CSMUserRights userRights, string rightName)
        {
            if (userRights.GetUserDefRight(rightName) == eMRightStatusType.M_rsEnable)
            {
                return true;
            }

            return false;
        }
    }
}
