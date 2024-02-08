using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using win = System.Windows.Forms;
using Autodesk.Revit.DB.Structure;
using System.Text.RegularExpressions;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace level_Checker
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MyForm : Window
    {
        ExternalEvent myEvent;
        public MyForm(List<Element> sheetcollector, ExternalEvent _myEvent)
        {
            InitializeComponent();
            cmbBox.ItemsSource = sheetcollector;
            myEvent = _myEvent;
        }

        public bool GetColur()
        {
            if (setColour.IsChecked == true) { return true; }
            else if (resetColour.IsChecked == true) { return false; }
            else return false;
        }

        Document docc = null;

        internal void getDocuments(Document doc)
        {
            docc = doc;
        }
        public List<Element> GetElements(Document doc)
        {
            docc = doc;
            List<Element> returnElements = new List<Element>();
            if (walls.IsChecked == true && framing.IsChecked == true && columns.IsChecked == true)
            {
                List<Element> allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements().ToList();
                List<Element> allFraming = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements().ToList();
                List<Element> allColumns = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns).WhereElementIsNotElementType().ToElements().Cast<Element>().ToList();
                foreach (Element element in allWalls) { returnElements.Add(element); }
                foreach (Element element in allFraming) { returnElements.Add(element); }
                foreach (Element element in allColumns) { returnElements.Add(element); }
            }
            else if (walls.IsChecked == true && columns.IsChecked == true)
            {
                List<Element> allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements().ToList();
                List<Element> allColumns = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns).WhereElementIsNotElementType().ToElements().Cast<Element>().ToList();
                foreach(Element element in allWalls) { returnElements.Add(element); }
                foreach (Element element in allColumns) { returnElements.Add(element); }
            }

            else if(walls.IsChecked == true && framing.IsChecked == true)
            {
                List<Element> allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements().ToList();
                List<Element> allFraming = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements().ToList();
                foreach (Element element in allWalls) { returnElements.Add(element); }
                foreach (Element element in allFraming) { returnElements.Add(element); }
            }

            else if (columns.IsChecked == true && framing.IsChecked == true)
            {
                List<Element> allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements().ToList();
                List<Element> allColumns = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns).WhereElementIsNotElementType().ToElements().Cast<Element>().ToList();
                foreach (Element element in allWalls) { returnElements.Add(element); }
                foreach (Element element in allColumns) { returnElements.Add(element); }
            }



            else if (walls.IsChecked == true)
            {
                List<Element> allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements().ToList();
                foreach (Element element in allWalls) { returnElements.Add(element); }
            }

            else if (framing.IsChecked == true)
            {
                List<Element> allFraming = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements().ToList();
                foreach (Element element in allFraming) { returnElements.Add(element); }
            }

            else if (columns.IsChecked == true)
            {

                List<Element> allColumns = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns).WhereElementIsNotElementType().ToElements().Cast<Element>().ToList();
                foreach (Element element in allColumns) { returnElements.Add(element); }
            }

            return returnElements;
        }



        internal Element getLevel()
        {
            if (cmbBox.SelectedItem != null)
            {
                // Get the name of the selected item
                Element itemName = cmbBox.SelectedItem as Level;
                return itemName;

            }
            return null;

        }

        private void apply_Click(object sender, RoutedEventArgs e)
        {
            Globals.SelectedElements = GetElements(docc);
            Globals.isSetcolour = GetColur();
            Globals.levelElement = getLevel();

            myEvent.Raise();
        }


    }
}
