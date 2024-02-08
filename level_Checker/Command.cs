#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Controls;
using System.Windows.Forms;
using APP = System.Windows;

#endregion

namespace level_Checker
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // put any code needed for the form here

            // open form

            EventAction action = new EventAction();
            ExternalEvent myEvent = ExternalEvent.Create(action);

            List<Element> sheetcollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElements().Cast<Element>().ToList();

            var currentForm = new MyForm(sheetcollector, myEvent)
            {
                Width = 800,
                Height = 800,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = false,
            };

            currentForm.getDocuments(doc);

            currentForm.Show();


            // get form data and do something

            return Result.Succeeded;
        }

        public string getParameterValue(Element e, string paramname)
        {
            IList<Parameter> parameters = e.GetParameters(paramname);
            Parameter parameter = parameters.First();
            return parameter.AsString();
        }


        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }

    }

    public class EventAction : IExternalEventHandler
    {
        public void Execute(UIApplication uIApp)
        {


            Document doc = uIApp.ActiveUIDocument.Document;

            //List<Element> selectedElements = currentForm.GetElements(doc);

            OverrideGraphicSettings setGraphicSettings = new OverrideGraphicSettings();
            if(Globals.isSetcolour == true)
            {
                Color newColor = new Color(255, 255, 0);
                setGraphicSettings.SetCutBackgroundPatternColor(newColor);
                setGraphicSettings.SetCutForegroundPatternColor(newColor);
                setGraphicSettings.SetSurfaceBackgroundPatternColor(newColor);
                setGraphicSettings.SetSurfaceForegroundPatternColor(newColor);

                FillPatternElement curPat = FillPatternElement.GetFillPatternElementByName(doc, FillPatternTarget.Drafting, "<Solid fill>");
                setGraphicSettings.SetCutForegroundPatternId(curPat.Id);
                setGraphicSettings.SetCutBackgroundPatternId(curPat.Id);
                setGraphicSettings.SetSurfaceForegroundPatternId(curPat.Id);
                setGraphicSettings.SetSurfaceBackgroundPatternId(curPat.Id);
            }


            Transaction t = new Transaction(doc);

            t.Start("Set & Reset Colur");
            {
                foreach (Element element in Globals.SelectedElements)
                {
                    //string curLevel = getParameterValue(element, "Base Constraint");
                    string cat = element.Category.Name;
                    string cat1 = "Structural Framing";
                    if (cat == cat1)
                    {
                        String LEVELVALUE = getparametervalue(element, "Reference Level");
                        Element levelValue2 = Globals.levelElement;
                        if(levelValue2.Name == LEVELVALUE) 
                        {
                            doc.ActiveView.SetElementOverrides(element.Id, setGraphicSettings);
                        }
                    }
                    else
                    {
                        Element levelValue = Globals.levelElement;
                        if (element.LevelId.Equals(levelValue.Id))
                        {
                            doc.ActiveView.SetElementOverrides(element.Id, setGraphicSettings);
                        }
                    }
                    


                }
            }


            t.Commit();
            t.Dispose();

        }

        public string GetName()
        {
            return "EventAction";
        }

        internal string getparametervalue(Element e, string paramname)
        {
            IList<Parameter> parameters = e.GetParameters(paramname);
            Parameter parameter = parameters.First();
            return parameter.AsValueString();
        }

    }

}
