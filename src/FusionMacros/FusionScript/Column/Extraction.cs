using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sophis.portfolio;

namespace FusionScript.Column
{
    public class Extraction
    {
        public Extraction(int extractionId)
        {
            Id = extractionId;
        }

        public int Id { get; }

        public CSMExtraction GetExtraction()
        {
            return CSMExtraction.GetExtractionFromInternalID(Id);
        }
    }
}
