using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using sophis.reporting;
using sophis.reporting.gui;

namespace RxdSolutions.FusionScript.Reporting
{
    public class FusionScriptXMLSourceGUI : CSMXMLSourceGUI
    {
        FusionScriptReportingSourceGUI view = null;

        public FusionScriptXMLSourceGUI()
        {

        }

        public override bool FillDataFromDialog(CSMXMLSource hData)
        {
            if (hData is FusionScriptXMLSource xs)
            {
                xs.ScriptId = view.ViewModel.ScriptId;
            }

            return true;
        }

        public override bool FillDialogFromData(CSMXMLSource hData)
        {
            if (hData is FusionScriptXMLSource xs)
            {
                view.ViewModel.ScriptId = xs.ScriptId;
            }

            return true;
        }

        public override System.Windows.Forms.Form GetView()
        {
            return null;
        }

        public override FrameworkElement GetXAMLView()
        {
            if (view == null)
            {
                view = new FusionScriptReportingSourceGUI();
                view.DataContext = new FusionScriptReportingSourceGUIViewModel(Main.ScriptCache);
            }

            return view;
        }

        public override bool IsWinForm()
        {
            return false;
        }
    }
}
