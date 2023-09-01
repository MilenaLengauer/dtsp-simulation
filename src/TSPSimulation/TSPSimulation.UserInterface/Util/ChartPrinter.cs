using Microsoft.Win32;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPSimulation.UserInterface.Util
{
    public class ChartPrinter
    {
        public void PrintChart(PlotModel model, int width, int height)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Pdf Files|*.pdf",
                FileName = "chart",
                DefaultExt = ".pdf",
                Title = "Save Chart"
            };
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                using (var stream = File.Create(saveFileDialog.FileName))
                {
                    var pdfExporter = new PdfExporter { Width = width, Height = height };
                    pdfExporter.Export(model, stream);
                }
            }
        }

    }
}
