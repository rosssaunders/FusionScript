import clr
clr.AddReference("System.Windows.Forms")

import FusionScript as fm
import System.Windows.Forms as forms

selectedFolios = fm.Application.SelectedPortfolios
forms.MessageBox.Show(str(selectedFolios))

selectedPositions = fm.Application.SelectedPositions
forms.MessageBox.Show(str(selectedPositions))
