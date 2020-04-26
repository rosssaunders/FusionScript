using System;

namespace RxdSolutions.FusionScript.ViewModels
{
    public static class UIHelper
    {
        public static string GetModificationName(int modification)
        {
            if (modification == 1)
            {
                return "Created";
            }
            else if (modification == 2)
            {
                return "Modified";
            }
            else if (modification == 3)
            {
                return "Deleted";
            }

            throw new ApplicationException($"Unknown modification type {modification}");
        }
    }
}
